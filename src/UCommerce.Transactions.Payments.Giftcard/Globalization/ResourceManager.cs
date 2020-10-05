using System.Globalization;
using System.Reflection;
using System.Threading;
using UCommerce.Infrastructure.Globalization;

namespace UCommerce.Transactions.Payments.GiftCard.Globalization
{
	public class ResourceManager : IResourceManager
	{
		public string GetLocalizedIcon(string key)
		{
			return "";
		}

		public string GetLocalizedText(string resource, string key)
		{
			return GetLocalizedText(Thread.CurrentThread.CurrentUICulture, resource, key);
		}

		public string GetLocalizedText(CultureInfo culture, string resource, string key)
		{
			string resourceObject = new System.Resources.ResourceManager("UCommerce.Transactions.Payments.GiftCard.UI.App_LocalResources." + resource, Assembly.Load("UCommerce.Transactions.Payments.GiftCard.UI")).GetString(key, culture);

			if (resourceObject == null)
				return string.Format("[{0}]", key);

			return resourceObject;
		}
	}
}
