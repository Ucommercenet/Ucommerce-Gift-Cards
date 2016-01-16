using System.Linq;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
    public class RemovePriceAndVariantsTabsFromEditProductTask : IPipelineTask<SectionGroup>
    {
        public PipelineExecutionResult Execute(SectionGroup sectionGroup)
        {
            if (sectionGroup.GetViewName() != "editproduct_aspx") return PipelineExecutionResult.Success;

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
