using Card.Common.Domain;
using Card.Domain.AggregatesModel.CardAggregate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Card.Infrastructure.Repositories;

public class ModelRepository : IModelRepository
{
    private readonly NNContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public ModelRepository(NNContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Model> AddAsync(Model order)
    {
        var result = await _context.Cards.AddAsync(order);
        
        return result.Entity;
    }

    public async Task<Model> GetByLatestAsync(string id)
    {
        var card = await _context.Cards.OrderBy(p => p.RegisteringTime).LastOrDefaultAsync();

        return card;
    }

    public async Task<Model> GetByIdAsync(string id)
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
