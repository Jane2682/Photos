using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;
using Photos.Models;
using Photos.AnalyzerService.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace Photos
{
    public  class PhotosStorage//iebuvetas funkcijas Function1 nosaukuma maina
                                     //, nepiecisama lai vieglak saprast funkcijas nozimi
    {
/*        private readonly IAnalyzerService analyzerService;

        public PhotosStorage(IAnalyzerService analyzerService)
        {
            this.analyzerService = analyzerService;
        }*/

        [FunctionName("PhotosStorage")]
        public async Task<byte[]> Run(
             // [HttpTrigger(AuthorizationLevel.Anonymous,  "post", Route = null)] HttpRequest req,
             [ActivityTrigger] PhotoUploadModel request,
             [Blob("photos", FileAccess.ReadWrite, Connection = Literals.StorageConnectionString)] CloudBlobContainer blobContainer,//veidojam jaunu parametru ar Blob konteinera atributu
             [CosmosDB("photos",//izejosa savienojuma atributs ar CosmosDB datubazi, pirms  elementa veidosanas tiek instaleta 
            //Microsoft.Azure.WebJobs.Extensions.CosmosDB pakotne no NuGet pakotnem sadala Dependencies,
             "metadata", ConnectionStringSetting = Literals.CosmosDBConnection,CreateIfNotExists = true)] IAsyncCollector<dynamic> items,//Cosmos DB izejosais savienojums lauj ierakstit metadatus 
             //Cosmos DB globalaja datubaze
             ILogger logger)

          //logisko maigigo izveidosana, izmantojot blob izvades saistibas
        {
            //var body = await new StreamReader(req.Body).ReadToEndAsync();//parveido visu dokumentu JSON simbolu virkne
           // var request = JsonConvert.DeserializeObject<PhotoUploadModel>(body);//parveido visu JSON virkni uz PhotoUploadModel objektu,
                                                                                //kas atrodas mape Models

            var newId = Guid.NewGuid();//pieskir ugsupladejamai datnei jaunu ID
            var blobName = $"{newId}.jpg";//pieskir ugsupladejamai datnei jaunu varu ar ID nosaukuma

            await blobContainer.CreateIfNotExistsAsync();//lai parliecinatos ka Blob konteineris eksiste izveidojam  Async metodi

            var cloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);// izmantojam metodi BlobReference
                                                                               // lai sanemt referenci uz Blob konteineri un pieskirt tam nosaukumu, saglabajam to cloudBlockBlob mainigaja


            var photoBytes = Convert.FromBase64String(request.Photo);//tiek veidots mainigais photoBytes,
                                                                     //lai augsupladet visu pieprasijuma ieklauto attelu. Tiks nolasiti baiti, izmantojot FromBase64String
                                                                     //un tiek nodoti attela ipasumi no pieprasijuma objekta.

            await cloudBlockBlob.UploadFromByteArrayAsync(photoBytes, 0, photoBytes.Length);//Tika izveidots baitu masivs,
                                                                                            //kas tiks izmantots, datnu ausupladei blob starage, izmantojot UploadFromByteArrayAsync
                                                                                            //kur tiek nodots viss baitu masivu un tiks izmantota  garuma ipasiba.

          //  var analysisResult = await analyzerService.AnalyzeAsync(photoBytes);
                                                                          
            var item = new//Veidojam CosmosDB objektu ar vajadzigiem atributiem
            {
                id = newId,
                name = request.Name,
                description = request.Description,
                tags = request.Tags,
             //   analysis = analysisResult
            };
            await items.AddAsync(item);//pievienojam AddAsync metodi kas gaida savienojumu


            logger?.LogInformation($"Successfully uploaded {newId}.jpg file and its metadata");//tiek pielika log informacija par veikmigu attela un metadatu augsupladi
                                                                                               //Blob kratuve

            // return new OkObjectResult(newId);//atgriezam OK objekta rezultatu ar  objekta ID, kas ir loti noderigs
            //Postman vai citai testesanas programmaturai vai jebkuram rikam , kam nepieciesams zinat objekta identifikatoru
            return photoBytes;
        }
    }
}
