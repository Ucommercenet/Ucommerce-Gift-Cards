using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
    public class EditProductGiftCardTabTask : IPipelineTask<SectionGroup>
    {
        private readonly IRepository<Product> _productRepository;
	    private readonly IResourceManager _resourceManager;

	    public EditProductGiftCardTabTask(IRepository<Product> productRepository, IResourceManager resourceManager)
        {
	        _productRepository = productRepository;
	        _resourceManager = resourceManager;
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
				Name = _resourceManager.GetLocalizedText("EditGiftCardPrices", "GiftCardTabName"),
                ID = sectionGroup.CreateUniqueControlId()
            };

            var control = sectionGroup.View.LoadControl("/Apps/UCommerce.GiftCards/EditGiftCardPrices.ascx");

			section.Menu.AddMenuButton(CreateGiftCardButton());

            section.AddControl(control);
            return section;
        }

	    private ImageButton CreateGiftCardButton()
	    {
			var createGiftCardButton = new ImageButton
			{
				ImageUrl = Presentation.Resources.Images.Menu.Create,
				CausesValidation = false
			};

			createGiftCardButton.Attributes.Add("onclick", "addNewVariantClick(); return false;");

			return createGiftCardButton;
	    }
    }
}
