using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;

namespace EventBookingApi.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            // Customer to ReadDto
            CreateMap<Customer, CustomerReadDto>();

            // CreateDto to Customer
            CreateMap<CustomerCreateDto, Customer>();
        }
    }
}