namespace HealthCare.Claims.DocumentsService.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult>
{
    TResult Handle(TQuery query);
}
