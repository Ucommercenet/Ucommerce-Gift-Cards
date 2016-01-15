using System;
using SampleApp.Extensions.Api;

namespace SampleApp.Web
{
	public partial class About : System.Web.UI.Page
	{
		/// <summary>
		/// Loads uCommerce version and the database schema version. 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void Page_Load(object sender, EventArgs e)
		{
			uCommerceVersion.Text = SampleApi.uCommerceVersion();

			SchemaVersion.Text = SampleApi.SchemaVersion();
		}
	}
}