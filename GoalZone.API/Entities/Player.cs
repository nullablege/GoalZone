namespace GoalZone.API.Entities
{
    public class Player
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;
    }
}
