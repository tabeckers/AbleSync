using AbleSync.Api.DataTransferObjects;
using AbleSync.Core.Interfaces.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenameMe.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbleSync.Api.Controllers
{
    /// <summary>
    ///     Controller for project related operations.
    /// </summary>
    [Route("project")]
    public sealed class ProjectController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public ProjectController(IMapper mapper,
            IProjectRepository projectRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
        }

        /// <summary>
        ///     Gets a project by its id.
        /// </summary>
        /// <param name="id">The project id.</param>
        /// <returns>The retrieved project.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectAsync([FromRoute] Guid id)
        {
            // Check.
            id.ThrowIfNullOrEmpty();

            // Act.
            var project = await _projectRepository.GetAsync(id, HttpContext.RequestAborted);

            // Map.
            var result = _mapper.Map<ProjectDTO>(project);

            // Return.
            return Ok(result);
        }

        // TODO Pagination.
        /// <summary>
        ///     Gets all projects from our data store.
        /// </summary>
        /// <returns>Collection of projects.</returns>
        [HttpGet("all")]      
        public async Task<IActionResult> GetProjectsAsync()
        {
            // Map.
            var result = new List<ProjectDTO>();
            await foreach (var project in _projectRepository.GetAllAsync(HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<ProjectDTO>(project));
            }

            // Return.
            return Ok(result);
        }

        // TODO Pagination.
        /// <summary>
        ///     Gets all projects from our data store ordered
        ///     by latest update date.
        /// </summary>
        /// <returns>Collection of latest projects.</returns>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestProjectsAsync()
        {
            // Map.
            var result = new List<ProjectDTO>();
            await foreach (var project in _projectRepository.GetLatestAsync(HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<ProjectDTO>(project));
            }

            // Return.
            return Ok(result);
        }
    }
}
