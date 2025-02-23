using DeliveryApp.Core.Application.UseCases.Commands.MoveCouriers;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class MoveCouriersJob(IMediator mediator) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return mediator.Send(new MoveCouriersCommand());
    }
}