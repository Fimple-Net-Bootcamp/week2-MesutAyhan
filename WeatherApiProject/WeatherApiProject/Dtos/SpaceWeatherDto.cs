namespace WeatherApiProject.Dtos
{
    public class SpaceWeatherDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SatelliteDto> Satellites { get; set; }
    }
}
