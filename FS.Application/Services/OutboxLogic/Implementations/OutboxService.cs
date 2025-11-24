using FS.Application.DTOs.OutboxDto;
using FS.Application.Services.OutboxLogic.Interfaces;
using FS.Core.Entities;

namespace FS.Application.Services.OutboxLogic.Implementations;

public class OutboxService : IOutboxService
{
    public async Task AddAsync(CreateOutboxEventData createData, CancellationToken cancellationToken)
    {
        
    }
}