using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UCommerce.EntitiesV2;
using UCommerce.Marketing.Targets;
using UCommerce.Transactions.Payments.Giftcard.Entities;

namespace UCommerce.Transactions.Payments.GiftCards
{
	/// <summary>
	/// Class for handeling Gift Card related issues.
	/// </summary>
	public class GiftCardService : IGiftCardService
	{
		private IVoucherCodeGenerator _voucherCodeGenerator;
		private IRepository<GiftCard> _giftCardRepository;

		public GiftCardService(IVoucherCodeGenerator voucherCodeGenerator, IRepository<GiftCard> giftCardRepository)
		{
			_voucherCodeGenerator = voucherCodeGenerator;
			_giftCardRepository = giftCardRepository;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="issueGiftCardRequestRequest"></param>
		/// <returns>List of issued giftcards.</returns>
		public virtual IList<GiftCard> IssueGiftCards(IList<IssueGiftCardRequest> issueGiftCardRequests)
		{
			var giftCards = new List<GiftCard>();
			foreach (var issueGiftCardRequest in issueGiftCardRequests)
			{
				GiftCard giftCard = new GiftCard();
				if (issueGiftCardRequest.Reference != null)
					giftCard.OrderNumber = issueGiftCardRequest.Reference;

				giftCard.PaymentMethod = issueGiftCardRequest.PaymentMethod;
				giftCard.Enabled = issueGiftCardRequest.Enabled;
				giftCard.AmountUsed = 0;
				giftCard.Amount = issueGiftCardRequest.Amount.Value;
				giftCard.ExpiresOn = issueGiftCardRequest.ExpiresOn;
				giftCard.Currency = issueGiftCardRequest.Amount.Currency;
				giftCard.Note = issueGiftCardRequest.Note;

				giftCards.Add(giftCard);
			}

			AssignUniqueCodes(giftCards, 0);

			_giftCardRepository.Save(giftCards);

			return giftCards;
		}

		#region helpers

		/// <summary>
		/// Method 
		/// </summary>
		private void AssignUniqueCodes(IEnumerable<GiftCard> giftCards, int runCount)
		{
			if (!giftCards.Any()) return;

			runCount++;
			if (runCount == 10)
			{
				foreach (var giftCard in giftCards)
				{
					giftCard.Code = Guid.NewGuid().ToString();
				}
			}

			foreach (var giftCard in giftCards)
			{
				giftCard.Code = _voucherCodeGenerator.GenerateCodes(1, 13, "GC", "").Single();
			}

			var nonUniqueGiftCards = giftCards.Where(x => _giftCardRepository.Select(y => y.Code == x.Code).Any()).ToList();

			AssignUniqueCodes(nonUniqueGiftCards, runCount);
		}

		#endregion
	}
}