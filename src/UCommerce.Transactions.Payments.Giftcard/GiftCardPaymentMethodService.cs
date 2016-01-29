﻿using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;
using UCommerce.Runtime;

namespace UCommerce.Transactions.Payments.GiftCard
{
    /// <summary>
    /// Default implementation of a PaymentMethodService for gift cards.
    /// This implementation provides out of the box functionallity to handle creation and payments 
    /// with <see cref="Entities.GiftCard">Giftcards</see>. 
    /// </summary>
    public class GiftCardPaymentMethodService : ExternalPaymentMethodService
    {
        private readonly IRepository<Entities.GiftCard> _giftCardRepsitory;
        private readonly IRepository<PaymentStatus> _paymentStatusRepository;
        private readonly IResourceManager _resourceManager;
	    private readonly IRepository<Payment> _paymentRepository;
        private readonly IOrderContext _orderContext;

        public GiftCardPaymentMethodService(
            IRepository<Entities.GiftCard> giftCardRepsitory, 
            IRepository<PaymentStatus> paymentStatusRepository, 
            IResourceManager resourceManager,
            IRepository<Payment> paymentRepository,
            IOrderContext orderContext)
        {
            _giftCardRepsitory = giftCardRepsitory;
            _paymentStatusRepository = paymentStatusRepository;
            _resourceManager = resourceManager;
	        _paymentRepository = paymentRepository;
            _orderContext = orderContext;
        }

        /// <summary>
        /// Ass this is an internal handeler for <see cref="Entities.GiftCard">Giftcards</see> no communication with external services are required.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns></returns>
        public override string RenderPage(PaymentRequest paymentRequest)
        {
            throw new NotSupportedException("GiftCardPaymentMethodService does not require a placeholder page.");
        }

        /// <summary>
        /// Ass this is an internal handeler for <see cref="Entities.GiftCard">Giftcards</see> no communication with external services are required.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public override void ProcessCallback(Payment payment)
        {
            throw new NotSupportedException("GiftCardPaymentMethodService does not communicate with an external payment gateway.");
        }

        /// <summary>
        /// Cancel's a payment.
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected override bool CancelPaymentInternal(Payment payment, out string status)
        {
            if (payment.PaymentStatus == _paymentStatusRepository.Get((int)PaymentStatusCode.Acquired))
                throw new InvalidOperationException("Use RefundPayment on payment with status Acquried.");

            payment.PaymentStatus = _paymentStatusRepository.Get((int)PaymentStatusCode.Cancelled);
            _paymentRepository.Save(payment);

            status = _resourceManager.GetLocalizedText("PaymentMessages.ascx", "CancelSuccess");
            return true;
        }

        /// <summary>
        /// Acquire payment on giftcard adding totalused on giftcard with the amount of the payment.
        /// </summary>
        /// <param name="payment">Payment associated with a giftCard.</param>
        /// <param name="status">Status that tells weatather the acquire went well or not.</param>
        /// <returns></returns>
        protected override bool AcquirePaymentInternal(Payment payment, out string status)
        {
            if (payment.PaymentStatus != null && payment.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.Acquired)
            {
                status = _resourceManager.GetLocalizedText("PaymentMessages.ascx", "AcquireSuccess");
                return true;                
            }

			var giftCard = GetGiftCard(payment[Constants.GiftCardCodePaymentPropertyName]);

            string giftCardStatus;
            if (!GiftCardIsValid(giftCard, out giftCardStatus))
                throw new InvalidOperationException(giftCardStatus);

            giftCard.AmountUsed += payment.Amount;
            _giftCardRepsitory.Save(giftCard);

			payment.PaymentStatus = _paymentStatusRepository.Get((int)PaymentStatusCode.Acquired);
			_paymentRepository.Save(payment);

            status = _resourceManager.GetLocalizedText("PaymentMessages.ascx", "AcquireSuccess");
            return true;
        }

        /// <summary>
        /// Removes the amount of the payment from total used on the <see cref="Entities.GiftCard"/>.
        /// </summary>
        /// <param name="payment">Payment associated with a giftCard.</param>
        /// <param name="status">Status that tells weatather the Refund went well or not.</param>
        /// <returns></returns>
        protected override bool RefundPaymentInternal(Payment payment, out string status)
        {
            var acquiredPaymentStatusCode = PaymentStatusCode.Acquired;
            if (payment.PaymentStatus != _paymentStatusRepository.Get((int)acquiredPaymentStatusCode))
                throw new InvalidOperationException(string.Format("Cannot refund payment on payment with paymentStatus: {0}.", payment.PaymentStatus.Name));

			var giftCard = _giftCardRepsitory.SingleOrDefault(x => x.Code == payment[Constants.GiftCardCodePaymentPropertyName]);
            giftCard.AmountUsed -= payment.Amount;
            _giftCardRepsitory.Save(giftCard);
			
			payment.PaymentStatus = _paymentStatusRepository.Get((int) PaymentStatusCode.Refunded);
			_paymentRepository.Save(payment);

			status = _resourceManager.GetLocalizedText("PaymentMessages.ascx", "RefundSuccess");

            return true;
        }

        /// <summary>
        /// Creates a payment if a correspondig <see cref="Entities.GiftCard"/> is valid.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns></returns>
        public override Payment RequestPayment(PaymentRequest paymentRequest)
        {
			var purchaseOrder = paymentRequest.PurchaseOrder;

			decimal paymentsMadeTotal = purchaseOrder.Payments
				.Where(x =>
					   x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.Authorized
					   || x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.Acquired)
				.Sum(x => x.Amount);

			//We can only execute post processing pipeline if authorized payments are greater than the order total which indicates that 
			//payments covers either a single or multiple gift cards. 
			//External payment gateways will do redirect and cover the flow from here.  
			if (paymentsMadeTotal < purchaseOrder.OrderTotal.GetValueOrDefault())
				return paymentRequest.Payment;

	        return ProcessPaymentRequest(paymentRequest);
        }

