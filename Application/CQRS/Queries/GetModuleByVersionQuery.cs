using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class GetModuleByVersionQuery
    : IQuery<GetModuleResult>
{
    public GetModuleByVersionQuery(string version)
    {
        Version = version;
    }

    public string Version { get; set; }
}
