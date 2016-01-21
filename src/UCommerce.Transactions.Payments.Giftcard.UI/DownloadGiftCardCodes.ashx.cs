﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Security;

namespace UCommerce.Transactions.Payments.GiftCard.UI
{
	/// <summary>
	/// Summary description for DownloadGiftCardCodes
	/// </summary>
	public class DownloadGiftCardCodes : IHttpHandler
	{
		private readonly IRepository<Entities.GiftCard> _giftCardRepository;
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;

		public DownloadGiftCardCodes()
		{
			_giftCardRepository = ObjectFactory.Instance.Resolve<IRepository<Entities.GiftCard>>();
			_paymentMethodRepository = ObjectFactory.Instance.Resolve<IRepository<PaymentMethod>>();
		}

		public void ProcessRequest(HttpContext context)
		{
			var authenticationService = ObjectFactory.Instance.Resolve<IAuthenticationService>();
			if (!authenticationService.IsAuthenticated()) throw new SecurityException("User not authenticated. Please log in to download gift cards.");

			var paymentMethod = _paymentMethodRepository.Get(Int32.Parse(context.Request.QueryString["Id"]));

			var giftCards = _giftCardRepository.Select(x => x.PaymentMethod.PaymentMethodId == paymentMethod.Id).ToList();

			string fileName = string.Format("GiftCards-for-{0}", paymentMethod.Name);

			var stringOfGiftCards = new StringBuilder();

			if (giftCards.Count > 0)
			{
				GetGiftCardString(giftCards, stringOfGiftCards);
			}
			else
			{
				stringOfGiftCards.Append(string.Format("No gift cards found for payment method: {0}", paymentMethod.Name));
			}

			byte[] buffer;
			using(var memoryStream = new System.IO.MemoryStream())
			{
				buffer = Encoding.Default.GetBytes(stringOfGiftCards.ToString());
				memoryStream.Write(buffer,0,buffer.Length);
				context.Response.Clear();
				context.Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName + ".txt");
				context.Response.ContentType = "text/plain"; //This is MIME type             
				memoryStream.WriteTo(context.Response.OutputStream);

			}
			context.Response.End();
		}

		private void GetGiftCardString(List<Entities.GiftCard> giftCards, StringBuilder stringOfGiftCards)
		{
			var openGiftCards = giftCards.Where(x => x.AmountUsed < x.Amount && x.ExpiresOn > DateTime.Now && x.Enabled);
			var amountUsed = openGiftCards.Sum(x => x.AmountUsed);
			var amountUnUsed = openGiftCards.Sum(x => x.Amount);

			var totalGiftCards = string.Format("Total number of gift cards: {0}\r\n", giftCards.Count().ToString());
			var totalAmount = string.Format("Total amount of gift cards: {0}\r\n", giftCards.Sum(x => x.Amount).ToString());
			var giftcardsInUse = string.Format("Gift cards in use: {0}\r\n",
				giftCards.Where(x => x.AmountUsed > 0 && x.AmountUsed < x.Amount && x.Enabled).Count().ToString());
			var expiredGiftCards = string.Format("Gift cards expired: {0}\r\n",
				giftCards.Where(x => x.ExpiresOn < DateTime.Now && x.AmountUsed < x.Amount).Count().ToString());
			var giftCardsClosed = string.Format("Gift cards closed: {0}\r\n",
				giftCards.Where(x => x.Amount == x.AmountUsed).Count().ToString());
			var giftCardsAmountUsed = string.Format("Amount used: {0}\r\n", amountUsed);
			var giftCardsAmountUnUsed = string.Format("Amount unused: {0}\r\n\r\n", (amountUnUsed - amountUsed).ToString());

			stringOfGiftCards.Append("Gift card summary:\r\n\r\n");
			stringOfGiftCards.Append(totalGiftCards);
			stringOfGiftCards.Append(totalAmount);
			stringOfGiftCards.Append(giftcardsInUse);
			stringOfGiftCards.Append(expiredGiftCards);
			stringOfGiftCards.Append(giftCardsClosed);
			stringOfGiftCards.Append(giftCardsAmountUsed);
			stringOfGiftCards.Append(giftCardsAmountUnUsed);

			stringOfGiftCards.Append("GiftCardCode Amount Used Expires Currency \r\n");
			foreach (var giftCard in giftCards)
			{
				stringOfGiftCards.Append(string.Format("{0} , {1} , {2} , {3} , {4} \r\n",
					giftCard.Code, giftCard.Amount, giftCard.AmountUsed, giftCard.ExpiresOn,
					giftCard.Currency.Name));
			}
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}