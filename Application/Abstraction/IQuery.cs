namespace Application.Abstraction
{
    public interface IQuery<out TVm> : IRequest<TVm>
    {
    }
}
