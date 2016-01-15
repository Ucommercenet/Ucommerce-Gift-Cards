using System;
using System.Linq;
using System.Reflection;
using SampleApp.Extensions.Queries;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;

namespace SampleApp.Extensions.Api
{
	/// <summary>
	/// SampleApi for helper methods
	/// </summary>
	/// <remarks></remarks>
	public static class SampleApi
	{
		public static string uCommerceVersion()
		{
			string binPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			var assemblyName = AssemblyName.GetAssemblyName(string.Format("{0}\\uCommerce.dll", binPath));
			return assemblyName.Version.ToString();
		}

		public static string SchemaVersion()
		{
			var sessionProvider = ObjectFactory.Instance.Resolve<ISessionProvider>();
			
			//Don't use the using(var session = _sessionProvider.GetSession()) pattern as the session will be disposed at the end of the http request AND
			//Disposing the session here will cause trouble as lazy loading for all entities will then be broken.
			var session = sessionProvider.GetSession();

			return new SchemaVersionQuery().Execute(session).FirstOrDefault().ToString();

		}
	}
}
