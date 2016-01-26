using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Infrastructure.Runtime;
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
		private readonly IPathService _pathService;
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;
		private readonly IResourceManager _resourceManager;

		public EditPaymentMethodGiftCardsTabTask(ISecurityService securityService, IJavaScriptFactory javaScriptFactory, IPathService pathService, IRepository<PaymentMethod> paymentMethodRepository, IResourceManager resourceManager)
		{
			_securityService = securityService;
			_javaScriptFactory = javaScriptFactory;
			_pathService = pathService;
			_paymentMethodRepository = paymentMethodRepository;
			_resourceManager = resourceManager;
		}

		public PipelineExecutionResult Execute(SectionGroup sectionGroup)
		{
			if (sectionGroup.GetViewName() != UCommerce.Constants.UI.Pages.Settings.PaymentMethod)
				return PipelineExecutionResult.Success;

			var paymentMethod = _paymentMethodRepository.Get(QueryString.Common.Id);

			if (paymentMethod == null || paymentMethod.PaymentMethodServiceName != Constants.GiftCardPaymentMethodName)
			{
				 return PipelineExecutionResult.Success;
			}

			var section = BuildSection(sectionGroup);
			sectionGroup.AddSection(section);

			return PipelineExecutionResult.Success;
		}

		private Section BuildSection(SectionGroup sectionGroup)
		{
			var section = new Section
			{
				Name = _resourceManager.GetLocalizedText("EditPaymentMethodGiftCards.ascx", "GiftCards.Text"),
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
				ImageUrl = Presentation.Resources.Images.Menu.Create,
				CausesValidation = false
			};

			generateGiftCardButton.Attributes.Add("onclick", _javaScriptFactory.OpenModalFunction(string.Format("/Apps/UCommerce.GiftCards/GenerateGiftCards.aspx?Id={0}", QueryString.Common.Id), "Create Gift Card", 700, 700));

			return generateGiftCardButton;
		}

		private ImageButton CreateExportButton()
		{
			var exportButton = new ImageButton
			{
				ImageUrl = string.Format("{0}/Apps/UCommerce.GiftCards/media/table_save.png", _pathService.GetPath()),
				CausesValidation = false,
			};

			var pathString = string.Format("{0}/Apps/UCommerce.GiftCards/DownloadGiftCardCodes.ashx?Id={1}", _pathService.GetPath(), QueryString.Common.Id);

			pathString = pathString.Substring(1, pathString.Length - 1);

			exportButton.Attributes.Add("onclick", "window.location.replace('" + pathString + "'); return false;");
			return exportButton;
		}
	}
}
