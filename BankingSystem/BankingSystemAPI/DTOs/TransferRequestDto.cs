namespace BankingSystemAPI.DTOs
{
    public class TransferRequestDto
    {
        public int FromAccountId { get; set; }
        public string ToAccountNumber { get; set; }
        public decimal Amount { get; set; }
    }
}
