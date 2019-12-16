using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IntegrationEventLog
{
    public class IntegrationEventLogContext : DbContext
    {       
        public IntegrationEventLogContext(DbContextOptions<IntegrationEventLogContext> options) 
            : base(options) { }

        public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }
    }
}