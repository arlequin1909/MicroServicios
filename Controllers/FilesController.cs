using AutoMapper;
using fotoTeca.Autentication;
using fotoTeca.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Subastalo.Models.File;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Subastalo.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FilesController : ControllerBase
    {
        private readonly AplicationDbContex context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "files";


        public FilesController(AplicationDbContex contex, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = contex;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpPost("/Files")]
        public async Task<string> Post2([FromForm] FileRequeride req)
        {
            var entidad = mapper.Map<FileDateTable>(req);
            
            var URL = "";
            if (req.IMG != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await req.IMG.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extencion = Path.GetExtension(req.IMG.FileName);
                    URL = await almacenadorArchivos.GuardarArchivo(contenido, extencion, contenedor, req.IMG.ContentType);
                }
            }


            return " { \"url\": \"" + URL + "\" } ";
        }

    }
}
