using System.Collections.Generic;
using Ucommerce.Transactions.Payments.GiftCard.Entities;

namespace UCommerce.Transactions.Payments.GiftCard.Services
{
	/// <summary>
	/// Interface for handeling Gift Card related issues.
	/// </summary>
	public interface IGiftCardService
	{
		IList<Entities.GiftCard> IssueGiftCards(IList<IssueGiftCardRequest> issueGiftCardRequest);
	}
}
