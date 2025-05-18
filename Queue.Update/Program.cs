using Queue.Domain.Interfaces;
using Queue.Repository.Interfaces;
using Queue.Repository.Repositories;
using Queue.Update;
using Queue.Update.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IRepositoryBase, RepositoryBase>();
builder.Services.AddSingleton<IQueueService, RabbitMQService>();
builder.Services.AddSingleton<IMessageProcessor, UpdateProcessor>();
builder.Services.AddTransient(provider => new RepositoryBase(builder.Configuration));

var host = builder.Build();
host.Run();
