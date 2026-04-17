namespace GoalZone.API.Entities
{
    public class MatchSubstitution
    {
        public int Id { get; set; }

        public int MatchId { get; set; }
        public Match Match { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int PlayerOutId { get; set; }
        public Player PlayerOut { get; set; } = null!;

        public int PlayerInId { get; set; }
        public Player PlayerIn { get; set; } = null!;

        public int Minute { get; set; }
    }
}
