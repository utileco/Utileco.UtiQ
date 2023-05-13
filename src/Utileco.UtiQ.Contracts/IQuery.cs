namespace Utileco.UtiQ.Contracts
{
    public interface IQuery : IBaseRequest { }

    public interface IQuery<out TResponse> : IBaseRequest { }
}
