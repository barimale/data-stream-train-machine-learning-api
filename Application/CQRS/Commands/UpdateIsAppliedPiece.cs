using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class UpdateIsAppliedPiece : ICommand<RegisterDataIsAppliedResult>
{
    public UpdateIsAppliedPiece(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}


public class RegisterDataIsAppliedResult
{
    public RegisterDataIsAppliedResult()
    {
        // intentionally left blank
    }

    public RegisterDataIsAppliedResult(string id)
        :this()
    {
        Id = id;
    }

    public string Id { get; set; }
}
