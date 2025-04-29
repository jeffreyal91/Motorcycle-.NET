using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class RabbitMQConsumerService : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public RabbitMQConsumerService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var motorcycleService = scope.ServiceProvider.GetRequiredService<IMotorcycleService>();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        //stop logic
        return Task.CompletedTask;
    }
}
