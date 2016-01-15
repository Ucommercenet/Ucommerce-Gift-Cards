using System.Linq;
using System.Web.UI.WebControls;
using UCommerce;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Infrastructure.Runtime;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;

namespace SampleApp.Extensions.UI.Button
{
	/// <summary>
	/// Adds a Client side button to the Settings search view. 
	/// </summary>
	/// <remarks>
	/// Which renders the front page of the website in the uCommerce backoffice. 
	/// Uses the IPathService to locate the uCommerce folder under the website.  
	/// </remarks>
	public class AddClientSideButtonToSettingsSearchTask : IPipelineTask<SectionGroup>
	{
		private readonly IResourceManager _resourceManager;
		private readonly IPathService _pathService;
		public AddClientSideButtonToSettingsSearchTask(IResourceManager resourceManager, IPathService appPathService)
		{
			_resourceManager = resourceManager;
			_pathService = appPathService;
		}

		public PipelineExecutionResult Execute(SectionGroup subject)
		{
			if (subject.GetViewName() != Constants.UI.Pages.Settings.Search) return PipelineExecutionResult.Success;

			//Finds the right section by filtering on Name and OriginalName 
			var section = subject.Sections.FirstOrDefault(s => s.Name == "Common" && s.OriginalName == "IndexFromScratch.ascx");

			//If the view is not the one that we want to hook into, then do nothing
			if (section == null) return PipelineExecutionResult.Success;

			section.Menu.AddMenuButton(CreateClientSideButton());

			return PipelineExecutionResult.Success;
		}

		public ImageButton CreateClientSideButton()
		{
			var clientSideButton = new ImageButton();

			clientSideButton.ImageUrl = string.Format("{0}/Apps/SampleApp/Media/uCommerce-logo-symbol-small.png",_pathService.GetPath());
			clientSideButton.CausesValidation = false;

			//The client side command which executes on right click. 
			var translatedConfirmText = _resourceManager.GetLocalizedText("SampleApp", "confirmClientSideButton");
			clientSideButton.Attributes.Add("onclick", "if (confirm('" + translatedConfirmText + "')) { window.location.replace('/'); } return false;");
			
			return clientSideButton;
		}
	}
}
