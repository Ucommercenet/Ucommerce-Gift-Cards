using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
    public class RemoveVatAndVatRateFromOrderlinesContainingGiftCardTask : IPipelineTask<PurchaseOrder>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly bool _removeVatOnGiftCards;

        public RemoveVatAndVatRateFromOrderlinesContainingGiftCardTask(IRepository<Product> productRepository, bool removeVatOnGiftCards = true)
        {
            _productRepository = productRepository;
            _removeVatOnGiftCards = removeVatOnGiftCards;
        }

        public PipelineExecutionResult Execute(PurchaseOrder subject)
        {
            if (!_removeVatOnGiftCards) return PipelineExecutionResult.Success;

            foreach (var orderLine in subject.OrderLines)
            {
                var giftCard =
                    _productRepository.Select(x => x.Sku == orderLine.Sku)
                        .FirstOrDefault(x => x.ProductDefinition.Name == Constants.GiftCardProductDefinition);

                if (giftCard == null) continue;

                orderLine.VAT = 0;
                orderLine.VATRate = 0;
            }
 
            return PipelineExecutionResult.Success;
        }
    }
}
