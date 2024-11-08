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
            var innmeldinger = _context.Innmeldinger
                                        .Include(i => i.Bruker) // Hent relatert Bruker
                                        .Include(i => i.Kategori) // Hent relatert Kategori
                                        .ToList();
            return View(innmeldinger);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Innmelding updatedInnmelding)
        {
            if (id != updatedInnmelding.InnmeldingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(updatedInnmelding);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(updatedInnmelding);
        }

        // Slett en innmelding
        public IActionResult Delete(int id)
        {
            var innmelding = _context.Innmeldinger
                                     //.Include(i => i.Bruker)
                                     .Include(i => i.Kategori)
                                     .FirstOrDefault(i => i.InnmeldingId == id);
            if (innmelding == null)
            {
                return NotFound();
            }
            return View(innmelding);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var innmelding = _context.Innmeldinger.Find(id);
            if (innmelding != null)
            {
                _context.Innmeldinger.Remove(innmelding);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }


    }

}
