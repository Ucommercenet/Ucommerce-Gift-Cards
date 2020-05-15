using System.Web.UI.WebControls;
using Ucommerce.Pipelines;
using Ucommerce.Presentation.UI;
using Ucommerce.Presentation.Web.Controls;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.UI
{
    public class AddButtonToVariantsTabTask : IPipelineTask<SectionGroup>
    {
        public PipelineExecutionResult Execute(SectionGroup subject)
        {
            var imageButton = new LabeledImageButton();
            {

            };

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