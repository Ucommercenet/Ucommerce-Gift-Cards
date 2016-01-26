using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Licensing;
using UCommerce.Presentation.Views.Settings.Orders;
using UCommerce.Presentation.Web;
using UCommerce.Presentation.Web.Controls;
using UCommerce.Presentation.Web.Pages;
using UCommerce.Security;
using UCommerce.Transactions.Payments.GiftCard.Entities.Security;

namespace UCommerce.Transactions.Payments.GiftCard.UI
{
	public partial class EditPaymentMethodGiftCards : ViewEnabledControl<IEditPaymentMethodView>, ISection
	{
		private readonly IRepository<Entities.GiftCard> _giftCardRepository;
		private readonly ISecurityService _securityService;
		private List<Entities.GiftCard> _giftCards;
		private List<Entities.GiftCard> GiftCards { 
			get
			{
				if (_giftCards == null)
					_giftCards = _giftCardRepository.Select(x => x.PaymentMethod == View.PaymentMethod).OrderByDescending(x => x.CreatedOn).ToList();
				return _giftCards;
			} 
		}
		private void BindListView()
		{
			GiftCardListView.DataSource = GiftCards;
		}

		public EditPaymentMethodGiftCards()
		{
			_giftCardRepository = ObjectFactory.Instance.Resolve<IRepository<Entities.GiftCard>>();
			_securityService = ObjectFactory.Instance.Resolve<ISecurityService>();
		}

		protected void Page_Init(object sender, EventArgs e)
		{
			View.Saved += View_Saved;

			GiftCardListView.DataSource = GiftCards;
			PopulateGiftCardSummary();

			BindListView();

			DataBind();
		}


		/// <summary>
		/// Populates the summary table.
		/// </summary>
		private void PopulateGiftCardSummary()
		{

			var currencySums = from giftCard in GiftCards
			                   group giftCard by giftCard.Currency
			                   into groupedByCurrency
			                   select new
								{
									Currency = groupedByCurrency.Key,
									ExpiredGiftCards = groupedByCurrency.Count(x => x.ExpiresOn < DateTime.Now),
									GiftCardsInUse = groupedByCurrency.Count(x => x.AmountUsed > 0 && x.AmountUsed < x.Amount && x.Enabled),
									TotalNumberOfGiftCards = groupedByCurrency.Count(),
									GiftCardsClosed = groupedByCurrency.Count(x => x.Amount == x.AmountUsed),
									OpenGiftCards = groupedByCurrency.Count(x => x.AmountUsed < x.Amount && x.Enabled && DateTime.Now < x.ExpiresOn),
									SpentBalance = new Money(groupedByCurrency.Sum(x => x.AmountUsed), groupedByCurrency.Key).Value.ToString("N"),
									OutstandingBalance = new Money(groupedByCurrency.Sum(x => x.Amount) - groupedByCurrency.Sum(x => x.AmountUsed), groupedByCurrency.Key).Value.ToString("N"),
									TotalBalance = new Money(groupedByCurrency.Sum(x => x.Amount), groupedByCurrency.Key).Value.ToString("N")
								};

			CurrencySummaryTable.DataSource = currencySums;
		}

		void View_Saved(object sender, Presentation.Views.Catalog.EntityCommandEventArgs<PaymentMethod> e)
		{
			UpdateGiftCards();
			PopulateGiftCardSummary();
		}

		public void UpdateGiftCards()
		{
			var giftCards = new List<Entities.GiftCard>();
			foreach (ListViewDataItem repeaterItem in GiftCardListView.Items)
			{
				var checkBox = (CheckBox)repeaterItem.FindControl("GiftCardEnabledCheckBox"); 
				var hiddenField = (HiddenField)repeaterItem.FindControl("GiftCardIdHiddenField");

				var enabled = checkBox.Checked;
				int giftCardId = Convert.ToInt32(hiddenField.Value);

				var giftCard = GiftCards.Single(x => x.GiftCardId == giftCardId);
				if (giftCard.Enabled != enabled) //Save only those giftcards that have had it's enabeld value swapped. 
				{
					giftCard.Enabled = enabled;
					giftCards.Add(giftCard); //Add to local list and save all through Repo in one hit.
				}
			}

			_giftCardRepository.Save(giftCards);
		}

		public void DownloadGiftCards()
		{
			var urlResolver = ObjectFactory.Instance.Resolve<IUrlResolver>();
			Response.Redirect(urlResolver.ResolveUrl(string.Format("/Settings/Orders/DownloadGiftCardCodes.ashx?Id={0}",View.PaymentMethod.PaymentMethodId)));
		}

		public IList<ICommand> GetCommands()
		{
			bool canCreateGiftCards = _securityService.UserIsInRole(Role.FirstOrDefault(x => x is CreateGiftCardRole));

			var list = new List<ICommand>();
			
			var createGiftCardbutton = new ClientImageCommand
				{
					Icon = Presentation.Resources.Images.Menu.Create,
					Text = GetLocalResourceObject("CreateGiftCard").ToString(),
					ClientCommand =
						JavaScriptFactory.OpenModalFunction(
							string.Format("/Settings/Orders/Dialogs/GenerateGiftCards.aspx?Id={0}",
			                           		View.PaymentMethod.PaymentMethodId),
							GetLocalResourceObject("CreateGiftCard").ToString(),
							700, 700)
				};

			var exportButton = new ImageCommand(x => DownloadGiftCards())
				{
					Icon = "media/table_save.png",
					Text = GetLocalResourceObject("Export").ToString()
				};

			if (canCreateGiftCards)
				list.Add(createGiftCardbutton);
			
			list.Add(exportButton);
			
			return list;
		}

		public bool Show
		{
			get
			{
				return (View.PaymentMethod.GetPaymentMethodService() is GiftCardPaymentMethodService);
			}
		}

		protected void GiftCardListView_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
		{
			GiftCardPager.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);	
			GiftCardListView.DataBind();
		}

		protected void GiftCard_ServerValidate(object source, ServerValidateEventArgs args)
		{
			foreach (ListViewDataItem repeaterItem in GiftCardListView.Items)
			{
				var checkBox = (CheckBox)repeaterItem.FindControl("GiftCardEnabledCheckBox");
				
				if (checkBox.Checked && !LicenseRestrictions.AllowCreate(typeof (Entities.GiftCard)))
				{
					args.IsValid = false;
					break;
				}
			}
		}
	}
}