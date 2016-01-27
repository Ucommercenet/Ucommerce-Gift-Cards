using System;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace UCommerce.Transactions.Payments.GiftCard.Api
{
	public class GiftCardLibraryInternal
	{
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;
		private readonly TransactionLibraryInternal _transactionLibraryInternal;

		public GiftCardLibraryInternal(IRepository<PaymentMethod> paymentMethodRepository, TransactionLibraryInternal transactionLibraryInternal)
		{
			_paymentMethodRepository = paymentMethodRepository;
			_transactionLibraryInternal = transactionLibraryInternal;
		}

		/// <summary>
		/// Uses a giftcard on the basket.
		/// </summary>
		/// <param name="giftCardCode"></param>
		/// <returns>A payment if the request added a usage of a giftcard.</returns>
		public Payment UseGiftCard(string giftCardCode)
		{
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

			var paymentRequest = new PaymentRequest(_transactionLibraryInternal.GetBasket(false).PurchaseOrder,null);
			paymentRequest.AdditionalProperties.Add(Constants.GiftCardCodePaymentPropertyName,giftCardCode);

			return (paymentMethodService as IPaymentFactory).CreatePayment(paymentRequest);
		}

		public OrderLine BuyGiftCard(int quantity, string sku, string variantSku, Decimal? customPrice, bool addToExistingLine = true, int? catalogId = null, bool executeBasketPipeline = true)
		{
            var catalogName = catalogId.HasValue ? catalogId.ToString() : string.Empty;

			var orderLine = _transactionLibraryInternal.AddToBasket(catalogName, quantity, sku, variantSku, addToExistingLine, false); //wait with execute basket pipeline untill we're ready to do so.
			if (customPrice != null)
			{
				orderLine.Price = customPrice.Value;
			}

			if (executeBasketPipeline)
				_transactionLibraryInternal.ExecuteBasketPipeline();

			return orderLine;
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

		public static OrderLine BuyGiftCard(int quantity, string sku, string variantSku, Decimal? amount, bool addToExistingLine = true, int? catalogId = null)
		{
			return GiftCardLibraryInternal.BuyGiftCard(quantity, sku, variantSku, amount, addToExistingLine, catalogId);
		}
	}
}
