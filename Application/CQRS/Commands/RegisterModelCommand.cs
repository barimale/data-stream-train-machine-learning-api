using Card.Common.Application.CQRS;

namespace Application.CQRS.Commands;

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

    public RegisterModelResult(int id)
    {
        Id = id;
    }

    public int Id { get; set; }
}
