using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
    public class EditProductGiftCardTabTask : IPipelineTask<SectionGroup>
    {
        private readonly IRepository<Product> _productRepository;

        public EditProductGiftCardTabTask(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public PipelineExecutionResult Execute(SectionGroup sectionGroup)
        {
            if (sectionGroup.GetViewName() != "editproduct_aspx") return PipelineExecutionResult.Success;

            var product = _productRepository.Get(QueryString.Common.Id);

            if (product.ProductDefinition.Name == Constants.GiftCardProductDefinition)
            {
                var section = BuildSection(sectionGroup);
                sectionGroup.Sections.Insert(1, section);
                sectionGroup.Controls.Add(section);
            }

            return PipelineExecutionResult.Success;
        }

        private Section BuildSection(SectionGroup sectionGroup)
        {
            var section = new Section
            {
                Name = "Gift Card Price",
                ID = sectionGroup.CreateUniqueControlId()
            };

            var control = sectionGroup.View.LoadControl("/Apps/UCommerce.GiftCards/EditGiftCardPrices.ascx");

            section.AddControl(control);
            return section;
        }
    }
}
