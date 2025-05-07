using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class RegisterDataCommand : ICommand<RegisterDataResult>
{
    public RegisterDataCommand()
    {
        // intentionally left blank
    }

    public string Xs { get; set; }
    public string Ys { get; set; }
    public byte[] Model { get; set; }
}


public class RegisterDataResult
{
    public RegisterDataResult()
    {
        // intentionally left blank
    }

    public RegisterDataResult(string id)
        :this()
    {
        Id = id;
    }

    public string Id { get; set; }
}
