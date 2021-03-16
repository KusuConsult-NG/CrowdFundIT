using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AlgorandCrowdfund.Data;
using AlgorandCrowdfund.Models;
using Algorand;
using Account = Algorand.Account;
using Algorand.V2;
using Algorand.Client;
using Algorand.V2.Model;
using Microsoft.AspNetCore.Identity;

namespace AlgorandCrowdfund.Controllers
{
    public class FundersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static UserManager<ApplicationUser> _userManager;

        public FundersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Funders
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Funders.Include(f => f.RequestFunds).Include(x => x.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Funders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funders = await _context.Funders
                .Include(f => f.RequestFunds)
                .Include(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funders == null)
            {
                return NotFound();
            }

            return View(funders);
        }

        // GET: Funders/Create
        public IActionResult Create()
        {
            ViewData["RequestFundsId"] = new SelectList(_context.RequestFunds, "Id", "Id");
            return View();
        }

        // POST: Funders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,RequestFundsId,Receiver,Key")] Funders funders)
        {
            if (ModelState.IsValid)
            {
                var username = await _userManager.FindByNameAsync(User.Identity.Name);
                funders.User = username;
                funders.Key = username.Key;
                FundMethod(username.Key, funders.Receiver, funders.Amount, username.AccountAddress);
                _context.Add(funders);
                ViewBag.Success = "Successfully transfered funds";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(RequestFundsController.Index));
            }
            ViewData["RequestFundsId"] = new SelectList(_context.RequestFunds, "Id", "Id", funders.RequestFundsId);
            return View(funders);
        }
        public static void FundMethod(string key, string receiver, int amount, string senderAddr)
        {
            string ALGOD_API_ADDR = "https://testnet-algorand.api.purestake.io/ps2"; //find in algod.net
            string ALGOD_API_TOKEN = "B3SU4KcVKi94Jap2VXkK83xx38bsv95K5UZm2lab"; //find in algod.token          
            string SRC_ACCOUNT = key;
            string DEST_ADDR = receiver;
            Account src = new Account(SRC_ACCOUNT);
            AlgodApi algodApiInstance = new AlgodApi(ALGOD_API_ADDR, ALGOD_API_TOKEN);
            try
            {
                var trans = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                Console.WriteLine("Exception when calling algod#getSupply:" + e.Message);
            }

            TransactionParametersResponse transParams;
            try
            {
                transParams = algodApiInstance.TransactionParams();
            }
            catch (ApiException e)
            {
                throw new Exception("Could not get params", e);
            }
            var amountsent = Utils.AlgosToMicroalgos(amount);
            var tx = Utils.GetPaymentTransaction(src.Address, new Address(DEST_ADDR), amountsent, "pay message", transParams);
            var signedTx = src.SignTransaction(tx);

            Console.WriteLine("Signed transaction with txid: " + signedTx.transactionID);

            // send the transaction to the network
            try
            {
                var id = Utils.SubmitTransaction(algodApiInstance, signedTx);
                Console.WriteLine("Successfully sent tx with id: " + id.TxId);
                Console.WriteLine(Utils.WaitTransactionToComplete(algodApiInstance, id.TxId));
            }
            catch (ApiException e)
            {
                // This is generally expected, but should give us an informative error message.
                Console.WriteLine("Exception when calling algod#rawTransaction: " + e.Message);
            }
        }
        // GET: Funders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funders = await _context.Funders.FindAsync(id);
            if (funders == null)
            {
                return NotFound();
            }
            ViewData["RequestFundsId"] = new SelectList(_context.RequestFunds, "Id", "Id", funders.RequestFundsId);
            return View(funders);
        }

        // POST: Funders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,RequestFundsId")] Funders funders)
        {
            if (id != funders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundersExists(funders.Id))
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
            ViewData["RequestFundsId"] = new SelectList(_context.RequestFunds, "Id", "Id", funders.RequestFundsId);
            return View(funders);
        }

        // GET: Funders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funders = await _context.Funders
                .Include(f => f.RequestFunds)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funders == null)
            {
                return NotFound();
            }

            return View(funders);
        }

        // POST: Funders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var funders = await _context.Funders.FindAsync(id);
            _context.Funders.Remove(funders);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FundersExists(int id)
        {
            return _context.Funders.Any(e => e.Id == id);
        }
    }
}
