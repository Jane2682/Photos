using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Photos
{
    public static class PhotosDownload
    {
        [FunctionName("PhotosDownload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "photos/{id}")] HttpRequest req,//lielam photosID ka parametru funkcijas route atributam
                                                                                                      //objekta attels URI pedejais segments tiek izmantots ka ID
            [Blob("photos-small/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream imageSmall,//tiek pievinots Blob atributs
                                                                                                                              //kas reprezentes maza  izmera objektu ar stream atributa palidzibu
            [Blob("photos-medium/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream imageMedium,//tiek pievinots Blob atributs
                                                                                                                                //kas reprezentes videja  izmera objektu ar stream atributa palidzibu
            [Blob("photos/{id}.jpg", FileAccess.Read, Connection = Literals.StorageConnectionString)] Stream imageOriginal,//tiek pievinots Blob atributs
                                                                                                                           //kas reprezentes originala  izmera objektu ar stream atributa palidzibu
            Guid id,
            ILogger logger)
        {
            logger?.LogInformation($"Downloading {id}...");//ari seit tiks izmantots attela ID , lai tas paradas loggera zinojuma

            byte[] data;//tiek izveidota metode kas atgriezis noteikta izmera attelu

            if (req.Query["size"] == "sm")
            {
                logger?.LogInformation("Retrieving the small size");//pazinojums par maza izmera attela atgusanu
                data = await GetBytesFromStreamAsync(imageSmall);
            }
            else if (req.Query["size"] == "md")
            {
                logger?.LogInformation("Retrieving the medium size");//pazinojums par videja izmera attela atgusanu
                data = await GetBytesFromStreamAsync(imageMedium);
            }
            else
            {
                logger?.LogInformation("Retrieving the original size");//pazinojums par originala izmera attela atgusanu
                data = await GetBytesFromStreamAsync(imageOriginal);
            }

            return new FileContentResult(data, "image/jpeg")//atgriezam pieprasito datni
            {
                FileDownloadName = $"{id}.jpg"
            };
        }

        private static async Task<byte[]> GetBytesFromStreamAsync(Stream stream)//si metode atgriez baitu masivu no viena noteikta Streama.
        {
            byte[] data = new byte[stream.Length];
            await stream.ReadAsync(data, 0, data.Length);
            return data;
        }
    }
}

