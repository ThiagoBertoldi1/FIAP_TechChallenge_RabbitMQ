using Queue.Domain.Interfaces;
using Queue.Insert;
using Queue.Insert.Services;
using Queue.Repository.Interfaces;
using Queue.Repository.Repositories;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IRepositoryBase, RepositoryBase>();
builder.Services.AddSingleton<IQueueService, RabbitMQService>();
builder.Services.AddSingleton<IMessageProcessor, InsertProcessor>();
builder.Services.AddTransient(provider => new RepositoryBase(builder.Configuration));

var host = builder.Build();
host.Run();
