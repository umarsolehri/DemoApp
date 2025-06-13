namespace Application.Abstraction
{
    public interface ICommand<out TVm> : IRequest<TVm>
    {
    }
}
