﻿using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Queries;

public class GetLatestQuery
    : IQuery<GetModuleResult>
{
    public GetLatestQuery(string version)
    {
        Version = version;
    }

    public string Version { get; set; }
}
