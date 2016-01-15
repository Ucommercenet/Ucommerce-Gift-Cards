using System;
using System.Collections.Generic;
using System.Security;
using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Infrastructure;
using UCommerce.Licensing;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web;
using UCommerce.Presentation.Web.Controls;
using UCommerce.Presentation.Web.Controls.Extensions;
using UCommerce.Presentation.Web.Pages;
using UCommerce.Security;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Entities.Security;
using UCommerce.Transactions.Payments.Giftcard.Services;

namespace UCommerce.Transactions.Payments.Giftcard.UI
{
	public partial class GenerateGiftCards : ProtectedPage
	{
		protected IPropertyControlFactory ControlFactory { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			ControlFactory = ObjectFactory.Instance.Resolve<IPropertyControlFactory>();

			var securityService = ObjectFactory.Instance.Resolve<ISecurityService>();
			bool canCreateGiftCards = securityService.UserIsInRole(Role.FirstOrDefault(x => x is CreateGiftCardRole));
			if (!canCreateGiftCards)
				throw new SecurityException("You are not allowed to create gift cards.");


			date.Controls.Clear();
			date.Controls.Add(ControlFactory.GetControl(new DateTimePickerDataTypeDefinition(), "DatePicker", ""));
			if (!IsPostBack)
			{
				(date.FindControl("DatePicker") as DatePicker).DateTime = DateTime.Now.AddDays(365);
				foreach (var currency in Currency.All())
				{
					CurrenciesDropDown.Items.Add(new ListItem(currency.Name, currency.CurrencyId.ToString()));
				}
			}
		}

		protected void SaveButton_Click(object sender, EventArgs e)
		{
			var giftCardService = ObjectFactory.Instance.Resolve<IGiftCardService>();

			bool enabled = LicenseRestrictions.AllowCreate(typeof(GiftCard));

			var amount = Convert.ToDecimal(tbAmount.Text);
			var numbersToGenerate = Convert.ToInt32(tbNumberOfGiftCards.Text);

			var currencyId = Convert.ToInt32(CurrenciesDropDown.SelectedValue);
			var currency = Currency.Get(currencyId);
			
			var paymentMethodId = Convert.ToInt32(Request.QueryString["Id"]);
			var paymentMethod = PaymentMethod.Get(paymentMethodId);

			var expires = (DatePicker)date.FindControl("DatePicker");

			var balance = new Money(amount, currency);

			var giftCardRequests = new List<IssueGiftCardRequest>();

			for (int i = 0;i < numbersToGenerate; i++)
			{
				var giftCardRequest = new IssueGiftCardRequest(balance, enabled, expires.DateTime.Value, paymentMethod);
				giftCardRequest.Note = NoteTextBox.Text;

				giftCardRequests.Add(giftCardRequest);
			}
			
			giftCardService.IssueGiftCards(giftCardRequests);
			
			ClientScript.RegisterClientScriptBlock(GetType(), "refresh",
				JavaScriptFactory.CreateJavascript(
					JavaScriptFactory.ContentFrameFunction(
						string.Format("/settings/Orders/EditPaymentMethod.aspx?id={0}&currenttab={1}",
										QueryString.Common.Id,
										"EditPaymentMethodGiftCards.ascx")
					),
					JavaScriptFactory.CloseModalWindowFunction()));
		}
	}
}