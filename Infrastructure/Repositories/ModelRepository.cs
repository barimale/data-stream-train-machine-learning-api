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
        var result = await _context.Cards.OrderBy(p => p.RegisteringTime).LastOrDefaultAsync();

        return result;
    }

    public async Task<double> GetYearsOldInMinutesAsync()
    {
        var result = await GetByLatestAsync(string.Empty);
        DateTimeOffset from = new DateTimeOffset(result.RegisteringTime);
        DateTimeOffset now = DateTimeOffset.Now;

        return (now - from).TotalMinutes;
    }
}
