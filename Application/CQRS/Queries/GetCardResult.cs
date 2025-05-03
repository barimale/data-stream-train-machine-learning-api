using Card.Application.Dtos;

namespace Card.Application.CQRS.Queries;

public class GetCardResult
{
    public GetCardResult()
    {
        //intentionally left blank
    }
    public GetCardResult(CardDto card)
    {
        Card = card;
    }

    public CardDto Card { get; set; }
}