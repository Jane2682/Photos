using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Photos.Models;

namespace Photos
{
    public static class PhotosOrchestrator//Orchestratora funkcijas implementesana, kas pieprasis attelu analizatora un attelu glabasanas funkcijas
    {
        [FunctionName("PhotosOrchestrator_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            var body = await req.Content.ReadAsStringAsync();

            var request = JsonConvert.DeserializeObject<PhotoUploadModel>(body);

            // funcijas ieeja dati naks no pieprasitiem datiem.
            string instanceId = await starter.StartNewAsync("PhotosOrchestrator", request);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("PhotosOrchestrator")]
        public static async Task<dynamic> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var model = context.GetInput<PhotoUploadModel>();
            var photoBytes = await context.CallActivityAsync<byte[]>("PhotosStorage", model);//si funcija atgriez baitu massivu, kas tiks ieliks massiva
            var analysis = await context.CallActivityAsync<dynamic>("PhotosAnalyzer", photoBytes.ToList());// analizes funkcija , kurai tiks padoti attelu baitu dati , ka saraksts
            return analysis;
        }



    }
}