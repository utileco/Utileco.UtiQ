using Utileco.UtiQ.Contracts;

namespace Utileco.UtiQ.Query
{
    public interface IQueryHandler<in TRequest, TResult>
        where TRequest : IQuery<TResult>
    {
        /// <summary>
        /// Handles request.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> Handle(TRequest query, CancellationToken cancellationToken);
    }
}
