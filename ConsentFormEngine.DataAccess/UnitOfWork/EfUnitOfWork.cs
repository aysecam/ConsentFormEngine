using Microsoft.EntityFrameworkCore.Storage;
using ConsentFormEngine.Core.Interfaces;
using ConsentFormEngine.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsentFormEngine.DataAccess.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly BaseDbContext _context;
        private IDbContextTransaction? _transaction;

        public EfUnitOfWork(BaseDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.CommitAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
