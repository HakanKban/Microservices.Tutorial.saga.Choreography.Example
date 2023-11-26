using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");

    });
});

var app = builder.Build();


app.Run();
