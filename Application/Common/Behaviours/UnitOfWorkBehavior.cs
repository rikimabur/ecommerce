using Application.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviours
{
    public interface IUnitOfWork
    {
        Task<IDbContextTransaction> BeginTransaction();
        Task Commit();
        void Dispose();
    }
    public class UnitOfWork(ApplicationDbContext context) : IDisposable, IUnitOfWork
    {

        public async Task<IDbContextTransaction> BeginTransaction()
        {
            return await context.Database.BeginTransactionAsync();
        }

        public async Task Commit()
        {
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }

    }
    public interface ITransactionCommand<out TResponse> : IRequest<TResponse>
    {
    }
    internal class UnitOfWorkBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionCommand<TResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<UnitOfWorkBehavior<TRequest, TResponse>> _logger;

        public UnitOfWorkBehavior(ILogger<UnitOfWorkBehavior<TRequest, TResponse>> logger, IUnitOfWork uow)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            using var connection = await _uow.BeginTransaction();
            TResponse? response = default;
            try
            {
                response = await next();
                await _uow.Commit();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured on transaction.");
                await connection.RollbackAsync();
            }
            finally
            {
                connection.Dispose();
            }

            return response!;
        }
    }
}
