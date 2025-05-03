using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class DeleteCardCommand : ICommand<DeleteCardResult>
{
    public DeleteCardCommand()
    {
        // intentionally left blank
    }
    public string Id { get; set; }
}


public class DeleteCardResult
{
    public DeleteCardResult()
    {
        // intentionally left blank
    }

    public DeleteCardResult(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}
