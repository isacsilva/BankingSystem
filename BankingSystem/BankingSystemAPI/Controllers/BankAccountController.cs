using BankingSystemAPI.DTOs;
using Core;
using Core.Service;
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Bankaccount account)
        {
            var created = await _service.CreateAccount(account);
            return CreatedAtAction(nameof(GetByNumber), new { number = created.Number }, created);
        }

        [HttpGet("by-number/{number}")]
        public async Task<IActionResult> GetByNumber(string number)
        {
            var account = await _service.GetByAccountNumber(number);
            return account == null ? NotFound() : Ok(account);
        }

        [HttpGet("by-branch/{branch}")]
        public async Task<IActionResult> GetByBranch(string branch)
        {
            var accounts = await _service.GetByBranch(branch);
            return Ok(accounts);
        }

        [HttpGet("by-document/{document}")]
        public async Task<IActionResult> GetByHolderDocument(string document)
        {
            var accounts = await _service.GetByHolderDocument(document);
            return Ok(accounts);
        }

        [HttpPut("update-email/{id}")]
        public async Task<IActionResult> UpdateEmail(int id, [FromQuery] string email)
        {
            var success = await _service.UpdateEmail(id, email);
            return success ? NoContent() : NotFound();
        }

        [HttpPut("update-status/{id}")]
        public async Task<IActionResult> UpdateStatus(int id, [FromQuery] string status)
        {
            var success = await _service.UpdateStatus(id, status);
            return success ? NoContent() : NotFound();
        }

        [HttpPut("close/{id}")]
        public async Task<IActionResult> CloseAccount(int id)
        {
            var success = await _service.CloseAccount(id);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("balance/{id}")]
        public async Task<IActionResult> GetBalance(int id)
        {
            var balance = await _service.GetBalance(id);
            return balance == null ? NotFound() : Ok(balance);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferRequestDto dto)
        {
            var success = await _service.Transfer(dto.FromAccountId, dto.ToAccountNumber, dto.Amount);
            return success ? Ok() : BadRequest("Transferência inválida.");
        }

        [HttpPost("hold")]
        public async Task<IActionResult> HoldAmount([FromQuery] int id, [FromQuery] decimal amount)
        {
            var success = await _service.HoldAmount(id, amount);
            return success ? Ok() : BadRequest("Saldo insuficiente.");
        }

        [HttpPost("release")]
        public async Task<IActionResult> ReleaseAmount([FromQuery] int id, [FromQuery] decimal amount)
        {
            var success = await _service.ReleaseAmount(id, amount);
            return success ? Ok() : BadRequest("Nenhum valor bloqueado suficiente.");
        }
    }
}
