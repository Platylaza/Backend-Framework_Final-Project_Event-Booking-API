using AutoMapper;
using EventBookingApi.Models;
using EventBookingApi.DTOs;

namespace EventBookingApi.Profiles
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            // Event to ReadDto
            CreateMap<Event, EventReadDto>();

            // CreateDto to Event
            CreateMap<EventCreateDto, Event>();
        }
    }
}