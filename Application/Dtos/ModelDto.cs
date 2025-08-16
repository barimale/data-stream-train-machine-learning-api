using TypeGen.Core.TypeAnnotations;

namespace Application.Dtos;

public class ModelDto
{
    public ModelDto()
    {
        // intentionally left blank
    }

    public DateTime RegisteringTime { get; set; }
    public string Id { get; set; }
    public byte[] ModelAsBytes { get; set; }   
    public string Version { get; set; }
}
