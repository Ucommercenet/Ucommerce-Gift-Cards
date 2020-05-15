﻿using System;
using System.Linq;
using Ucommerce.Api;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure;
using Ucommerce.Transactions.Payments;

namespace UCommerce.Transactions.Payments.GiftCard.Api
{
	public class GiftCardLibraryInternal
	{
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;
		private readonly ITransactionLibrary _transactionLibrary;
		private readonly IRepository<Entities.GiftCard> _giftCardRepository;

		public GiftCardLibraryInternal(IRepository<PaymentMethod> paymentMethodRepository, Ucommerce.Api.ITransactionLibrary transactionLibrary, IRepository<GiftCard.Entities.GiftCard> giftCardRepository)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_transactionLibrary = transactionLibrary;
			_giftCardRepository = giftCardRepository;
		}

		/// <summary>
		/// Uses a giftcard on the basket.
		/// </summary>
		/// <param name="giftCardCode"></param>
		/// <returns>A payment if the request added a usage of a giftcard.</returns>
		public Payment UseGiftCard(string giftCardCode)
		{
			if (string.IsNullOrEmpty(giftCardCode)) 
				return null;

			if (!_giftCardRepository.Select(x => x.Code == giftCardCode).Any())
				return null;
			
			var paymentMethod = _paymentMethodRepository.SingleOrDefault(x => x.Name == Constants.GiftCardPaymentMethodName);
			
			if (paymentMethod == null)
			{
				throw new InvalidOperationException(string.Format("You cannot use gift cards without the gift card payment method. Please add a paymentmethod with name: '{0}' that uses the service: '{1}'", Constants.GiftCardPaymentMethodName, "Gift Card"));	
			}

			var paymentMethodService = paymentMethod.GetPaymentMethodService();
			if (paymentMethodService == null || (paymentMethodService as GiftCardPaymentMethodService == null))
			{
				throw new InvalidOperationException(string.Format("Payment method with name '{0}' needs to use the service 'Gift Card'", Constants.GiftCardPaymentMethodName));	
			}

			if (!_transactionLibrary.HasBasket()) return null; //throw exception??

			var paymentRequest = new PaymentRequest(_transactionLibrary.GetBasket(false), new Payment()
			{
				PaymentMethod = paymentMethod,
				Amount = 0,
				
			});

			paymentRequest.AdditionalProperties.Add(Constants.GiftCardCodePaymentPropertyName, giftCardCode);

			return (paymentMethodService as IPaymentFactory).CreatePayment(paymentRequest);
		}
	}

	public static class GiftCardLibrary
	{
		private static GiftCardLibraryInternal GiftCardLibraryInternal
		{
			get { return ObjectFactory.Instance.Resolve<GiftCardLibraryInternal>(); }
		}

		/// <summary>
		/// Uses a giftcard on the basket.
		/// </summary>
		/// <param name="giftCardCode"></param>
		/// <returns>A payment if the request added a usage of a giftcard.</returns>
		public static  Payment UseGiftCard(string giftCardCode)
		{
			return GiftCardLibraryInternal.UseGiftCard(giftCardCode);
		}
	}
}
