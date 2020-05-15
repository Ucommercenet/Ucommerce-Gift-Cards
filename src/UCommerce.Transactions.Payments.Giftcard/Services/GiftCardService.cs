using System;
using System.Collections.Generic;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Marketing.Targets;
using Ucommerce.Security;
using Ucommerce.Transactions.Payments.GiftCard.Entities;

namespace UCommerce.Transactions.Payments.GiftCard.Services
{
	/// <summary>
	/// Class for handeling Gift Card related issues.
	/// </summary>
	public class GiftCardService : IGiftCardService
	{
		private IVoucherCodeGenerator _voucherCodeGenerator;
		private IRepository<Entities.GiftCard> _giftCardRepository;
		private readonly IUserService _userService;
		private readonly ICurrentUserNameService _currentUserNameService;

		public GiftCardService(IVoucherCodeGenerator voucherCodeGenerator, IRepository<Entities.GiftCard> giftCardRepository, ICurrentUserNameService currentUserNameService)
		{
			_voucherCodeGenerator = voucherCodeGenerator;
			_giftCardRepository = giftCardRepository;
			_currentUserNameService = currentUserNameService;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="issueGiftCardRequestRequest"></param>
		/// <returns>List of issued giftcards.</returns>
		public virtual IList<Entities.GiftCard> IssueGiftCards(IList<IssueGiftCardRequest> issueGiftCardRequests)
		{
			var giftCards = new List<Entities.GiftCard>();
			foreach (var issueGiftCardRequest in issueGiftCardRequests)
			{
				Entities.GiftCard giftCard = new Entities.GiftCard();
				if (issueGiftCardRequest.Reference != null)
					giftCard.OrderNumber = issueGiftCardRequest.Reference;

				giftCard.PaymentMethod = issueGiftCardRequest.PaymentMethod;
				giftCard.Enabled = issueGiftCardRequest.Enabled;
				giftCard.AmountUsed = 0;
				giftCard.Amount = issueGiftCardRequest.Amount.Value;
				giftCard.CreatedOn = DateTime.UtcNow;
				giftCard.ModifiedOn = DateTime.UtcNow;
				giftCard.CreatedBy = _currentUserNameService.CurrentUserName;
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
		private void AssignUniqueCodes(IEnumerable<Entities.GiftCard> giftCards, int runCount)
		{
			var NonUniqueGiftCards = new List<Entities.GiftCard>();

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

				if(_giftCardRepository.Select(x => x.Code == giftCard.Code).SingleOrDefault() != null)
					NonUniqueGiftCards.Add(giftCard);
			}

			AssignUniqueCodes(NonUniqueGiftCards, runCount);
		}

		#endregion
	}
}