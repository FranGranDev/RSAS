using Application.Data;
using Application.Model.Orders;
using Application.Model.Stocks;
using Application.ViewModel.Catalog;
using Microsoft.EntityFrameworkCore;
using static Application.Model.Orders.Order;

namespace Application.Services.Repository
{
    public class EFOrderStore : IOrderStore
    {
        private readonly AppDbContext _context;

        public EFOrderStore(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> All => _context.Orders;

        public Order Get(int id)
        {
            return _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .FirstOrDefault(o => o.Id == id);
        }

        public void Save(Order entity)
        {
            if (entity.Id == 0)
            {
                _context.Orders.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }

        public void Delete(Order entity)
        {
            _context.Orders.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> SaveAsync(Order entity)
        {
            if (entity.Id == 0)
            {
                _context.Orders.Add(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);
                
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Order>> GetByClientIdAsync(int clientId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .Where(o => o.UserId == clientId.ToString())
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByEmployeeIdAsync(int employeeId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .Where(o => o.UserId == employeeId.ToString())
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(States status)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Stock)
                .Include(o => o.Delivery)
                .Include(o => o.Products)
                .Where(o => o.State == status)
                .ToListAsync();
        }
        

        public async Task<Order> CreateOrderAsync(Order order)
        {
            order.OrderDate = DateTime.UtcNow;
            order.ChangeDate = DateTime.UtcNow;
            order.State = States.New;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task ExecuteOrderAsync(Order order, Stock stock)
        {
            if (order.State != States.New)
            {
                throw new InvalidOperationException("Можно выполнять только новые заказы");
            }

            order.State = States.InProcess;
            order.ChangeDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task CompleteOrderAsync(Order order)
        {
            if (order.State != States.InProcess)
            {
                throw new InvalidOperationException("Можно завершать только заказы в процессе");
            }

            order.State = States.Completed;
            order.ChangeDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}