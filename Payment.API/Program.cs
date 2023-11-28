using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");

    });
});

var app = builder.Build();


app.Run();
