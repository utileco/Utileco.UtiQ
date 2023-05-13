namespace Utileco.UtiQ.Contracts
{
    public interface ICommand : IBaseRequest { }

    public interface ICommand<out TResonse> : IBaseRequest { }
}
