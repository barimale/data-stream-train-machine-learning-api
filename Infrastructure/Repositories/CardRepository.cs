using Card.Common.Domain;
using Card.Domain.AggregatesModel.CardAggregate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Card.Infrastructure.Repositories;

public class CardRepository : ICardRepository
{
    private readonly CardContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public CardRepository(CardContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.AggregatesModel.CardAggregate.Model> AddAsync(Domain.AggregatesModel.CardAggregate.Model order)
    {
        var result = await _context.Cards.AddAsync(order);
        
        return result.Entity;
    }

    // WIP
    public async Task<Domain.AggregatesModel.CardAggregate.Model> GetByLatestAsync(string id)
    {
        var card = await _context.Cards.OrderByDescending(p => p.RegisteringTime).LastOrDefaultAsync();

        return card;
    }

    public async Task<Domain.AggregatesModel.CardAggregate.Model> GetByIdAsync(string id)
    {
        var card = await _context.Cards.FirstOrDefaultAsync(p => p.Id == id);

        return card;
    }

    public async Task<string> Delete(string id)
    {
        var toBeDeleted = await _context.Cards.FirstOrDefaultAsync(p => p.Id == id);
        var result = _context.Cards.Remove(toBeDeleted);

        return result.Entity.Id;
    }
}
