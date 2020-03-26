using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RegistrationService.Data
{
    public class RegistrationContext : DbContext, IUnitOfWork
    {
        public DbSet<Patient> Patient { get; set; }
        public DbSet<PatientAddress> PatientAddress { get; set; }

        private IDbContextTransaction _currentTransaction;

        public RegistrationContext(DbContextOptions<RegistrationContext> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public bool HasActiveTransaction => _currentTransaction != null;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entityType.Name).Property<DateTime>("CreateDate");
                modelBuilder.Entity(entityType.Name).Property<DateTime>("LastUpdateDate");
            }
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }


        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            OnBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }



        private void OnBeforeSaving()
        {
            ChangeTracker.DetectChanges();
            var timestamp = DateTime.Now;
            foreach (var entry in ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                entry.Property("LastUpdateDate").CurrentValue = timestamp;

                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreateDate").CurrentValue = timestamp;
                }
            }

        }

        
        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));
            if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await base.SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            return true;
        }

    }
}
