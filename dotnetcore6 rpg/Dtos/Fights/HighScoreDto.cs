namespace dotnetcore6_rpg.Dtos.Fights
{
    public class HighScoreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public int Fights { get; set; } 
        public int Victories { get; set; } 
        public int Defeats { get; set; } 
    }
}
