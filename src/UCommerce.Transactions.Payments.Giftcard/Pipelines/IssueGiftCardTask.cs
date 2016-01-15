using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Services;

namespace UCommerce.Transactions.Payments.Giftcard.Pipelines
{
    /// <summary>
    /// Issues a <see cref="GiftCard"/> 
    /// in case an orderLine contains a product of definition: GiftCard.  
    /// </summary>
    public class IssueGiftCardTask : IPipelineTask<PurchaseOrder>
	{
		protected readonly int _daysGiftIsAvailableAfterPurchase;
		protected readonly IRepository<Product> _productRepository;
		protected readonly IGiftCardService _giftCardService;

		public IssueGiftCardTask(int daysGiftIsAvailableAfterPurchase, IRepository<Product> productRepository, IGiftCardService giftCardService)
		{
			_daysGiftIsAvailableAfterPurchase = daysGiftIsAvailableAfterPurchase;
			_productRepository = productRepository;
			_giftCardService = giftCardService;
		}

		/// <summary>
		/// Executes the CreateGiftCardTask pipeline task.
		/// </summary>
		/// <param name="subject">The PurchaseOrder</param>
		/// <returns></returns>
		public virtual PipelineExecutionResult Execute(PurchaseOrder subject)
		{
			var giftCardRequests = new List<IssueGiftCardRequest>();
			foreach (var orderLine in GetOrderLinesWithGiftCard(subject))
			{
				for (int i = 0; i < orderLine.Quantity; i++)
				{
					var amount = new Money(orderLine.Price + orderLine.VAT, subject.BillingCurrency);
					var request = new IssueGiftCardRequest(
						balance: amount,
						enabled: true,
						expiresOn: DateTime.Now.Date.AddDays(_daysGiftIsAvailableAfterPurchase),
						paymentMethod: GetDefaultGiftCardPaymentMethodServiceForProductCatalogGroup(subject.ProductCatalogGroup),
						reference: subject.OrderNumber);

					request.Note = "Created for order";

					giftCardRequests.Add(request);
				}
			}

			_giftCardService.IssueGiftCards(giftCardRequests);

			return PipelineExecutionResult.Success;
		}

		/// <summary>
		/// Checks if purchaseOrder contains a bought product of type, giftcard. If so, returns the specific orderLines 
		/// </summary>
		/// <param name="purchaseOrder">The PurchaseOrder to check.</param>
		/// <returns>Orderlines with giftcard.</returns>
		protected virtual IEnumerable<OrderLine> GetOrderLinesWithGiftCard(PurchaseOrder purchaseOrder)
		{
			var query = from orderLine in purchaseOrder.OrderLines
						join product in _productRepository.Select() on
							new { orderLine.Sku, orderLine.VariantSku } equals
							new { product.Sku, product.VariantSku }
						where product.ProductDefinition.Name == Constants.GiftCardProductDefinition
						select orderLine;

			return query.ToList();
		}

		/// <summary>
		/// Gets the default implementation of GiftCardPaymentMethodService configured on a store. 
		/// </summary>
		/// <param name="productCatalogGroup">The productcataloggroup on which a purchaseOrder have been made.</param>
		/// <returns>default implementation of GiftCardPaymentMethodService</returns>
		protected virtual PaymentMethod GetDefaultGiftCardPaymentMethodServiceForProductCatalogGroup(ProductCatalogGroup productCatalogGroup)
		{
			var paymentMethods = productCatalogGroup.GetAvailablePaymentMethods();

			var paymentMethod = paymentMethods.FirstOrDefault(x => x.GetPaymentMethodService() is GiftCardPaymentMethodService);

			if (paymentMethod != null) return paymentMethod;

			throw new ConfigurationErrorsException(string.Format(
				@"Unable to find configured GiftCardPaymentMethod for store: {0}. 
                Please configure it by adding a payment method for: {0}, 
                using the GiftCardPaymentMethodService provided.", productCatalogGroup.Name));
		}
	}
}
