namespace Application.Abstraction
{
    public interface IQueryHandler<in TQuery, TVm> : IRequestHandler<TQuery, TVm> where TQuery : IQuery<TVm>
    {
    }
}
