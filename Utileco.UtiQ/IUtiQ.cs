namespace Utileco.UtiQ
{
    public interface IUtiQ
    {
        Task SendCommand<T>(T command);
        Task SendQuery<T, TResult>(T query);
    }
}
