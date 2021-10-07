using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Photos;
using Photos.AnalyzerService;
using Photos.AnalyzerService.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]

namespace Photos
{
    public class Startup : FunctionsStartup//lai tiktu realizeta sakaribu implementesana
                                           //tiek veidota klase, kas tiek mantota no funkciju palaisanas bazes klases,
                                           //kuras atrodas Microsoft function extentions pakotne
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<IAnalyzerService, ComputerVisionAnalyzerService>();
        }
    }
}