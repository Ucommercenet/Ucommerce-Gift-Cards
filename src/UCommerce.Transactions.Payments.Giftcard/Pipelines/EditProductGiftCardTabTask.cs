using System.Web.UI.WebControls;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Globalization;
using Ucommerce.Pipelines;
using Ucommerce.Presentation.UI;
using Ucommerce.Presentation.Web;
using Ucommerce.Presentation.Web.Controls;

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

			if (product == null || product.ProductDefinition.Name != Constants.GiftCardProductDefinition) return PipelineExecutionResult.Success;

			var section = BuildSection(sectionGroup);
			sectionGroup.Sections.Insert(1, section);
			sectionGroup.Controls.Add(section);

			return PipelineExecutionResult.Success;
		}

		private Section BuildSection(SectionGroup sectionGroup)
		{
			var section = new Section
			{
				Name = _resourceManager.GetLocalizedText("EditGiftCardPrices.ascx", "GiftCardTabName"),
				ID = sectionGroup.CreateUniqueControlId()
			};

			var control = sectionGroup.View.LoadControl("/Apps/UCommerce.GiftCards/EditGiftCardPrices.ascx");

			section.Menu.AddMenuButton(new SaveButtonPlaceholder());
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