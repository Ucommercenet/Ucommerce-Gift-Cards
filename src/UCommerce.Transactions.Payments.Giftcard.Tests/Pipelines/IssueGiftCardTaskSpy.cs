using System;
using System.Collections.Generic;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Pipelines;
using UCommerce.Transactions.Payments.Giftcard.Services;

namespace UCommerce.Transactions.Payments.Giftcard.Tests.Pipelines
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