using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
     .ConfigureServices((hostContext, services) =>
     {
         services.AddSingleton(args);

         // services.AddHostedService<GenerateCertificate.GenerateCertificate>();

     })
    .Build();




var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromMinutes(60));
await host.RunAsync(cts.Token);

Console.WriteLine("Process exited!");
Environment.Exit(0);