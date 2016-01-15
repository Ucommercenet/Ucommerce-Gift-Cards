using FluentNHibernate.Mapping;
using UCommerce.Transactions.Payments.Giftcard.Entities.Security;

namespace UCommerce.Transactions.Payments.Giftcard.Entities.Maps
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
