using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using WeatherApiProject.Dtos;
using WeatherApiProject.Entities;

namespace WeatherApiProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    /*
     * API V1
     * 
     */
    public class WeatherForecastController : ControllerBase
    {
        private static List<Planet> planets = new List<Planet>
        {
            new Planet { Id = 1, Name = "Earth",CreatedDate = DateTime.Now.AddDays(-5), Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Moon", Weather = "Clear" } } },
            new Planet { Id = 2, Name = "Mars",CreatedDate = DateTime.Now.AddDays(-4), Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Phobos", Weather = "Cloudy" },  new Satellite { Id = 2, Name = "Deimos", Weather = "Cloudy" } } },
            new Planet { Id = 3, Name = "Jupiter",CreatedDate = DateTime.Now.AddDays(-3), Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Europa", Weather = "Windy" } } },
            new Planet { Id = 4, Name = "Saturn",CreatedDate = DateTime.Now.AddDays(-2), Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Titan", Weather = "Cloudy" } } },
            new Planet { Id = 5, Name = "Neptune",CreatedDate = DateTime.Now.AddDays(-1), Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Triton", Weather = "Clear" } } }
        };

        // GET api/v1/planets
        [HttpGet("api/v1/planets")]
        public IActionResult GetPlanets([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string status = null, [FromQuery] string sort = null)
        {
            IQueryable<Planet> query = planets.AsQueryable();

            // Filtreleme
            if (!string.IsNullOrEmpty(status))
            {
                var active = status.Equals("active");
                query = query.Where(p => p.IsActive == active);
            }

            // Sýralama
            if (!string.IsNullOrEmpty(sort))
            {
                var sortParams = sort.Split(',');
                string sortBy = sortParams[0];
                string sortOrder = sortParams.Length > 1 ? sortParams[1] : "asc";

                switch (sortBy.ToLower())
                {
                    case "name":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                        break;
                    case "id":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.Id) : query.OrderByDescending(p => p.Id);
                        break;
                    case "created_date":
                        query = sortOrder.ToLower() == "asc" ? query.OrderBy(p => p.CreatedDate) : query.OrderByDescending(p => p.CreatedDate);
                        break;
                        // Diðer sýralama seçenekleri eklenebilir.
                }
            }

            // Sayfalama
            var paginatedResult = query.Skip((page - 1) * size).Take(size).ToList();

            // Dönüþ verilerini DTO'ya çevir
            var resultDto = paginatedResult.Select(p => new SpaceWeatherDto
            {
                Id = p.Id,
                Name = p.Name,
                CreatedDate = p.CreatedDate,
                Satellites = p.Satellites.Select(s => new SatelliteDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Weather = s.Weather
                }).ToList()
            }).ToList();

            return Ok(resultDto);
        }

        // GET api/v1/planets/{id}
        [HttpGet("api/v1/planets/{id}")]
        public IActionResult GetPlanetById(int id)
        {
            var planet = planets.FirstOrDefault(p => p.Id == id);

            if (planet == null)
            {
                return NotFound(); // 404 Not Found
            }

            var planetDto = new SpaceWeatherDto
            {
                Id = planet.Id,
                Name = planet.Name,
                Satellites = planet.Satellites.Select(s => new SatelliteDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Weather = s.Weather
                }).ToList()
            };

            return Ok(planetDto); // 200 OK
        }

        // POST api/v1/planets
        [HttpPost("api/v1/planets")]
        public IActionResult CreatePlanet([FromBody] Planet newPlanet)
        {
            planets.Add(newPlanet);

            return CreatedAtAction(nameof(GetPlanetById), new { id = newPlanet.Id }, newPlanet); // 201 Created
        }

        // PUT api/v1/planets/{id}
        [HttpPut("api/v1/planets/{id}")]
        public IActionResult UpdatePlanet(int id, [FromBody] Planet updatedPlanet)
        {
            var existingPlanet = planets.FirstOrDefault(p => p.Id == id);

            if (existingPlanet == null)
            {
                return NotFound(); // 404 Not Found
            }

            existingPlanet.Name = updatedPlanet.Name;
            existingPlanet.Satellites = updatedPlanet.Satellites;

            return Ok(existingPlanet); // 200 OK
        }

        // DELETE api/v1/planets/{id}
        [HttpDelete("api/v1/planets/{id}")]
        public IActionResult DeletePlanet(int id)
        {
            var planetToRemove = planets.FirstOrDefault(p => p.Id == id);

            if (planetToRemove == null)
            {
                return NotFound(); // 404 Not Found
            }

            planets.Remove(planetToRemove);

            return NoContent(); // 204 No Content
        }
    }
}
