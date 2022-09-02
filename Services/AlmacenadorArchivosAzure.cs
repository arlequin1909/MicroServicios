using fotoTeca.Services;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NegronWebApi.services
{
    public class AlmacenadorArchivosAzure : IAlmacenadorArchivos
    {
        private readonly string connectionString;
        public AlmacenadorArchivosAzure(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }
        public async Task BorrarArchivo(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var cuenta = CloudStorageAccount.Parse(connectionString);
                var cliente = cuenta.CreateCloudBlobClient();
                var contenedorReferencia = cliente.GetContainerReference(contenedor);

                var nombreBlob = Path.GetFileName(ruta);
                var blob = contenedorReferencia.GetBlockBlobReference(nombreBlob);
                await blob.DeleteIfExistsAsync();
            }

        }

        public async Task<string> EditarArchivo(byte[] contenido, string extencion, string contenedor, string ruta, string contentType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extencion, contenedor, contentType);
        }



        public async Task<string> GuardarArchivo(byte[] contenido, string extencion, string contenedor, string contentType)
        {
            var cuenta = CloudStorageAccount.Parse(connectionString);
            var cliente = cuenta.CreateCloudBlobClient();
            var contenedorReferencia = cliente.GetContainerReference(contenedor);

            await contenedorReferencia.CreateIfNotExistsAsync();
            await contenedorReferencia.SetPermissionsAsync(new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            });
            var nombreArchivo = $"{Guid.NewGuid()}{extencion}";
            var blob = contenedorReferencia.GetBlockBlobReference(nombreArchivo);
            await blob.UploadFromByteArrayAsync(contenido, 0, contenido.Length);
            blob.Properties.ContentType = contentType;
            await blob.SetPropertiesAsync();
            return blob.Uri.ToString();
        }
    }
}
