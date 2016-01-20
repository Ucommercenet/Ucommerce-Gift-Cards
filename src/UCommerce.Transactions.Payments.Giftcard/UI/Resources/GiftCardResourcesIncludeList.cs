using System.Web.UI;
using ClientDependency.Core;
using UCommerce.Presentation.UI.Resources;

namespace UCommerce.Transactions.Payments.GiftCard.UI.Resources
{
	[ClientDependency(ClientDependencyType.Css, "Apps/UCommerce.GiftCards/Css/GiftCard.css", "UCommerce")]

	public class GiftCardResourcesIncludeList : Control, IResourcesIncludeList
	{
		public Control GetControl()
		{
			return this;
		}
	}
}
