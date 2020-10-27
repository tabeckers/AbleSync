using AbleSync.Api.DataTransferObjects;
using AbleSync.Core.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleSync.Api
{
    /// <summary>
    ///     Contains our mapper configuration.
    /// </summary>
    public class MapperProfile : Profile
    {
        /// <summary>
        ///     Sets up our Automapper profile.
        /// </summary>
        public MapperProfile()
        {
            CreateMap<Artist, ArtistDTO>().ReverseMap();
            CreateMap<AudioFile, AudioFileDTO>().ReverseMap();
            CreateMap<Project, ProjectDTO>().ReverseMap();
        }
    }
}
