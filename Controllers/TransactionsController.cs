﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Repositories;
using WebApplication3.Repositories.Entities;

namespace WebApplication3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepository _transactionsRepository;

        public TransactionsController(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }

        [HttpGet]
        [Route("hello")]
        public ActionResult<List<TransactionEntity>> Get()
        {
            return Ok("Hello world!");
        }

        [HttpGet]
        [Route("hello/{name}")]
        public ActionResult<List<TransactionEntity>> Get(string name)
        {
            return Ok("Hello world! Welcome " + name);
        }

        [HttpPost]
        [Route("credit")]
        public async Task<IActionResult> Credit(TransactionEntity obj)
        {
            if (obj.Account <= 0)
                return BadRequest(new { error = "Account number must be greater than 0" });

            if (obj.IsDebit)
                return BadRequest(new { error = "Operation must be Credit" });

            if (obj.Value <= 0)
                return BadRequest(new { error = "Cannot credit account with value less than 0 or 0" });

            await _transactionsRepository.InsertAsync(obj);
            return Ok(new
            {
                Message = "Successfully inserted credits",
                obj.Value,
                Balance = await _transactionsRepository.BalanceAsync(obj.Account)
            });
        }

        [HttpPost]
        [Route("debit")]
        public async Task<IActionResult> Debit(TransactionEntity obj)
        {
            if (obj.Account <= 0)
                return BadRequest(new { error = "Account number must be greater than 0" });

            if (await _transactionsRepository.FindByAccountAsync(obj.Account) == null)
                return NotFound(new { error = "Account not found" });

            if (!obj.IsDebit)
                return BadRequest(new { error = "Operation must be Debit" });

            if (obj.Value <= 0)
                return BadRequest(new { error = "Cannot debit account with value less than 0 or 0" });

            if (await _transactionsRepository.BalanceAsync(obj.Account) < obj.Value)
                return BadRequest(new { error = "Balance must be greater than amount to be debited" });

            await _transactionsRepository.InsertAsync(obj);
            return Ok(new
            {
                Message = "Successfully debited credits",
                obj.Value,
                Balance = await _transactionsRepository.BalanceAsync(obj.Account)
            });
        }

        [HttpGet]
        [Route("accountextract/{account}")]
        public async Task<IActionResult> AccountExtract(int? account)
        {
            if (!account.HasValue)
                return BadRequest(new { error = "Account not provided" });

            if (account.Value <= 0)
                return BadRequest(new { error = "Account number must be greater than 0" });

            if (await _transactionsRepository.FindByAccountAsync(account.Value) == null)
                return NotFound(new { error = "Account not found" });

            var model = await _transactionsRepository.ExtractAsync(account);
            return Ok(new { data = model });
        }

        [HttpGet]
        [Route("monthlyreport/{year}/{account}")]
        public async Task<IActionResult> MonthlyReport(int? account, int? year)
        {
            if (!account.HasValue)
                return BadRequest(new { error = "Account not provided" });

            if (account.Value <= 0)
                return BadRequest(new { error = "Account number must be greater than 0" });

            if (!year.HasValue)
                return BadRequest(new { error = "Year not provided" });

            if (year.Value < 1900 || year.Value > DateTime.Now.Year)
                return BadRequest(new { error = "Year is not valid" });

            if (await _transactionsRepository.FindByAccountAsync(account.Value) == null)
                return NotFound(new { error = "Account not found" });

            var model = await _transactionsRepository.MonthlyReportAsync(account, year);
            return Ok(new { data = model });
        }
    }
}