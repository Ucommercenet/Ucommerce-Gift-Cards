using System.Web.UI.WebControls;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web;
using UCommerce.Security;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
	public class EditPaymentMethodGiftCardsTabTask : IPipelineTask<SectionGroup>
	{
		private readonly ISecurityService _securityService;
		private readonly IJavaScriptFactory _javaScriptFactory;

		public EditPaymentMethodGiftCardsTabTask(ISecurityService securityService, IJavaScriptFactory javaScriptFactory)
		{
			_securityService = securityService;
			_javaScriptFactory = javaScriptFactory;
		}

		public PipelineExecutionResult Execute(SectionGroup sectionGroup)
		{
			if (sectionGroup.GetViewName() != UCommerce.Constants.UI.Pages.Settings.PaymentMethod)
                return PipelineExecutionResult.Success;

			var section = BuildSection(sectionGroup);
			sectionGroup.AddSection(section);

			return PipelineExecutionResult.Success;
		}

		private Section BuildSection(SectionGroup sectionGroup)
		{
			var section = new Section
			{
				Name = "Gift Card Payment Method",
				ID = sectionGroup.CreateUniqueControlId()
			};

			var control = sectionGroup.View.LoadControl("/Apps/UCommerce.GiftCards/EditPaymentMethodGiftCards.ascx");

			//if (_securityService.UserIsInRole(Role.FirstOrDefault(x => x is CreateGiftCardRole)))
			//{
				section.Menu.AddMenuButton(CreateGenerateGiftCardImageButton());
				section.Menu.AddMenuButton(CreateExportButton());
			//}

			section.AddControl(control);
			return section;
		}

		private ImageButton CreateGenerateGiftCardImageButton()
		{
			var generateGiftCardButton = new ImageButton
			{
				ImageUrl = Presentation.Resources.Images.Menu.Create
			};
			generateGiftCardButton.Attributes.Add("onclick", _javaScriptFactory.OpenModalFunction(string.Format("/Apps/Gift Cards/GenerateGiftCards.aspx?Id={0}", QueryString.Common.Id), "Create Gift Card", 700, 700));

			return generateGiftCardButton;
		}

		private ImageButton CreateExportButton()
		{
			var exportButton = new ImageButton
			{
				ImageUrl = "/media/table_save.png",
				CausesValidation = false,
			};

			exportButton.Attributes.Add("onclick", "if (confirm('" + "WOOP" + "')) { window.location.replace(" + string.Format("/Apps/Gift Cards/DownloadGiftCardCodes.ashx?Id={0}", QueryString.Common.Id) + "); } return false;");
			
			return exportButton;
		}
	}
}
