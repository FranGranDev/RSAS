namespace Application.Services.Repository
{
    public interface IStore<T> where T : class
    {
        IQueryable<T> All { get; }

        T Get(int id);
        void Save(T entity);
        void Delete(T entity);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> SaveAsync(T entity);
        Task DeleteAsync(int id);
    }
}