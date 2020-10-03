using AbleSync.Core.Exceptions;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    /// <summary>
    ///     Scraping service which can scrape Ableton project folders.
    /// </summary>
    public class ProjectScrapingService : IProjectScrapingService
    {
        protected readonly IFileTrackingService _fileTrackingService;
        protected readonly IProjectRepository _projectRepository;
        protected readonly ILogger<ProjectScrapingService> _logger;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectScrapingService(IFileTrackingService fileTrackingService,
            IProjectRepository projectRepository,
            ILogger<ProjectScrapingService> logger)
        {
            _fileTrackingService = fileTrackingService ?? throw new ArgumentNullException(nameof(fileTrackingService));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        ///     This calls <see cref="ProcessAbletonProjectFolderAsync(DirectoryInfo)"/>
        ///     recursively for each item in the start directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to process recursively.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public async Task ProcessDirectoryRecursivelyAsync(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            try
            {
                _logger.LogTrace($"Analyzing directory recursively: {directoryInfo.FullName}");

                if (ProjectFolderHelper.IsAbletonProjectFolder(directoryInfo))
                {
                    await ProcessAbletonProjectFolderAsync(directoryInfo);
                }
                else
                {
                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        if (ProjectFolderHelper.IsAbletonProjectFolder(directoryInfo))
                        {
                            await ProcessAbletonProjectFolderAsync(directoryInfo);
                        }
                        else
                        {
                            await ProcessDirectoryRecursivelyAsync(directory);
                        }
                    }
                }
            }
            catch (IOException e)
            {
                _logger.LogError(e.Message);
                throw new FileAccessException("Could not determine if directory was Ableton project", e);
            }
        }

        /// <summary>
        ///     Processes a single Ableton project folder by syncing its metadata
        ///     to our data store. If the state is considered invalid, this is
        ///     marked both in our data store and in our tracking file.
        /// </summary>
        /// <remarks>
        ///     If the given <paramref name="directoryInfo"/> is not an Ableton 
        ///     project directory, this will throw an exception of type
        ///     <see cref="NotAnAbletonProjectFolderException"/>.
        /// </remarks>
        /// <param name="directoryInfo">The directory to process.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public async Task ProcessAbletonProjectFolderAsync(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }

            try
            {
                if (!ProjectFolderHelper.IsAbletonProjectFolder(directoryInfo))
                {
                    throw new NotAnAbletonProjectFolderException();
                }

                if (_fileTrackingService.HasTrackingFile(directoryInfo))
                {
                    // If marked invalid, this service will log and skip.
                    var trackingFile = _fileTrackingService.GetTrackingFile(directoryInfo);
                    if (trackingFile.ProjectStatus == ProjectStatus.Invalid)
                    {
                        _logger.LogInformation($"Project with id {trackingFile.ProjectId} marked as invalid, skipping");
                        return;
                    }

                    if (!await _projectRepository.ExistsAsync(trackingFile.ProjectId))
                    {
                        // If the project has a tracking file but does not exist in the store, 
                        // we have reached an invalid state. This should never happen.
                        trackingFile.ProjectStatus = ProjectStatus.Invalid;
                        _fileTrackingService.UpdateTrackingFile(directoryInfo, trackingFile);
                        _logger.LogWarning($"Project with id {trackingFile.ProjectId} has a tracking file" +
                            $"but does not exist in the store - marked as invalid.");
                        return;
                    }
                    else
                    {
                        // The state where there exists a tracking file and where the project
                        // exists in the data store is valid. We can continue to process.
                        await UpdateProjectAsync(directoryInfo);
                        _logger.LogTrace($"Project with id {trackingFile.ProjectId} has been updated");
                        return;
                    }
                }
                else
                {
                    // If no tracking file exists we can only assume that the project is 
                    // not yet being tracked. This will start tracking the project.
                    // TODO Move this to some helper class.
                    var extractedProject = ProjectFolderHelper.ExtractProject(directoryInfo);
                    var createdProject = await _projectRepository.CreateAsync(extractedProject);
                    _fileTrackingService.CreateTrackingFile(createdProject.Id, directoryInfo);
                    _logger.LogTrace($"A new tracking file has been created for project {createdProject.Id} at {directoryInfo.FullName}");
                    return;

                    // Note: This service does not consider the possibility of a project
                    // existing in the data store without it having a tracking file.
                }
            }
            catch (IOException e)
            {
                _logger.LogError(e.Message);
                throw new FileAccessException("Could not determine if directory was Ableton project", e);
            }
        }

        // TODO Implement.
        private Task UpdateProjectAsync(DirectoryInfo directoryInfo)
        {
            var trackingFile = _fileTrackingService.GetTrackingFile(directoryInfo);

            // TODO Process

            _fileTrackingService.UpdateTrackingFile(directoryInfo, trackingFile);

            return Task.CompletedTask;
        }

    }
}
