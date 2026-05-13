using EventBookingApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EventBookingApi.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly IMemoryCache _cache;
        private readonly List<Customer> _customers = new List<Customer>
        {
            new Customer { Id = 1, Name = "John", Email = "John@mail.com"},
            new Customer { Id = 2, Name = "Jack", Email = "Jack@mail.com"},
            new Customer { Id = 3, Name = "Jane", Email = "Jane@mail.com"},
        };

        private const string CustomersCacheKey = "customers_list";

        public CustomerService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public IEnumerable<Customer> GetAll()
        {
            if (!_cache.TryGetValue(CustomersCacheKey, out IEnumerable<Customer> cachedCustomers))
            {
                cachedCustomers = _customers.ToList();

                var cachedOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));

                _cache.Set(CustomersCacheKey, cachedCustomers, cachedOptions);
            }

            return cachedCustomers;
        }

        public Customer? GetById(int id) => _customers.FirstOrDefault(e => e.Id == id);

        public Customer Add(Customer newCustomer)
        {
            newCustomer.Id = _customers.Max(e => e.Id) + 1;
            _customers.Add(newCustomer);

            _cache.Remove(CustomersCacheKey);

            return newCustomer;
        }

        public bool Update(int id, Customer newCustomer)
        {
            var customer = _customers.FirstOrDefault(e => e.Id == id);
            if (customer == null) return false;

            customer.Name = newCustomer.Name;
            customer.Email = newCustomer.Email;
            return true;
        }

        public bool Delete(int id)
        {
            var customer = GetById(id);
            if (customer == null) return false;

            _cache.Remove(CustomersCacheKey);
            return _customers.Remove(customer);
        }
    }
}