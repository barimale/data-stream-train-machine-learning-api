using Application.Dtos;

namespace Application.CQRS.Queries;

public class GetPiecesResult
{
    public GetPiecesResult()
    {
        //intentionally left blank
    }
    public GetPiecesResult(DataDto[] model)
    {
        Models = model;
    }

    public DataDto[] Models { get; set; }
}