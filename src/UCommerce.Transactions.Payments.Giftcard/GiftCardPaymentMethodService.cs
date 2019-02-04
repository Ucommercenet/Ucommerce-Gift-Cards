using System;
using System.Configuration;
using System.Linq;
using System.Security;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Infrastructure.Components.Windsor;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Pipelines;
using UCommerce.Runtime;
using UCommerce.Web;
using UCommerce.Web.Impl;

namespace UCommerce.Transactions.Payments.GiftCard
{
    /// <summary>
    /// Default implementation of a PaymentMethodService for gift cards.
    /// This implementation provides out of the box functionallity to handle creation and payments 
    /// with <see cref="Entities.GiftCard">Giftcards</see>. 
    /// </summary>
    public class GiftCardPaymentMethodService : ExternalPaymentMethodService, IRequireRedirect
    {

        [Mandatory]
        public IAbsoluteUrlService AbsoluteUrlService { get; set; }

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
        /// As this is an internal handler for <see cref="Entities.GiftCard">Giftcards</see> no communication with external services is required.
        /// </summary>
        /// <param name="paymentRequest"></param>
        /// <returns></returns>
        public override string RenderPage(PaymentRequest paymentRequest)
        {
            RequestPayment(paymentRequest);
            throw new NotSupportedException("GiftCardPaymentMethodService does not require a placeholder page.");
        }

        /// <summary>
        /// As this is an internal handler for <see cref="Entities.GiftCard">Giftcards</see> no communication with external services is required.
        /// </summary>
        /// <param name="payment"></param>
        /// <returns></returns>
        public override void ProcessCallback(Payment payment)
        {
            throw new NotSupportedException("GiftCardPaymentMethodService does not communicate with an external payment gateway.");
        }

        /// <summary>
        /// Cancels a payment.
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
        /// Acquire payment on GiftCard adding total used on a GiftCard with the amount of the payment.
        /// </summary>
        /// <param name="payment">Payment associated with a GiftCard.</param>
        /// <param name="status">Status that tells whether the acquire went well or not.</param>
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

			if (!AvailableBalanceOnGiftCardCoversPaymentCreated(payment, giftCard))
				throw new SecurityException(string.Format("Balance on gift card does not cover the payment created. Gift card balance: {0} payment amount: {1} ", giftCard.AvailableBalance(), payment.Amount));

            giftCard.AmountUsed += payment.Amount;
            _giftCardRepsitory.Save(giftCard);

			payment.PaymentStatus = _paymentStatusRepository.Get((int)PaymentStatusCode.Acquired);
			_paymentRepository.Save(payment);

            status = _resourceManager.GetLocalizedText("PaymentMessages.ascx", "AcquireSuccess");
            return true;
        }

	    private bool AvailableBalanceOnGiftCardCoversPaymentCreated(Payment payment, Entities.GiftCard giftCard)
	    {
		    return giftCard.AvailableBalance() >= payment.Amount;
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
        /// Creates a payment if a corresponding <see cref="Entities.GiftCard"/> is valid.
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
        /// Checks whether a <see cref="Entities.GiftCard"/> is valid as a Payment method for a <see cref="PaymentRequest"/>
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

        public string GetRedirectUrl(Payment payment)
        {
            var redirectUrl = payment.PaymentMethod.DynamicProperty<string>().AcceptUrl;
            if (string.IsNullOrWhiteSpace(redirectUrl))
                throw new ConfigurationErrorsException("No RedirectUrl (absolute or relative) has been configured for DefaultPaymentMethodService. Please configure one under settings/paymentmethods/defaultpaymentmethodservice in the Ucommerce backoffice. Thanks in advance.");

            return AbsoluteUrlService.GetAbsoluteUrl(redirectUrl.ToString());
        }
    }
}
