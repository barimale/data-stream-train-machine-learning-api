using Application.Dtos;

namespace Application.CQRS.Queries;

public class GetModuleResult
{
    public GetModuleResult()
    {
        //intentionally left blank
    }
    public GetModuleResult(ModelDto model)
    {
        Model = model;
    }

    public ModelDto Model { get; set; }
}