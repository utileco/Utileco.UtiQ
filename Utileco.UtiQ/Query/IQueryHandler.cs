namespace Utileco.UtiQ.Query
{
    public interface IQueryHandler<in T, TResult>
    {
        Task<TResult> Handle(T query, CancellationToken cancellationToken);
    }
}
