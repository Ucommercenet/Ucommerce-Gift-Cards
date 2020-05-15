using Ucommerce.EntitiesV2;

namespace UCommerce.Transactions.Payments.GiftCard.Entities.Security
{
	/// <summary>
	/// A subtype of <see cref="Role"/> representing the ablity to Create <see cref="GiftCard">Gift Cards</see>.
	/// </summary>
	public class CreateGiftCardRole : Role
	{
		public CreateGiftCardRole(string name)
		{
			Name = name;
		}

		public CreateGiftCardRole() { }
	}
}
