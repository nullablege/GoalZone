namespace GoalZone.API.DTOs.AdminDTOs
{
    public class AddSubstitutionDto
    {
        public int MatchId { get; set; }
        public int TeamId { get; set; }
        public int PlayerOutId { get; set; }
        public int PlayerInId { get; set; }
        public int Minute { get; set; }
    }
}
