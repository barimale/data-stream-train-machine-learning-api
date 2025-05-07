using Card.Application.Dtos;

namespace Card.Application.CQRS.Queries;

public class GetPiecesResult
{
    public GetPiecesResult()
    {
        //intentionally left blank
    }
    public GetPiecesResult(ModelDto[] model)
    {
        Models = model;
    }

    public ModelDto[] Models { get; set; }
}