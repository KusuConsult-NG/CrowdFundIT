using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlgorandCrowdfund.Data;
using AlgorandCrowdfund.Models;
using Microsoft.AspNetCore.Identity;

namespace AlgorandCrowdfund.Controllers
{
    public class RequestFundsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static UserManager<ApplicationUser> _userManager;

        public RequestFundsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: RequestFunds
        public async Task<IActionResult> Index()
        {
            var username = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.UserAddr = username.AccountAddress;
            ViewBag.Key = username.Key;
            return View(await _context.RequestFunds.Include(x => x.User).Include(x => x.Funders).ToListAsync());
        }

        // GET: RequestFunds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var mymodel = new ViewModel();
            mymodel.RequestFundsList = RequestFundsList();
            mymodel.FundersList = FundersList();
            mymodel.RequestFunds = await _context.RequestFunds
                .Include(x => x.Funders)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            mymodel.Funders = await _context.Funders
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mymodel.RequestFunds == null)
            {
                return NotFound();
            }

            return View(mymodel);
        }
        private List<RequestFunds> RequestFundsList()
        {
            List<RequestFunds> list = new List<RequestFunds>();
            list = _context.RequestFunds.Include(x => x.User).ToList();
            return list;
        }
        private List<Funders> FundersList()
        {
            List<Funders> list = new List<Funders>();
            list = _context.Funders.Include(x => x.User).ToList();
            return list;
        }
        public class ViewModel
        {
            public RequestFunds RequestFunds { get; set; }
            public Funders Funders { get; set; }
            public IEnumerable<RequestFunds> RequestFundsList { get; set; }
            public IEnumerable<Funders> FundersList { get; set; }
        }
        // GET: RequestFunds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RequestFunds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FundTitle,FundDescription,AmountNeeded,AmountRaised,Created")] RequestFunds requestFunds)
        {
            if (ModelState.IsValid)
            {
                var username = await _userManager.FindByNameAsync(User.Identity.Name);
                requestFunds.User = username;
                _context.Add(requestFunds);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(requestFunds);
        }

        // GET: RequestFunds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFunds = await _context.RequestFunds.FindAsync(id);
            if (requestFunds == null)
            {
                return NotFound();
            }
            return View(requestFunds);
        }

        // POST: RequestFunds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FundTitle,FundDescription,AmountNeeded,AmountRaised,Created,ValidTill")] RequestFunds requestFunds)
        {
            if (id != requestFunds.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requestFunds);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestFundsExists(requestFunds.Id))
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
            return View(requestFunds);
        }

        // GET: RequestFunds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requestFunds = await _context.RequestFunds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (requestFunds == null)
            {
                return NotFound();
            }

            return View(requestFunds);
        }

        // POST: RequestFunds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requestFunds = await _context.RequestFunds.FindAsync(id);
            _context.RequestFunds.Remove(requestFunds);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestFundsExists(int id)
        {
            return _context.RequestFunds.Any(e => e.Id == id);
        }
    }
}
