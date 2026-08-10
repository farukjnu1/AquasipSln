using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Aquasip.EF;

namespace Aquasip.Controllers
{
    public class ReviewVotesController : Controller
    {
        private readonly AquasipContext _context;

        public ReviewVotesController(AquasipContext context)
        {
            _context = context;
        }

        // GET: ReviewVotes
        public async Task<IActionResult> Index(long reviewId)
        {
            var aquasipContext = _context.ReviewVotes.Where(x=>x.ReviewId == reviewId).Include(r => r.Customer).Include(r => r.Review);
            return View(await aquasipContext.ToListAsync());
        }

        // GET: ReviewVotes/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewVote = await _context.ReviewVotes
                .Include(r => r.Customer)
                .Include(r => r.Review)
                .FirstOrDefaultAsync(m => m.VoteId == id);
            if (reviewVote == null)
            {
                return NotFound();
            }

            return View(reviewVote);
        }

        // GET: ReviewVotes/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["ReviewId"] = new SelectList(_context.Reviews, "ReviewId", "ReviewId");
            return View();
        }

        // POST: ReviewVotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoteId,ReviewId,CustomerId,IsHelpful,CreatedAt")] ReviewVote reviewVote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviewVote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reviewVote.CustomerId);
            ViewData["ReviewId"] = new SelectList(_context.Reviews, "ReviewId", "ReviewId", reviewVote.ReviewId);
            return View(reviewVote);
        }

        // GET: ReviewVotes/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewVote = await _context.ReviewVotes.FindAsync(id);
            if (reviewVote == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reviewVote.CustomerId);
            ViewData["ReviewId"] = new SelectList(_context.Reviews, "ReviewId", "ReviewId", reviewVote.ReviewId);
            return View(reviewVote);
        }

        // POST: ReviewVotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("VoteId,ReviewId,CustomerId,IsHelpful,CreatedAt")] ReviewVote reviewVote)
        {
            if (id != reviewVote.VoteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reviewVote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewVoteExists(reviewVote.VoteId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", reviewVote.CustomerId);
            ViewData["ReviewId"] = new SelectList(_context.Reviews, "ReviewId", "ReviewId", reviewVote.ReviewId);
            return View(reviewVote);
        }

        // GET: ReviewVotes/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviewVote = await _context.ReviewVotes
                .Include(r => r.Customer)
                .Include(r => r.Review)
                .FirstOrDefaultAsync(m => m.VoteId == id);
            if (reviewVote == null)
            {
                return NotFound();
            }

            return View(reviewVote);
        }

        // POST: ReviewVotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var reviewVote = await _context.ReviewVotes.FindAsync(id);
            if (reviewVote != null)
            {
                _context.ReviewVotes.Remove(reviewVote);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewVoteExists(long id)
        {
            return _context.ReviewVotes.Any(e => e.VoteId == id);
        }
    }
}
