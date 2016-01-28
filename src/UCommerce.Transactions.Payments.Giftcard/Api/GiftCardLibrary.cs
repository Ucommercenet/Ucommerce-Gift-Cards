using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace UCommerce.Transactions.Payments.GiftCard.Api
{
	public class GiftCardLibraryInternal
	{
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;
		private readonly TransactionLibraryInternal _transactionLibraryInternal;
		private readonly IRepository<Entities.GiftCard> _giftCardRepository;

		public GiftCardLibraryInternal(IRepository<PaymentMethod> paymentMethodRepository, TransactionLibraryInternal transactionLibraryInternal, IRepository<GiftCard.Entities.GiftCard> giftCardRepository)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_transactionLibraryInternal = transactionLibraryInternal;
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

			if (!_transactionLibraryInternal.HasBasket()) return null; //throw exception??

			var paymentRequest = new PaymentRequest(_transactionLibraryInternal.GetBasket(false).PurchaseOrder, new Payment()
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
		public static Payment UseGiftCard(string giftCardCode)
		{
			return GiftCardLibraryInternal.UseGiftCard(giftCardCode);
		}
	}
}
