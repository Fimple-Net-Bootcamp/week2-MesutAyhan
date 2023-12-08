namespace WeatherApiProject.Entities
{
    public class Planet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Satellite> Satellites { get; set; }
        public bool IsActive { get; set; }
    }
}
