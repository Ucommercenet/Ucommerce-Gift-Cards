using System;
using UCommerce.Pipelines;
using UCommerce.Presentation.UI;

namespace UCommerce.Transactions.Payments.Giftcard.Pipelines
{
	public class EditPaymentMethodGiftCardsTabTask : IPipelineTask<SectionGroup>
	{
		public PipelineExecutionResult Execute(SectionGroup sectionGroup)
		{
			if (sectionGroup.GetViewName() != UCommerce.Constants.UI.Pages.Settings.PaymentMethod) return PipelineExecutionResult.Success;

			var section = BuildSection(sectionGroup);
			sectionGroup.AddSection(section);

			return PipelineExecutionResult.Success;
		}

		private Section BuildSection(SectionGroup sectionGroup)
		{
			var section = new Section
			{
				Name = "Gift Card Payment Method",
				ID = sectionGroup.CreateUniqueControlId()
			};

			var control = sectionGroup.View.LoadControl("/Apps/UCommerce.Transactions.Payments.Giftcard/EditPaymentMethodGiftCards.ascx");

			section.AddControl(control);
			return section;
		}
	}
}
