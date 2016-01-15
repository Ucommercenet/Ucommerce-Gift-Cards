using System;
using System.Collections.Generic;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Marketing.Targets;
using UCommerce.Pipelines.Checkout;
using UCommerce.Transactions.Payments.GiftCards;

namespace UCommerce.Pipelines.Test.GiftCards
{
    public class IssueGiftCardTaskSpy : IssueGiftCardTask
    {
    	public List<IssueGiftCardRequest> IssueGiftCardRequests; 
        public IssueGiftCardTaskSpy(IGiftCardService giftCardService, IRepository<Product> productRepository, int daysGiftCardIsAvailable, Product productToReturn)
			: base(daysGiftCardIsAvailable, productRepository, giftCardService)
        {

        }

        protected override PaymentMethod GetDefaultGiftCardPaymentMethodServiceForProductCatalogGroup(ProductCatalogGroup productCatalogGroup)
        {
        	return null;
        }

        public IEnumerable<OrderLine> GetOrderLinesWithGiftCard(PurchaseOrder purchaseOrder)
        {
            return base.GetOrderLinesWithGiftCard(purchaseOrder);
        }

		public override PipelineExecutionResult Execute(PurchaseOrder subject)
		{
			var giftCardRequests = new List<IssueGiftCardRequest>();
			foreach (var orderLine in GetOrderLinesWithGiftCard(subject))
			{
				for (int i = 0; i < orderLine.Quantity; i++)
				{
					var amount = new Money(orderLine.Price + orderLine.VAT, orderLine.PurchaseOrder.BillingCurrency);
					var request = new IssueGiftCardRequest(
						amount,
						true,
						DateTime.Now.Date.AddDays(_daysGiftIsAvailableAfterPurchase),
						GetDefaultGiftCardPaymentMethodServiceForProductCatalogGroup(subject.ProductCatalogGroup),
						subject.OrderNumber);

					giftCardRequests.Add(request);
				}
			}

			_giftCardService.IssueGiftCards(null);

			IssueGiftCardRequests = giftCardRequests;
			
			return PipelineExecutionResult.Success;
		} 
	}
}