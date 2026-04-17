namespace GoalZone.API.DTOs.AdminDTOs
{
    public class EventCreatePageDto
    {
        public AddEventDto Form { get; set; } = new();
        public AddSubstitutionDto SubstitutionForm { get; set; } = new();
        public List<AdminMatchOptionDto> Matches { get; set; } = new();
        public List<AdminLookupItemDto> Teams { get; set; } = new();
        public List<AdminPlayerOptionDto> Players { get; set; } = new();
        public List<AdminLookupItemDto> EventTypes { get; set; } = new();
    }
}
