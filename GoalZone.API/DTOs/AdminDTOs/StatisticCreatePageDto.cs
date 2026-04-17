namespace GoalZone.API.DTOs.AdminDTOs
{
    public class StatisticCreatePageDto
    {
        public AddStatisticsDto Form { get; set; } = new();
        public List<AdminMatchOptionDto> Matches { get; set; } = new();
    }
}
