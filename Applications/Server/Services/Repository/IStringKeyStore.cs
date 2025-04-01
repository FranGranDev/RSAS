namespace Application.Services.Repository
{
    public interface IStringKeyStore<T> where T : class
    {
        IQueryable<T> All { get; }

        T Get(string id);
        void Save(T entity);
        void Delete(T entity);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(string id);
        Task<T> SaveAsync(T entity);
        Task DeleteAsync(string id);
    }
} 