using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SpaceWeatherAPI
{
    public class Planet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Satellite> Satellites { get; set; }
    }

    public class Satellite
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Weather { get; set; }
    }

    public class SpaceWeatherDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SatelliteDto> Satellites { get; set; }
    }

    public class SatelliteDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Weather { get; set; }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class SpaceWeatherController : ControllerBase
    {
        private static List<Planet> planets = new List<Planet>
        {
            new Planet { Id = 1, Name = "Earth", Satellites = new List<Satellite> { new Satellite { Id = 1, Name = "Moon", Weather = "Clear" } } },
            new Planet { Id = 2, Name = "Mars", Satellites = new List<Satellite> { new Satellite { Id = 2, Name = "Phobos", Weather = "Cloudy" } } },
            new Planet { Id = 3, Name = "Jupiter", Satellites = new List<Satellite> { new Satellite { Id = 3, Name = "Europa", Weather = "Cloudy" } } },
            new Planet { Id = 4, Name = "Saturn", Satellites = new List<Satellite> { new Satellite { Id = 4, Name = "Titan", Weather = "Acid Rain" } } },
            new Planet { Id = 5, Name = "Neptune", Satellites = new List<Satellite> { new Satellite { Id = 5, Name = "Triton", Weather = "Icy" } } }
        };

        // GET api/v1/planets
        [HttpGet("api/v1/planets")]
        public IActionResult GetPlanets([FromQuery] int page = 1, [FromQuery] int size = 10, [FromQuery] string status = null, [FromQuery] string sort = null)
        {
            IQueryable<Planet> query = planets.AsQueryable();

            // Filtreleme
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(p => p.Name.ToLower().Contains(status.ToLower()));
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
