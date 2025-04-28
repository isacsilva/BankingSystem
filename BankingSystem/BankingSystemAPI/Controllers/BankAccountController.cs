using BankingSystemAPI.DTOs;
using Core;
using Core.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _service;

        public BankAccountController(IBankAccountService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bankaccount account)
        {
            var created = await _service.CreateAccount(account);
            return CreatedAtAction(nameof(GetByNumber), new { number = created.Number }, created);
        }

        [Authorize]
        [HttpGet("by-number/{number}")]
        public async Task<IActionResult> GetByNumber(string number)
        {
            var account = await _service.GetByAccountNumber(number);
            return account == null ? NotFound() : Ok(account);
        }

        [Authorize]
        [HttpGet("by-branch/{branch}")]
        public async Task<IActionResult> GetByBranch(string branch)
        {
            var accounts = await _service.GetByBranch(branch);
            return Ok(accounts);
        }

        [Authorize]
        [HttpGet("by-document/{document}")]
        public async Task<IActionResult> GetByHolderDocument(string document)
        {
            var accounts = await _service.GetByHolderDocument(document);
            return Ok(accounts);
        }

        [Authorize]
        [HttpPut("update-email/{id}")]
        public async Task<IActionResult> UpdateEmail(int id, [FromQuery] string email)
        {
            var success = await _service.UpdateEmail(id, email);
            return success ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var success = await _service.UpdateStatus(id, status);
            return success ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpPut("close/{id}")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var success = await _service.CloseAccount(id);
            return success ? NoContent() : NotFound();
        }

        [Authorize]
        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var balance = await _service.GetBalance(id);
            return balance == null ? NotFound() : Ok(balance);
        }

        [Authorize]
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            var success = await _service.Transfer(dto.FromAccountId, dto.ToAccountNumber, dto.Amount);
            return success ? Ok() : BadRequest("Transferência inválida.");
        }

        [Authorize]
        [HttpPost("hold")]
        public async Task<IActionResult> HoldAmount([FromQuery] int id, [FromQuery] decimal amount)
        {
            var success = await _service.HoldAmount(id, amount);
            return success ? Ok() : BadRequest("Saldo insuficiente.");
        }

        [Authorize]
        [HttpPost("release")]
        public async Task<IActionResult> ReleaseAmount([FromQuery] int id, [FromQuery] decimal amount)
        {
            var success = await _service.ReleaseAmount(id, amount);
            return success ? Ok() : BadRequest("Nenhum valor bloqueado suficiente.");
        }

        [Authorize]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterAccounts([FromQuery] string? number, [FromQuery] string? branch, [FromQuery] string? document)
        {
            var result = await _service.FilterAccounts(number, branch, document);
            return Ok(result);
        }
    }
}
