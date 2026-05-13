using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;

namespace EventBookingApi.Profiles
{
    public class BookingProfile : Profile
    {
        public BookingProfile()
        {
            // Booking to ReadDto
            CreateMap<Booking, BookingReadDto>();

            // CreateDto to Booking
            CreateMap<BookingCreateDto, Booking>();
        }
    }
}