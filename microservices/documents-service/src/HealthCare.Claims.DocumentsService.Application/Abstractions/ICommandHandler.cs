namespace HealthCare.Claims.DocumentsService.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResult>
{
    Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}
