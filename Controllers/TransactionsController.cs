using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Repositories.Entities;
using WebApplication3.Repositories;
using System.Diagnostics;
using WebApplication3.Models.ViewModels;
using WebApplication3.Repositories.Exceptions;

namespace WebApplication3.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsController(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Credit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Credit(TransactionEntity obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(obj);
                }

                if (obj.Account <= 0)
                {
                    throw new TransactionException("Account number must be greater than 0");
                }

                if (obj.IsDebit)
                {
                    throw new TransactionException("Operation must be Credit");
                }

                if (obj.Value <= 0)
                {
                    throw new TransactionException("Cannot credit account with value less than 0 or 0");
                }

                await _transactionsRepository.InsertAsync(obj);
                return RedirectToAction(nameof(Success), new { Message = "Successfully inserted credits", obj.Value, Balance = await _transactionsRepository.BalanceAsync(obj.Account) });
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { e.Message });
            }
        }

        public IActionResult Debit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Debit(TransactionEntity obj)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            try
            {
                if (obj.Account <= 0)
                {
                    throw new TransactionException("Account number must be greater than 0");
                }

                if (await _transactionsRepository.FindByAccountAsync(obj.Account) == null)
                {
                    throw new NotFoundException("Account not found");
                }

                if (!obj.IsDebit)
                {
                    throw new TransactionException("Operation must be Debit");
                }

                if (obj.Value <= 0)
                {
                    throw new TransactionException("Cannot debit account with value less than 0 or 0");
                }

                if (await _transactionsRepository.BalanceAsync(obj.Account) < obj.Value)
                {
                    throw new TransactionException("Balance must be greater than amount to be debited");
                }

                await _transactionsRepository.InsertAsync(obj);
                return RedirectToAction(nameof(Success), new { Message = "Successfully debited credits", obj.Value, Balance = await _transactionsRepository.BalanceAsync(obj.Account) });
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { e.Message });
            }

        }

        public IActionResult IndexAccountExtract()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccountExtract(int? account)
        {
            try
            {
                if (!account.HasValue)
                    return RedirectToAction(nameof(Error), new { message = "Account not provided" });

                if (account.Value <= 0)
                    return RedirectToAction(nameof(Error), new { message = "Account number must be greater than 0" });

                if (await _transactionsRepository.FindByAccountAsync(account.Value) == null)
                    throw new NotFoundException("Account not found");

                ViewData["account"] = account;
                var model = await _transactionsRepository.ExtractAsync(account);
                return View(model);
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { e.Message });
            }
        }

        public IActionResult IndexMonthlyReport()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MonthlyReport(int? account, int? year)
        {
            try
            {
                if (!account.HasValue)
                    return RedirectToAction(nameof(Error), new { message = "Account not provided" });

                if (account.Value <= 0)
                    return RedirectToAction(nameof(Error), new { message = "Account number must be greater than 0" });

                if (!year.HasValue)
                    return RedirectToAction(nameof(Error), new { message = "Year not provided" });

                if (year.Value < 1900 || year.Value > DateTime.Now.Year)
                    return RedirectToAction(nameof(Error), new { message = "Year is not valid" });

                if (await _transactionsRepository.FindByAccountAsync(account.Value) == null)
                    throw new NotFoundException("Account not found");

                ViewData["account"] = account;
                ViewData["year"] = year;
                var model = await _transactionsRepository.MonthlyReportAsync(account, year);
                return View(model);
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { e.Message });
            }
        }

        public IActionResult Success(string message, double value, double balance)
        {
            var viewModel = new SuccessViewModel
            {
                Message = message,
                Value = value,
                Balance = balance
            };

            return View(viewModel);
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };

            return View(viewModel);
        }
    }
}