namespace UpSkillz.Models;

public class PaymentModel
{
    public string Name { get; set; }
    public string Email { get; set; }
    public long Amount { get; set; } // Amount in cents
    public string PaymentIntentId { get; set; }
}