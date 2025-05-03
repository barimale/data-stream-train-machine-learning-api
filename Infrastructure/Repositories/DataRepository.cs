using Card.Common.Domain;
using Card.Domain.AggregatesModel.CardAggregate;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Card.Infrastructure.Repositories;

public class DataRepository : IDataRepository
{
    private readonly CardContext _context;

    public IUnitOfWork UnitOfWork => _context;

    public DataRepository(CardContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.AggregatesModel.CardAggregate.Data> AddAsync(Domain.AggregatesModel.CardAggregate.Data order)
    {
        var result = await _context.Datas.AddAsync(order);
        
        return result.Entity;
    }

    // WIP
    public async Task<Domain.AggregatesModel.CardAggregate.Data> GetByLatestAsync(string id)
    {
        var card = await _context.Datas.OrderBy(p => p.IngestionTime).LastOrDefaultAsync();

        return card;
    }

    public async Task<Domain.AggregatesModel.CardAggregate.Data> GetByIdAsync(string id)
    {
        var card = await _context.Datas.FirstOrDefaultAsync(p => p.Id == id);

        return card;
    }

    public async Task<string> Delete(string id)
    {
        var toBeDeleted = await _context.Datas.FirstOrDefaultAsync(p => p.Id == id);
        var result = _context.Datas.Remove(toBeDeleted);

        return result.Entity.Id;
    }
}
