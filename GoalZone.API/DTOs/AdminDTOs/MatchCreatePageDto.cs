namespace GoalZone.API.DTOs.AdminDTOs
{
    public class MatchCreatePageDto
    {
        public AddMatchDto Form { get; set; } = new();
        public List<AdminLookupItemDto> Seasons { get; set; } = new();
        public List<AdminLookupItemDto> Teams { get; set; } = new();
        public List<AdminLookupItemDto> Statuses { get; set; } = new();
    }
}
