using System;
using System.Collections.Generic;
using System.Security;
using System.Web.UI.WebControls;
using Ucommerce.EntitiesV2;
using Ucommerce.EntitiesV2.Definitions;
using Ucommerce.Infrastructure;
using Ucommerce.Licensing;
using Ucommerce.Presentation.UI;
using Ucommerce.Presentation.Web;
using Ucommerce.Presentation.Web.Controls;
using Ucommerce.Presentation.Web.Controls.Extensions;
using Ucommerce.Presentation.Web.Pages;
using Ucommerce.Security;
using Ucommerce.Transactions.Payments.GiftCard.Entities;
using Ucommerce.Transactions.Payments.GiftCard.Entities.Security;
using Ucommerce.Transactions.Payments.GiftCard.Services;

namespace UCommerce.Transactions.Payments.GiftCard.UI
{
	public partial class GenerateGiftCards : ProtectedPage
	{
		protected IPropertyControlFactory ControlFactory { get; set; }

		protected void Page_Load(object sender, EventArgs e)
		{
			ControlFactory = ObjectFactory.Instance.Resolve<IPropertyControlFactory>();

			var securityService = ObjectFactory.Instance.Resolve<ISecurityService>();
			//bool canCreateGiftCards = securityService.UserIsInRole(Role.FirstOrDefault(x => x is CreateGiftCardRole));
			//if (!canCreateGiftCards)
			//	throw new SecurityException("You are not allowed to create gift cards.");

			var datePicker = new DatePickerDataTypeDefinition {Name = "Date"};
			date.Controls.Clear();
			date.Controls.Add(ControlFactory.GetControl(datePicker, "DatePicker", ""));
			if (!IsPostBack)
			{
				(date.FindControl("DatePicker") as DatePicker).DateTime = DateTime.Now.AddYears(1);
				foreach (var currency in Currency.All())
				{
					CurrenciesDropDown.Items.Add(new ListItem(currency.Name, currency.CurrencyId.ToString()));
				}
			}
		}

		protected void SaveButton_Click(object sender, EventArgs e)
		{
			var giftCardService = ObjectFactory.Instance.Resolve<IGiftCardService>();

			bool enabled = LicenseRestrictions.AllowCreate(typeof(Entities.GiftCard));

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