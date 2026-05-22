using FS.Application.OutboxLogic.DTOs;
using FS.Application.OutboxLogic.Interfaces;

namespace FS.Application.OutboxLogic.Implementations;

public class OutboxService : IOutboxService
{
    public async Task AddAsync(CreateOutboxEventData createData, CancellationToken cancellationToken)
    {
        
    }
}