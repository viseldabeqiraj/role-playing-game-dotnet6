namespace dotnetcore6_rpg.Dtos.Fights
{
    public class AttackResultDto
    {
        public string Attacker { get; set; } = String.Empty;
        public string Opponent { get; set; } = String.Empty;
        public int AttackerHP { get; set; }
        public int OpponentHP { get; set; }
        public int Damage { get; set; }
    }
}
