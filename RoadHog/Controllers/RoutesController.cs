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
    public class RoutesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoutesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Routes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Routes.Include(r => r.EndAt).Include(r => r.StartAt);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Routes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.SingleOrDefaultAsync(m => m.RouteId == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // GET: Routes/Create
        public IActionResult Create()
        {
            ViewData["EndSightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            ViewData["StartSightId"] = new SelectList(_context.Sights, "SightId", "SightName");
            return View();
        }

        // POST: Routes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteId,EndSightId,Length,StartSightId")] Route route)
        {
            if (ModelState.IsValid)
            {
                route.RouteId = DateTime.Now.GetHashCode().ToString();
                var Inverseroute = new Route();
                Inverseroute.EndSightId = route.StartSightId;
                Inverseroute.StartSightId = route.EndSightId;
                Inverseroute.Length = route.Length;
                Inverseroute.RouteId = (DateTime.Now.GetHashCode() + 1).ToString();

                _context.Add(route);
                _context.Add(Inverseroute);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["EndSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.EndSightId);
            ViewData["StartSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.StartSightId);
            return View(route);
        }

        // GET: Routes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.SingleOrDefaultAsync(m => m.RouteId == id);
            if (route == null)
            {
                return NotFound();
            }
            ViewData["EndSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.EndSightId);
            ViewData["StartSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.StartSightId);
            return View(route);
        }

        // POST: Routes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RouteId,EndSightId,Length,StartSightId")] Route route)
        {
            if (id != route.RouteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(route);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteExists(route.RouteId))
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
            ViewData["EndSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.EndSightId);
            ViewData["StartSightId"] = new SelectList(_context.Sights, "SightId", "SightId", route.StartSightId);
            return View(route);
        }

        // GET: Routes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var route = await _context.Routes.SingleOrDefaultAsync(m => m.RouteId == id);
            if (route == null)
            {
                return NotFound();
            }

            return View(route);
        }

        // POST: Routes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var route = await _context.Routes.SingleOrDefaultAsync(m => m.RouteId == id);
            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool RouteExists(string id)
        {
            return _context.Routes.Any(e => e.RouteId == id);
        }
    }
}
