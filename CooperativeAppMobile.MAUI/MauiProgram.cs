global using Microsoft.Extensions.Logging;
global using CooperativeAppMobile.MAUI.Helpers;
global using Syncfusion.Maui.Core.Hosting;
global using CommunityToolkit.Maui;
global using CooperativeAppMobile.MAUI.Views;
global using System.Net;
global using CooperativeAppMobile.MAUI.Models;
global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Linq;
global using System.Net.Http.Headers;
global using System.Net.Http.Json;
global using System.Text;
global using System.Threading.Tasks;
global using CooperativeAppMobile.MAUI.Services;
global using System.Text.RegularExpressions;


namespace CooperativeAppMobile.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
               .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<APIService>();
            builder.Services.AddSingleton(Connectivity.Current);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
