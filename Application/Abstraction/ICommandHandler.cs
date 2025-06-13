namespace Application.Abstraction
{
    public interface ICommandHandler<in TCommand, TVm> : IRequestHandler<TCommand, TVm> where TCommand : ICommand<TVm>
    {
    }
}
