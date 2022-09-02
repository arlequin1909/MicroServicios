using AutoMapper;
using Subastalo.Models.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fotoTeca.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            //Mapeo desde y hacia 
            CreateMap<FileRequeride, FileDateTable>();
        }
    }
}
