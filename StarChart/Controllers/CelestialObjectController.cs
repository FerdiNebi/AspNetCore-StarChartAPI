using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        public CelestialObjectController(ApplicationDbContext context)
        {
            this._context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var celestialObject = this._context.CelestialObjects.Find(id);
            if (celestialObject == null)
            {
                return NotFound();
            }

            var satelites = this._context.CelestialObjects.Where(c => c.OrbitedObjectId == celestialObject.Id);
            celestialObject.Satellites = satelites.ToList();

            return Ok(celestialObject);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var celestialObjects = this._context.CelestialObjects.Where(c => c.Name == name).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            celestialObjects.ForEach(c =>
            c.Satellites = this._context.CelestialObjects
                                        .Where(s => s.OrbitedObjectId == c.Id)
                                        .ToList());

            return Ok(celestialObjects);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allCelestialObjects = this._context.CelestialObjects.ToList();
            allCelestialObjects.ForEach(c => 
            c.Satellites = this._context.CelestialObjects
                                        .Where(s => s.OrbitedObjectId == c.Id)
                                        .ToList());

            return Ok(allCelestialObjects);
        }

        [HttpPost]
        public IActionResult Create([FromBody]CelestialObject celestialObject)
        {
            this._context.CelestialObjects.Add(celestialObject);
            this._context.SaveChanges();

            return CreatedAtRoute("GetById", new { id = celestialObject.Id }, celestialObject);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var persistedCelestialObject = this._context.CelestialObjects.Find(id);
            if (persistedCelestialObject == null)
            {
                return NotFound();
            }

            persistedCelestialObject.Name = celestialObject.Name;
            persistedCelestialObject.OrbitalPeriod = celestialObject.OrbitalPeriod;
            persistedCelestialObject.OrbitedObjectId = celestialObject.OrbitedObjectId;

            //this._context.CelestialObjects.Update(persistedCelestialObject);
            this._context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var persistedCelestialObject = this._context.CelestialObjects.Find(id);
            if (persistedCelestialObject == null)
            {
                return NotFound();
            }

            persistedCelestialObject.Name = name;
            this._context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var celestialObjects = this._context.CelestialObjects.Where(c => c.Id == id || c.OrbitedObjectId == id).ToList();
            if (!celestialObjects.Any())
            {
                return NotFound();
            }

            this._context.CelestialObjects.RemoveRange(celestialObjects);
            this._context.SaveChanges();

            return NoContent();
        }

        private readonly ApplicationDbContext _context;
    }
}
