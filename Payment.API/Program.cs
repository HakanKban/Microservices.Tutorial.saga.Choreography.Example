using MassTransit;

var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();
builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((context, _conf) =>
    {
        _conf.Host("localhost");

    });
});

app.Run();
