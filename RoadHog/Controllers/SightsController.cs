using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DSHomework.Data;
using DSHomework.Models;

namespace DSHomework.Controllers
{
    public class SightsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SightsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Sights
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sights.ToListAsync());
        }

        // GET: Sights/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sight = await _context.Sights.SingleOrDefaultAsync(m => m.SightId == id);
            if (sight == null)
            {
                return NotFound();
            }

            return View(sight);
        }

        // GET: Sights/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Sights/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SightId,HasRestPlace,HasRestRoom,SightDescription,SightName,SightWelcomeRatio")] Sight sight)
        {
            if (ModelState.IsValid)
            {
                sight.SightId = DateTime.Now.GetHashCode().ToString();
                _context.Add(sight);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(sight);
        }

        // GET: Sights/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sight = await _context.Sights.SingleOrDefaultAsync(m => m.SightId == id);
            if (sight == null)
            {
                return NotFound();
            }
            return View(sight);
        }

        // POST: Sights/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("SightId,HasRestPlace,HasRestRoom,SightDescription,SightName,SightWelcomeRatio")] Sight sight)
        {
            if (id != sight.SightId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sight);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SightExists(sight.SightId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(sight);
        }

        // GET: Sights/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sight = await _context.Sights.SingleOrDefaultAsync(m => m.SightId == id);
            if (sight == null)
            {
                return NotFound();
            }

            return View(sight);
        }

        // POST: Sights/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sight = await _context.Sights.SingleOrDefaultAsync(m => m.SightId == id);
            _context.Sights.Remove(sight);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool SightExists(string id)
        {
            return _context.Sights.Any(e => e.SightId == id);
        }
    }
}
