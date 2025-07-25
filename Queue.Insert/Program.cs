using Queue.Insert;
using Queue.Repository.Interfaces;
using Queue.Repository.Repositories;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IRepositoryBase, RepositoryBase>();
builder.Services.AddTransient(provider => new RepositoryBase(builder.Configuration));

var host = builder.Build();
host.Run();
