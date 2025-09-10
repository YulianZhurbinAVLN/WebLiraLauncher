using System.Diagnostics;
using Newtonsoft.Json;

namespace WebLiraLauncher;

public class FreeLicenseWaiter
{
    public void Wait()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")
                        ?? "http://localhost:7136");
        var app = builder.Build();

        LiraLauncher.LaunchCompleted += () => app.StopAsync();

        app.Map("/notify", (HttpContext context) =>
        {
            Task.Run(() =>
            {
                Thread.Sleep(500);
                LiraLauncher.Launch();
            });

            context.Response.StatusCode = 200;
            return "Лира запускается";
        });

        app.MapPost("/queue", async (HttpContext context) =>
        {
            Console.WriteLine("Запрос на /queue");
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            var queueDto = JsonConvert.DeserializeObject<QueueDto>(body);
            
            string queuePosition = queueDto?.Position.ToString() ?? "не известно";
            Console.WriteLine("Место в очереди: " + queuePosition);

            context.Response.StatusCode = 200;
        });

        Task.Run(() =>
        {
            Thread.Sleep(500);
            Console.SetWindowSize(50, 10);
            //Console.Clear();
            Console.WriteLine("Нет свободных лицензий" + Environment.NewLine + 
                "Постановка в очередь на получение лицензии" +
                Environment.NewLine + "Ожидание своей очереди");
        });

        app.Run();

#if DEBUG
        Console.WriteLine("Server's stopped");
#endif
    }

    public class QueueDto
    {
        public int Position { get; set; }
    };
}