	    public override Payment ProcessPaymentRequest(PaymentRequest request)
	    {
		    return base.ProcessPaymentRequest(request);
	    }

	    public override PipelineExecutionResult ExecutePostProcessingPipeline(Payment payment)
	    {
		    return base.ExecutePostProcessingPipeline(payment);
	    }

	    /// <summary>
        /// Finds and returns a <see cref="Entities.GiftCard"/> from a <see cref="Payment"/>.
        /// </summary>
        /// <param name="giftCardCode">The identifier of a giftcard.</param>
        /// <returns>A <see cref="Entities.GiftCard"/>.</returns>
        private Entities.GiftCard GetGiftCard(string giftCardCode)
        {
            return _giftCardRepsitory.SingleOrDefault(x => x.Code == giftCardCode);
        }

        /// <summary>
        /// Finds and returns a <see cref="Entities.GiftCard"/> from a <see cref="PaymentRequest"/>.
        /// </summary>
        /// <param name="paymentRequest">The <see cref="PaymentRequest"/>.</param>
        /// <returns>A <see cref="Entities.GiftCard"/>.</returns>
        private Entities.GiftCard GetGiftCard(PaymentRequest paymentRequest)
        {
            string giftCardCode;
			paymentRequest.AdditionalProperties.TryGetValue(Constants.GiftCardCodePaymentPropertyName, out giftCardCode);

			if (giftCardCode == null)
				giftCardCode = paymentRequest.Payment[Constants.GiftCardCodePaymentPropertyName];
            
            return _giftCardRepsitory.SingleOrDefault(
                x => x.Code == giftCardCode);   
        }

        /// <summary>
        /// Checks weather a <see cref="Entities.GiftCard"/> is valid as a Paymentmethod for a <see cref="PaymentRequest"/>
        /// </summary>
        /// <param name="giftCard">The <see cref="Entities.GiftCard"/>.</param>
        /// <param name="status">Status message for operation.</param>
        /// <returns>True if a <see cref="GiftCard"/> is valid.</returns>
        private bool GiftCardIsValid(Entities.GiftCard giftCard, out string status)
        {
			status = "OK";
            
			if (giftCard == null)
            {
				status = "No gift cards with specified code.";
            	return false;
            }

            if (giftCard.AmountUsed == giftCard.Amount)
            {
            	status = string.Format("There's no available funds on the giftCard with code: {0}.", giftCard.Code);
            	return false;
            }

            if (giftCard.ExpiresOn < DateTime.Now)
            {
            	status = string.Format("The gift card expired on date: {0}.", giftCard.ExpiresOn);
            	return false;
            }

            if (!giftCard.Enabled)
            {
            	status = "The gift card has been disabled and can no longer be used for purchases.";
            	return false;
            }
        	return true;
        }

        /// <summary>
        /// Creates a <see cref="Payment"/> when a user registers a GiftCard on a <see cref="Basket"/>.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A new created Payment.</returns>
        public override Payment CreatePayment(PaymentRequest request)
        {
			var paymentWithSameCode = request.PurchaseOrder.Payments.FirstOrDefault(x => x.TransactionId == request.AdditionalProperties[Constants.GiftCardCodePaymentPropertyName]);

            if (paymentWithSameCode != null)
                return null;

            Entities.GiftCard giftCard = GetGiftCard(request);
           
            if (giftCard == null)
                return null;

            if (giftCard.Currency != request.PurchaseOrder.BillingCurrency)
                return null;
			
			string status;
            if (GiftCardIsValid(giftCard, out status))
            {
                var notUsedAmount = giftCard.Amount - giftCard.AmountUsed;
                var amount = request.PurchaseOrder.OrderTotal.GetValueOrDefault(0);
                var basket = _orderContext.GetBasket(false).PurchaseOrder;

                amount = GetRemainingAmountToPay(basket, amount);

                if (notUsedAmount < 0) return null;

                Payment payment = base.CreatePayment(request);
                payment.Amount = Math.Min(notUsedAmount, amount);
                payment.ReferenceId = GetReferenceId(request);
                payment.PaymentStatus = _paymentStatusRepository.Get((int)PaymentStatusCode.Authorized);
                payment.TransactionId = request.AdditionalProperties[Constants.GiftCardCodePaymentPropertyName];
                payment["paymentGuid"] = Guid.NewGuid().ToString();
                payment["IsGiftCard"] = "True";
				payment[Constants.GiftCardCodePaymentPropertyName] = request.AdditionalProperties[Constants.GiftCardCodePaymentPropertyName];
				_paymentRepository.Save(payment);

				return payment;
            }

            return null;
        }

        private static decimal GetRemainingAmountToPay(PurchaseOrder basket, decimal amount)
        {
            decimal amountAlreadyCoveredByPayments = basket.Payments.Where(x =>
                x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.New ||
                x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.PendingAuthorization ||
                x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.Authorized ||
                x.PaymentStatus.PaymentStatusId == (int)PaymentStatusCode.Acquired)
                .Sum(x => x.Amount);

            if (amountAlreadyCoveredByPayments > 0)
                amount -= amountAlreadyCoveredByPayments;

            return amount;
        }
    }
}
