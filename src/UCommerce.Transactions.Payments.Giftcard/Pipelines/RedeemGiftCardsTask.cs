using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Transactions.Payments.Giftcard.Entities;

namespace UCommerce.Transactions.Payments.Giftcard.Pipelines
{
    /// <summary>
    /// Redeems every <see cref="GiftCard"/> associated with a purchaseOrder. 
    /// </summary>
    public class RedeemGiftCardsTask : IPipelineTask<PurchaseOrder>
    {
    	/// <summary>
        /// Redeems every <see cref="GiftCard"/> associated with a <see cref="PurchaseOrder"/>.
        /// </summary>
        /// <param name="subject">a specific payment which is beeing checked out and should have it's <see cref="GiftCard"/> redeemed..</param>
        /// <returns>success.</returns>
        public PipelineExecutionResult Execute(PurchaseOrder subject)
        {
			var giftCardPayments = subject.Payments
				.Where(x => x.PaymentProperties.Any(y => y.Key == "IsGiftCard" && y.Value == "True"))
				.ToList();
	                       
			foreach (var payment in giftCardPayments)
            {              
				payment.PaymentMethod.GetPaymentMethodService().AcquirePayment(payment);
            }

            return PipelineExecutionResult.Success;
        }
    }
}
