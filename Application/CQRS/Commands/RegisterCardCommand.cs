using BuildingBlocks.Application.CQRS;

namespace Card.Application.CQRS.Commands;

public class RegisterCardCommand : ICommand<RegisterCardResult>
{
    public RegisterCardCommand()
    {
        // intentionally left blank
    }

    public string AccountNumber { get; set; }
    public string PIN { get; set; }
    public string SerialNumber { get; set; }
    public string Id { get; set; }
}


public class RegisterCardResult
{
    public RegisterCardResult()
    {
        // intentionally left blank
    }

    public RegisterCardResult(string id)
    {
        Id = id;
    }

    public string Id { get; set; }
}
