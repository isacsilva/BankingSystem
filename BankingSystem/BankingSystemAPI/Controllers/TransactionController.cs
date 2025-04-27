using BankingSystemAPI.DTOs;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var transaction = await _service.GetById(id);
            return transaction == null ? NotFound() : Ok(transaction);
        }

        [Authorize]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterTransactions(    [FromQuery] int? Id,
                                                                [FromQuery] int? accountId,
                                                                [FromQuery] DateTime? from,
                                                                [FromQuery] DateTime? to,
                                                                [FromQuery] string? type,
                                                                [FromQuery] string? counterpartyDocument)
        {
            var result = await _service.FilterTransactions(Id, accountId, from, to, type, counterpartyDocument);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("by-counterparty-document/{document}")]
        public async Task<IActionResult> GetByCounterpartyDocument(string document)
        {
            var transactions = await _service.GetByCounterpartyDocument(document);
            return Ok(transactions);
        }

        [Authorize]
        [HttpPost("credit")]
        public async Task<IActionResult> Credit([FromBody] TransactionAmountDto dto)
        {
            var result = await _service.Credit(dto.AccountId, dto.Amount);
            return result == null ? BadRequest("Crédito inválido.") : Ok(result);
        }

        [Authorize]
        [HttpPost("debit")]
        public async Task<IActionResult> Debit([FromBody] TransactionAmountDto dto)
        {
            var result = await _service.Debit(dto.AccountId, dto.Amount);
            return result == null ? BadRequest("Débito inválido.") : Ok(result);
        }
    }
}
