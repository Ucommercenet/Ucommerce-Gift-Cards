using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Runtime;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web.Controls;
using UCommerce.Security;
using UCommerce.Transactions.Payments.GiftCard.Entities.Security;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.UI
{
    public class AddButtonToPaymentMethodTask : IPipelineTask<SectionGroup>
    {
        private readonly IJavaScriptFactory _javaScriptFactory;
        private readonly ISecurityService _securityService;

        public AddButtonToPaymentMethodTask(IJavaScriptFactory javaScriptFactory, ISecurityService securityService)
        {
            _javaScriptFactory = javaScriptFactory;
            _securityService = securityService;
        }

        public PipelineExecutionResult Execute(SectionGroup subject)
        {
            if (subject.GetViewName() != UCommerce.Constants.UI.Pages.Settings.PaymentMethod)
                return PipelineExecutionResult.Success;
            
            bool canCreateGiftCards = _securityService.UserIsInRole(Role.FirstOrDefault(x => x is CreateGiftCardRole));
            
            var imageButton = new LabeledImageButton(); {};

            var paymentMethodId = System.Web.HttpContext.Current.Request.QueryString["id"];

            imageButton.CausesValidation = false;
            imageButton.OnClientClick = _javaScriptFactory.OpenModalFunction(
                $"/Apps/UCommerce.GiftCards/GenerateGiftCards?Id={paymentMethodId}",
                "Create Gift Card",
                700, 700);
            imageButton.CommandName = "Issue Gift Card";
            
            foreach (var section in subject.Sections)
            {
                if (section.Name == "Gift Cards" && canCreateGiftCards)
                {
                    section.Menu.AddMenuButton(imageButton);
                }
            }
            
            return PipelineExecutionResult.Success;
        }
    }
}