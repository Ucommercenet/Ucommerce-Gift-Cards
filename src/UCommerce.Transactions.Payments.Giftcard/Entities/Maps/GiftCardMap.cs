using UCommerce.EntitiesV2.Maps;

namespace UCommerce.Transactions.Payments.GiftCard.Entities.Maps
{
    public class GiftCardMap : BaseClassMap<GiftCard>
    {
        public GiftCardMap()
        {
            Id(x => x.GiftCardId);

            Map(x => x.OrderNumber);
            Map(x => x.Amount);
            Map(x => x.AmountUsed);
            Map(x => x.CreatedOn);
            Map(x => x.CreatedBy);
        	Map(x => x.ModifiedOn);
			Map(x => x.ModifiedBy);
			Map(x => x.ExpiresOn);
            Map(x => x.Code);
            Map(x => x.Enabled);
        	Map(x => x.Note);
            References(x => x.Currency).Not.Nullable();
            References(x => x.PaymentMethod).Not.Nullable();
        }
    }
}