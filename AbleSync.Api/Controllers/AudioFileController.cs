﻿using AbleSync.Api.DataTransferObjects;
using AbleSync.Core.Interfaces.Services;
using AbleSync.Core.Types;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RenameMe.Utility.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AbleSync.Api.Controllers
{
    /// <summary>
    ///     Controller for audio file related operations.
    /// </summary>
    [Route("audiofile")]
    public sealed class AudioFileController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAudioFileService _audioFileService;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public AudioFileController(IMapper mapper,
            IAudioFileService audioFileService)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _audioFileService = audioFileService ?? throw new ArgumentNullException(nameof(audioFileService));
        }

        /// <summary>
        ///     Gets an audio file by its id.
        /// </summary>
        /// <param name="id">The audio file id.</param>
        /// <returns>The retrieved audio file.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudioFileAsync([FromRoute] Guid id)
        {
            // Check.
            id.ThrowIfNullOrEmpty();

            // Act.
            var project = await _audioFileService.GetAsync(id, HttpContext.RequestAborted);

            // Map.
            var result = _mapper.Map<AudioFileDTO>(project);

            // Return.
            return Ok(result);
        }

        /// <summary>
        ///     Gets an audio file by its id.
        /// </summary>
        /// <param name="id">The audio file id.</param>
        /// <returns>The retrieved audio file.</returns>
        [HttpGet("{id}/download")]
        public async Task<IActionResult> GetAudioFileDownloadAsync([FromRoute] Guid id)
        {
            // Check.
            id.ThrowIfNullOrEmpty();

            // Act.
            var uri = await _audioFileService.GetAccessUriAsync(id, HttpContext.RequestAborted);

            // Return.
            return Redirect(uri.ToString());
        }

        // TODO Pagination.
        /// <summary>
        ///     Gets all audio files from our data store.
        /// </summary>
        /// <returns>Collection of audio files.</returns>
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAudioFilesAsync()
        {
            // Act and map.
            var result = new List<AudioFileDTO>();
            await foreach (var audioFile in _audioFileService.GetLastestAsync(Pagination.Default, HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<AudioFileDTO>(audioFile));
            }

            // Return.
            return Ok(result);
        }

        // TODO Pagination.
        /// <summary>
        ///     Gets all audio files from our data store ordered
        ///     by latest update date.
        /// </summary>
        /// <returns>Collection of latest audio files.</returns>
        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestAudioFilesAsync()
        {
            // Act and map.
            var result = new List<AudioFileDTO>();
            await foreach (var audioFile in _audioFileService.GetLastestAsync(Pagination.Default, HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<AudioFileDTO>(audioFile));
            }

            // Return.
            return Ok(result);
        }

        /// <summary>
        ///     Gets all audio files that belong to a project.
        /// </summary>
        /// <param name="projectId">The respective project id.</param>
        /// <returns>Collection of audio files.</returns>
        [HttpGet("byproject/{projectId}")]
        public async Task<IActionResult> GetLatestAudioFilesAsync([FromRoute] Guid projectId)
        {
            // Prepare.
            projectId.ThrowIfNullOrEmpty();

            // Act and map.
            var result = new List<AudioFileDTO>();
            await foreach (var audioFile in _audioFileService.GetForProjectAsync(projectId, Pagination.Default, HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<AudioFileDTO>(audioFile));
            }

            // Return.
            return Ok(result);
        }

        // TODO Pagination.
        /// <summary>
        ///     Search for audio files in our data store.
        /// </summary>
        /// <returns>Audio files search result.</returns>
        [HttpGet("search")]
        public async Task<IActionResult> SearchAudioFilesAsync(string query)
        {
            // Prepare.
            query.ThrowIfNullOrEmpty();

            // Act and map.
            var result = new List<AudioFileDTO>();
            await foreach (var audioFile in _audioFileService.SearchAsync(query, Pagination.Default, HttpContext.RequestAborted))
            {
                result.Add(_mapper.Map<AudioFileDTO>(audioFile));
            }

            // Return.
            return Ok(result);
        }
    }
}
