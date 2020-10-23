using AbleSync.Core.Exceptions;
using AbleSync.Core.Helpers;
using AbleSync.Core.Interfaces.Repositories;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AbleSync.Core.Services
{
    // TODO Look at this properly for version 0.2.
    /// <summary>
    ///     Scraping service which can scrape Ableton project folders.
    /// </summary>
    /// <remarks>
    ///     This does nothing with project tasks.
    /// </remarks>
    public class ProjectScrapingService : IProjectScrapingService
    {
        protected readonly IFileTrackingService _fileTrackingService;
        protected readonly IProjectRepository _projectRepository;
        protected readonly ILogger<ProjectScrapingService> _logger;
        protected readonly AbleSyncOptions _options;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectScrapingService(IFileTrackingService fileTrackingService,
            IProjectRepository projectRepository,
            ILogger<ProjectScrapingService> logger,
            IOptions<AbleSyncOptions> options)
        {
            _fileTrackingService = fileTrackingService ?? throw new ArgumentNullException(nameof(fileTrackingService));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        ///     This calls <see cref="ProcessAbletonProjectFolderAsync(DirectoryInfo)"/>
        ///     recursively for each item in the start directory.
        /// </summary>
        /// <param name="directoryInfo">The directory to process recursively.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public async Task ProcessDirectoryRecursivelyAsync(DirectoryInfo directoryInfo, CancellationToken token)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            try
            {
                _logger.LogTrace($"Analyzing directory recursively: {directoryInfo.FullName}");

                if (ProjectFolderHelper.IsAbletonProjectFolder(directoryInfo))
                {
                    await ProcessAbletonProjectFolderAsync(directoryInfo, token);
                }
                else
                {
                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        if (ProjectFolderHelper.IsAbletonProjectFolder(directoryInfo))
                        {
                            await ProcessAbletonProjectFolderAsync(directoryInfo, token);
                        }
                        else
                        {
                            await ProcessDirectoryRecursivelyAsync(directory, token);
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

        // TODO TransactionScope?
        /// <summary>
        ///     Processes a single Ableton project folder by syncing its metadata
        ///     to our data store. If the state is considered invalid, this is
        ///     marked both in our data store and in our tracking file.
        /// </summary>
        /// <remarks>
        ///     This does nothing with project tasks.
        /// 
        ///     If the given <paramref name="directoryInfo"/> is not an Ableton 
        ///     project directory, this will throw an exception of type
        ///     <see cref="NotAnAbletonProjectFolderException"/>.
        ///     
        ///     Note: This service does not consider the possibility of a project
        ///     existing in the data store without it having a tracking file.
        /// </remarks>
        /// <param name="directoryInfo">The directory to process.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>See <see cref="Task"/>.</returns>
        public async Task ProcessAbletonProjectFolderAsync(DirectoryInfo directoryInfo, CancellationToken token)
        {
            if (directoryInfo == null)
            {
                throw new ArgumentNullException(nameof(directoryInfo));
            }
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
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

                    if (!await _projectRepository.ExistsAsync(trackingFile.ProjectId, token))
                    {
                        // If the project has a tracking file but does not exist in the store, 
                        // we have reached an invalid state. This should never happen.
                        _fileTrackingService.MarkTrackingFileInvalid(directoryInfo);
                        _logger.LogWarning($"Project with id {trackingFile.ProjectId} has a tracking file" +
                            $"but does not exist in the store - marked as invalid.");
                        return;
                    }
                    else
                    {
                        // The state where there exists a tracking file and where the project
                        // exists in the data store is valid. We can continue to process.
                        // TODO Transaction?
                        var project = await _projectRepository.GetAsync(trackingFile.ProjectId, token);

                        project = await _projectRepository.MarkProjectAsScrapedAsync(trackingFile.ProjectId, token);
                        
                        _fileTrackingService.UpdateTrackingFile(directoryInfo, project);

                        _logger.LogTrace($"Project with id {trackingFile.ProjectId} has been updated");
                        return;
                    }
                }
                else
                {
                    // If no tracking file exists we can only assume that the project is 
                    // not yet being tracked. This will start tracking the project.
                    var extractedProject = ProjectFolderHelper.ExtractProject(directoryInfo);

                    // TODO This should not happen here.
                    var root = _options.RootDirectoryPath.LocalPath;
                    extractedProject.RelativePath = PathHelper.GetRelativePath(directoryInfo.FullName, root);

                    // TODO Transaction?
                    var createdProject = await _projectRepository.CreateAsync(extractedProject, token);
                    _fileTrackingService.CreateTrackingFile(createdProject.Id, directoryInfo);
                    _logger.LogTrace($"A new tracking file has been created for project {createdProject.Id} at {directoryInfo.FullName}");

                    return;
                }
            }
            catch (IOException e)
            {
                _logger.LogError(e.Message);
                throw new FileAccessException("Could not determine if directory was Ableton project", e);
            }
        }

        /// <summary>
        ///     Calls <see cref="ProcessDirectoryRecursivelyAsync(DirectoryInfo, CancellationToken)"/>
        ///     with the <see cref="AbleSyncOptions.RootDirectoryPath"/> as the directory.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        public Task ProcessRootDirectoryRecursivelyAsync(CancellationToken token)
            => ProcessDirectoryRecursivelyAsync(new DirectoryInfo(_options.RootDirectoryPath.LocalPath), token);
    }
}
