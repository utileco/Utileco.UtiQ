namespace Utileco.UtiQ
{
    public interface IQuery : IBaseRequest { }

    public interface IQuery<out TResponse> : IBaseRequest { }
}
