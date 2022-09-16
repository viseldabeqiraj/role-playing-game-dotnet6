namespace dotnetcore6_rpg.Models
{
    public class Weapon
    {
        public string Name { get; set; } =String.Empty;
        public int Id { get; set; }
        public int Damage { get; set; }
        public Character Character { get; set; }

        public int CharacterId { get; set; }
    }
}
