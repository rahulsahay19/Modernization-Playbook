namespace HealthCare.Claims.DocumentsService.Application.Abstractions;

public interface ICommandHandler<in TCommand, TResult>
{
    TResult Handle(TCommand command);
}
