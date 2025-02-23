using DeliveryApp.Core.Application.UseCases.Commands.AssignOrders;
using MediatR;
using Quartz;

namespace DeliveryApp.Api.Adapters.BackgroundJobs;

[DisallowConcurrentExecution]
public class AssignOrdersJob(IMediator mediator) : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        return mediator.Send(new AssignOrdersCommand());
    }
}