using EventBookingApi.Models;

namespace EventBookingApi.Services
{
    public interface ICustomerService
    {
        IEnumerable<Customer> GetAll();
        Customer? GetById(int id);
        Customer Add(Customer newCustomer);
        bool Update(int id, Customer newCustomer);
        bool Delete(int id);
    }
}