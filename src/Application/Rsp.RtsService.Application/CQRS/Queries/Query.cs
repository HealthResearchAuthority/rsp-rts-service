using MediatR;
using Rsp.RtsService.Application.DTOS.Responses;

namespace Rsp.RtsService.Application.CQRS.Queries;

public class Query(int id) : IRequest<QueryResponse>
{
    public int Id { get; } = id;
}