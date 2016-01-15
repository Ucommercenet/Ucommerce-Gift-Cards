using System.Collections.Generic;
using UCommerce.Transactions.Payments.Giftcard.Entities;

namespace UCommerce.Transactions.Payments.Giftcard.Services
{
	/// <summary>
	/// Interface for handeling Gift Card related issues.
	/// </summary>
	public interface IGiftCardService
	{
		IList<GiftCard> IssueGiftCards(IList<IssueGiftCardRequest> issueGiftCardRequest);
	}
}
