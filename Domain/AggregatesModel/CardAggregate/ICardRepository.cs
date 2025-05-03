using Card.Common.Domain;

namespace Card.Domain.AggregatesModel.CardAggregate
{
    public interface ICardRepository : IRepository<Card>
    {
        Task<Card> AddAsync(Card card);
        Task<string> Delete(string id);
        Task<Card> GetByAccountNumberAsync(string id);
        Task<Card> GetByIdAsync(string id);
        Task<Card> GetBySerialNumberAsync(string id);
    }
}