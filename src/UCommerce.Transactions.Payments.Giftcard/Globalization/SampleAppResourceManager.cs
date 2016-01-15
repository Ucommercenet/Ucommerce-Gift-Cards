using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using UCommerce.Infrastructure.Globalization;

namespace SampleApp.Extensions.Globalization
{
	/// <summary>
	/// Responsible for finding the right translation of a text based on a giving key, resource file and culture. 
	/// </summary>
	public class SampleAppResourceManager : IResourceManager
	{
		public string GetLocalizedText(string resource, string key)
		{
			return GetLocalizedText(Thread.CurrentThread.CurrentUICulture, resource, key);
		}

		/// <summary>
		/// Uses ResourceManager to return a translated string.
		/// Based on the created resource files. 
		/// The default translate is en.  
		/// </summary>
		/// <param name="culture"></param>
		/// <param name="resource"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetLocalizedText(CultureInfo culture, string resource, string key)
		{
			string resourceObject = new ResourceManager("SampleApp.Extensions.Globalization." + resource, this.GetType().Assembly).GetString(key, culture);

			if (resourceObject == null)
				return string.Format("[{0}]", key);

			return resourceObject;
		}
	}
}
