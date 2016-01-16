using UCommerce.Pipelines;
using UCommerce.Presentation.UI;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
	public class EditProductGiftCardTabTask : IPipelineTask<SectionGroup>
	{
		public PipelineExecutionResult Execute(SectionGroup sectionGroup)
		{
			if (sectionGroup.GetViewName() != "editproduct_aspx") return PipelineExecutionResult.Success;

            var section = BuildSection(sectionGroup);
           
			sectionGroup.Sections.Insert(1, section);
            sectionGroup.Controls.Add(section);

			return PipelineExecutionResult.Success;
		}

		private Section BuildSection(SectionGroup sectionGroup)
		{
			var section = new Section
			{
				Name = "Gift Card Price",
				ID = sectionGroup.CreateUniqueControlId()
			};

			var control = sectionGroup.View.LoadControl("/Apps/UCommerce.GiftCards/EditGiftCardPrices.ascx");

			section.AddControl(control);
			return section;
		}
	}
}
