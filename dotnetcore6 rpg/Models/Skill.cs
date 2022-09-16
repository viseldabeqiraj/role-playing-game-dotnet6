namespace dotnetcore6_rpg.Models
{
    public class Skill
    {
        public string Name { get; set; } =String.Empty;
        public int Id { get; set; }
        public int Damage { get; set; }

        public List<Character> Characters { get; set; }
    }
}
