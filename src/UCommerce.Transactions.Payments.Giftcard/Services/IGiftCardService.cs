using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCommerce.EntitiesV2;
using UCommerce.Transactions.Payments.Giftcard.Entities;

namespace UCommerce.Transactions.Payments.GiftCards
{
	/// <summary>
	/// Interface for handeling Gift Card related issues.
	/// </summary>
	public interface IGiftCardService
	{
		IList<GiftCard> IssueGiftCards(IList<IssueGiftCardRequest> issueGiftCardRequest);
	}
}
