using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Presentation.UI;
using Ucommerce.Presentation.Web;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
    public class RemovePriceAndVariantsTabsFromEditProductTask : IPipelineTask<SectionGroup>
    {
        private readonly IRepository<Product> _productRepository;

        public RemovePriceAndVariantsTabsFromEditProductTask(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public PipelineExecutionResult Execute(SectionGroup sectionGroup)
        {
            if (sectionGroup.GetViewName() != "editproduct_aspx") return PipelineExecutionResult.Success;

            var product = _productRepository.Get(QueryString.Common.Id);

            if (product == null || product.ProductDefinition.Name != Constants.GiftCardProductDefinition) { 
                return PipelineExecutionResult.Success;
            }

            var tabsToRemove = sectionGroup.Sections.Where(x => x.OriginalName == UCommerce.Constants.UI.Sections.Stores.Product.Pricing || x.OriginalName == UCommerce.Constants.UI.Sections.Stores.Product.Variants).ToList();

            foreach (var tab in tabsToRemove)
            {
                tab.Visible = false;
                sectionGroup.Sections.Remove(tab);
            }

            return PipelineExecutionResult.Success;
        }
    }
}
