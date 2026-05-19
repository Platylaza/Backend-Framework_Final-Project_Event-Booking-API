namespace EventBookingApi.DTOs
{
    public class EventReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly Date { get; set; }
        public string Location  { get; set; } = string.Empty;
        public int Capacity {get; set;}
    }
}