namespace Utileco.UtiQ
{
    public interface ICommand : IBaseRequest { }

    public interface ICommand<out TResonse> : IBaseRequest { }
}
