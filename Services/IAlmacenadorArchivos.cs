using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fotoTeca.Services
{
    public interface IAlmacenadorArchivos
    {
        Task<string> EditarArchivo(byte[] contenido, string extencion, string contenedor, string ruta,
            string contentType);
        Task BorrarArchivo(string ruta, string contenedor);
        Task<string> GuardarArchivo(byte[] contenido, string extencion, string contenedor, string contentType);
    }
}
