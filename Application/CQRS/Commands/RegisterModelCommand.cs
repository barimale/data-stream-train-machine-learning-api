using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class RegisterModelCommand : ICommand<RegisterModelResult>
{
    public RegisterModelCommand()
    {
        // intentionally left blank
    }

    public string Ys { get; set; }
    public string Xs { get; set; }
    public byte[] Model { get; set; }
    public string Version { get; set; }
}


public class RegisterModelResult
{
    public RegisterModelResult()
    {
        // intentionally left blank
    }

    public RegisterModelResult(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}
