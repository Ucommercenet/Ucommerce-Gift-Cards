using System.Web.UI.WebControls;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;
using UCommerce.Presentation.Web.Controls;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.UI
{
    public class AddButtonToVariantsTabTask : IPipelineTask<SectionGroup>
    {
        public PipelineExecutionResult Execute(SectionGroup subject)
        {
            if (subject.GetViewName() != UCommerce.Constants.UI.Pages.Stores.Product)
                return PipelineExecutionResult.Success;
            
            var imageButton = new LabeledImageButton(); {};

            imageButton.CausesValidation = false;
            imageButton.OnClientClick = "addNewVariantClick(); return false;";
            imageButton.CommandName = "Add new price";
            
            foreach (var section in subject.Sections)
            {
                if (section.Name == "Gift Cards")
                {
                    section.Menu.AddMenuButton(imageButton);
                }
            }
            
            return PipelineExecutionResult.Success;
        }
    }
}