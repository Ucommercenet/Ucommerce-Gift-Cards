using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Transactions.Payments.GiftCard.Entities;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
	/// <summary>
	/// Task for recalculating gift card payments on a purchaseorder.
	/// </summary>
    public class RecalculateGiftCardPaymentsAmountTask : IPipelineTask<PurchaseOrder>
    {
        private readonly IRepository<Entities.GiftCard> _giftCardRepository;

        public RecalculateGiftCardPaymentsAmountTask(IRepository<Entities.GiftCard> giftCardRepository)
        {
            _giftCardRepository = giftCardRepository;
        }

		/// <summary>
		/// Recalculates a <see cref="PurchaseOrder">PurchaseOrders</see> gift card payments 
		/// matching highest possible amount covered by gift card
		/// and closing as many gift cards as possible.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
        public PipelineExecutionResult Execute(PurchaseOrder subject)
        {
			//find all gift card codes associated with a purchaseorder. 
			var giftCardPayments = subject.Payments
				.Where(x => x.PaymentProperties.Any(y => y.Key == "IsGiftCard" && y.Value == "True"))
				.ToList();

			if (!giftCardPayments.Any())
				return PipelineExecutionResult.Success;

            decimal totalPaid = GetNonGiftCardPaymentTotal(subject);
            decimal unCoveredAmountOnOrder = subject.OrderTotal.GetValueOrDefault() - totalPaid;
            bool isCovered = unCoveredAmountOnOrder <= 0;
			
			IList<string> giftCardCodes = giftCardPayments.Select(x => x["GiftCardCode"]).ToList();

			//select all giftcards from the repository that matches any codes in giftCardCodes list.
	        IList<Entities.GiftCard> giftCardsUsedOnOrder = _giftCardRepository.Select().Where(x => giftCardCodes.Contains(x.Code)).ToList();

			//order payments by least available amount on giftcard
			foreach (Payment giftCardPayment in PaymentsOrderedByAvailableAmountOnGiftCard(giftCardsUsedOnOrder,giftCardPayments))
            {
				if (isCovered) //when total amount is covered set every other payment to zero
                {
					giftCardPayment.Amount = 0;
                    continue;
                }

            	var giftCard = giftCardsUsedOnOrder.Single(x => x.Code == giftCardPayment["GiftCardCode"]); 
                decimal giftCardAvailableBalance = giftCard.AvailableBalance(); 

				if (RemainingAmountIsLargerThanGiftCardBalance(unCoveredAmountOnOrder, giftCardAvailableBalance))
                {
					//remaining amount and payment amount is set to the available balance of the giftcard
                    unCoveredAmountOnOrder -= giftCardAvailableBalance; 
                    giftCardPayment.Amount = giftCardAvailableBalance;

                    if (unCoveredAmountOnOrder == 0) //amount is covered if uncovered amount reaches zero here
                        isCovered = true;
                }
                else //available balance on gift card cover's remaining amount
                {
                    giftCardPayment.Amount = unCoveredAmountOnOrder;
                    isCovered = true;
                }
            }
			
            return PipelineExecutionResult.Success;
        }

		private IEnumerable PaymentsOrderedByAvailableAmountOnGiftCard(IList<Entities.GiftCard> giftCardsUsedOnOrder, IList<Payment> giftCardPayments)
		{
			return giftCardPayments.OrderBy(x => giftCardsUsedOnOrder.Single(y => y.Code == x["GiftCardCode"]).AvailableBalance());
		}

		private bool RemainingAmountIsLargerThanGiftCardBalance(decimal unCoveredAmountOnOrder, decimal giftCardAvailableBalance)
		{
			return unCoveredAmountOnOrder - giftCardAvailableBalance >= 0;
		}

		/// <summary>
		/// Returns the total amount of payments not associated with a gift card.
		/// </summary>
		/// <param name="subject">The <see cref="PurchaseOrder"/> with gift card payments.</param>
		/// <returns>Total amount of payments not associated with a gift card.</returns>
        private decimal GetNonGiftCardPaymentTotal(PurchaseOrder subject)
		{
			return subject.Payments
				.Where(x => (
								x.PaymentStatus.PaymentStatusId == (int) PaymentStatusCode.Acquired
								|| x.PaymentStatus.PaymentStatusId == (int) PaymentStatusCode.Authorized
							)
							&& x["IsGiftCard"] != "True")
				.Sum(x => x.Amount);
		}
    }
}