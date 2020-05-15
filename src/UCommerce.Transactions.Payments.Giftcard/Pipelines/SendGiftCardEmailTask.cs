using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Common;
using Ucommerce.Transactions;
using UCommerce.Transactions.Payments.GiftCard.Extension;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines
{
	public class SendGiftCardEmailTask : IPipelineTask<PurchaseOrder>
	{
		private readonly string _emailTypeName;
		private readonly ILoggingService _loggingService;
		private readonly IEmailService _emailService;
		private readonly IRepository<Entities.GiftCard> _giftCardRepository;

	    public SendGiftCardEmailTask(string emailTypeName, 
			ILoggingService loggingService, 
			IEmailService emailService,
			IRepository<Entities.GiftCard> giftCardRepository)
		{
			_emailTypeName = emailTypeName;
			_loggingService = loggingService;
			_emailService = emailService;
			_giftCardRepository = giftCardRepository;
		}

		/// <summary>
		/// Sends email of type GiftCard if order is subject for sending a gift card.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		public PipelineExecutionResult Execute(PurchaseOrder subject)
		{
			var giftCards = _giftCardRepository.Select(x => x.OrderNumber == subject.OrderNumber);
			if (!giftCards.Any()) //Order not associated with a Gift Card.
				return PipelineExecutionResult.Success;

			var recieverEmail = GetRecieverEmail(subject);

			if (string.IsNullOrWhiteSpace(recieverEmail)) //no email recipient was found.
				return PipelineExecutionResult.Warning;

			// Override default culture with the one found on order to support different UI culture from order culture.

			var stringOfGiftCardCodes = string.Join(",", giftCards.Select(x => x.Code));
			
			var dictionary = new Dictionary<string, string>
					{
						{"orderNumber", subject.OrderNumber},
						{"orderGuid", subject.OrderGuid.ToString()},
						{"giftCardCodes", stringOfGiftCardCodes}
					};


		    var customCulture = new GiftCardGlobalization();
		    customCulture.SetCulture(new CultureInfo(subject.CultureCode ?? customCulture.DefaultCulture.ToString()));

            try
			{
				_emailService.Send(customCulture, subject.ProductCatalogGroup.EmailProfile, _emailTypeName,
								  new MailAddress(recieverEmail),
								  dictionary);
			}
			catch (SmtpException smtpException)
			{
				_loggingService.Log<SendEmailTask>(smtpException);
			}

			return PipelineExecutionResult.Success;

		}

		/// <summary>
		/// Finds a receiver address based on deliveryaddresses on an order.
		/// </summary>
		/// <param name="subject"></param>
		/// <returns></returns>
		protected virtual string GetRecieverEmail(PurchaseOrder subject)
		{
			string email = "";
			OrderAddress address = null;

			//We only want to send to delivery address if there's exactly one shipmentaddress
			//as number of codes and number of deliveryaddress might not match. Hence we cannot 
			//determine which recepient gets which giftcard. Also every deliveryaddress would get every code as configured in the task.
			if (subject.Shipments.Count == 1) 
				address = subject.Shipments.First().ShipmentAddress;

			if (address != null && !string.IsNullOrWhiteSpace(address.EmailAddress))
				email = address.EmailAddress;
				
			if (string.IsNullOrWhiteSpace(email))
			{
				address = subject.GetBillingAddress();
				if (address != null)
					email = address.EmailAddress;
			}

			return email;

		}
	}
}
