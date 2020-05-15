using FluentNHibernate.Mapping;
using UCommerce.Transactions.Payments.GiftCard.Entities.Security;

namespace UCommerce.Transactions.Payments.GiftCard.Entities.Maps
{
    class CreateGiftCardRoleMap : SubclassMap<CreateGiftCardRole>
    {
        public CreateGiftCardRoleMap()
        {
            Table("uCommerce_Role");
            DiscriminatorValue(170);
        }
    }
}