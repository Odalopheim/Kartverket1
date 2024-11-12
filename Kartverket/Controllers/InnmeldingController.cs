using Kartverket.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Kartverket.Controllers
{
    public class InnmeldingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InnmeldingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hent alle innmeldinger og vis dem
        public IActionResult Index()
        {
            var GeoChanges = _context.GeoChange
                                        .Include(i => i.Bruker) // Hent relatert Bruker
                                        .Include(i => i.Kategori) // Hent relatert Kategori
                                        .ToList();
            return View(GeoChanges);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, GeoChange updatedGeoChanges)
        {
            if (id != updatedGeoChanges.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(updatedGeoChanges);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedGeoChanges);
        }

        // Slett en innmelding
        public IActionResult Delete(int id)
        {
            var geoChanges = _context.GeoChange
                                     //.Include(i => i.Bruker)
                                     .Include(i => i.Kategori)
                                     .FirstOrDefault(i => i.Id == id);
            if (geoChanges == null)
            {
                return NotFound();
            }
            return View(geoChanges);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var geoChange = _context.GeoChange.Find(id);
            if (geoChange != null)
            {
                _context.GeoChange.Remove(geoChange);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


    }

}
