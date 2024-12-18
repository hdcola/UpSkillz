using Stripe;
namespace UpSkillz.Services;

public class StripeService
{
    private readonly string _secretKey;

    public StripeService(IConfiguration configuration)
    {
        _secretKey = Environment.GetEnvironmentVariable("Stripe__SecretKey") ?? string.Empty;
        StripeConfiguration.ApiKey = _secretKey;
    }

    public PaymentIntent CreatePaymentIntent(long amount, string currency = "usd")
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = amount,
            Currency = currency,
            PaymentMethodTypes = new List<string> { "card" }
        };
        var service = new PaymentIntentService();
        return service.Create(options);
    }
}