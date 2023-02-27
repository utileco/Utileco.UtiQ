namespace Utileco.UtiQ.Command
{
    public interface ICommandHandler<in T>
    {
        Task Handle(T command, CancellationToken cancellationToken);
    }
}
