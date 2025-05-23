using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class UpdateIsAppliedPiece : ICommand<RegisterDataIsAppliedResult>
{
    public UpdateIsAppliedPiece(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}


public class RegisterDataIsAppliedResult
{
    public RegisterDataIsAppliedResult()
    {
        // intentionally left blank
    }

    public RegisterDataIsAppliedResult(int id)
        :this()
    {
        Id = id;
    }

    public int Id { get; set; }
}
