namespace GoalZone.API.Entities
{
    public class StandingCorrection
    {
        public int Id { get; set; }

        public int SeasonId { get; set; }
        public Season Season { get; set; } = null!;

        public int TeamId { get; set; }
        public Team Team { get; set; } = null!;

        public int StageId { get; set; }
        public int TypeId { get; set; }

        public int Value { get; set; }
        public string CalcType { get; set; } = null!; // e.g. "-"
        public bool IsActive { get; set; }
    }
}
