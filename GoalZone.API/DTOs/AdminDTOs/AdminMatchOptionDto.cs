namespace GoalZone.API.DTOs.AdminDTOs
{
    public class AdminMatchOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}
