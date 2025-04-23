using BankingSystemAPI.DTOs;
using Core.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _service;

        public TransactionController(ITransactionService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _service.GetById(id);
            return transaction == null ? NotFound() : Ok(transaction);
        }

        [HttpGet("by-account/{accountId}")]
        public async Task<IActionResult> GetByAccountId(int accountId, [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string type)
        {
            var transactions = await _service.GetByAccountId(accountId, from, to, type);
            return Ok(transactions);
        }

        [HttpGet("by-counterparty-document/{document}")]
        public async Task<IActionResult> GetByCounterpartyDocument(string document)
        {
            var transactions = await _service.GetByCounterpartyDocument(document);
            return Ok(transactions);
        }

        [HttpPost("credit")]
        public async Task<IActionResult> Credit([FromBody] TransactionAmountDto dto)
        {
            var result = await _service.Credit(dto.AccountId, dto.Amount);
            return result == null ? BadRequest("Crédito inválido.") : Ok(result);
        }

        [HttpPost("debit")]
        public async Task<IActionResult> Debit([FromBody] TransactionAmountDto dto)
        {
            var result = await _service.Debit(dto.AccountId, dto.Amount);
            return result == null ? BadRequest("Débito inválido.") : Ok(result);
        }
    }
}
