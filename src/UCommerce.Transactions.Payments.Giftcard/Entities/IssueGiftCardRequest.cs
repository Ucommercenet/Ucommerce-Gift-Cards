using System;
using Ucommerce;
using Ucommerce.EntitiesV2;

namespace UCommerce.Transactions.Payments.GiftCard.Entities
{
	/// <summary>
	/// Request to issue a gift card.
	/// </summary>
	public class IssueGiftCardRequest
	{
		public PaymentMethod PaymentMethod { get; set; }
		public bool Enabled { get; set; }
		public string Reference { get; set; }
		public Money Amount { get; set; }
		public DateTime ExpiresOn { get; set; }
		public string Note { get; set; }
		
		public Currency Currency { get; set; }

		public IssueGiftCardRequest(Money balance, Currency currency, bool enabled, DateTime expiresOn, PaymentMethod paymentMethod)
		{
			ExpiresOn = expiresOn;
			Enabled = enabled;
			PaymentMethod = paymentMethod;
			Amount = balance;
		}

		public IssueGiftCardRequest(Money balance, Currency currency, bool enabled, DateTime expiresOn, PaymentMethod paymentMethod, string reference)
		{
			ExpiresOn = expiresOn;
			Enabled = enabled;
			PaymentMethod = paymentMethod;
			Amount = balance;
			Reference = reference;
		}
	}
}
