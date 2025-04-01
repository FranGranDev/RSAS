using System.Linq.Expressions;

namespace Server.Services.Repository;

public interface IRepository<TEntity, TKey> where TEntity : class
{
    // Получение всех сущностей
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    // Получение сущности по ключу
    Task<TEntity?> GetByIdAsync(TKey id);
    
    // Получение сущностей по условию
    Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
    
    // Добавление сущности
    Task<TEntity> AddAsync(TEntity entity);
    
    // Обновление сущности
    Task UpdateAsync(TEntity entity);
    
    // Удаление сущности по ключу
    Task DeleteAsync(TKey id);
    
    // Удаление сущности
    Task DeleteAsync(TEntity entity);
    
    // Проверка существования сущности
    Task<bool> ExistsAsync(TKey id);
    
    // Сохранение изменений
    Task SaveChangesAsync();
} 