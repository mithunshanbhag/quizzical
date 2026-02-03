var host = Host
    .CreateApplicationBuilder(args)
    .ConfigureApp()
    .ConfigureServices()
    .Build();

var engine = host.Services.GetRequiredService<SinglePlayerConsoleQuizEngine>();
await engine.RunAsync(CancellationToken.None);