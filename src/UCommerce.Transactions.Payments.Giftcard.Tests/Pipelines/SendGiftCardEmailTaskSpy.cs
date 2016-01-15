using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Text;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines.Checkout;
using UCommerce.Transactions;

namespace UCommerce.Pipelines.Test.GiftCards
{
	class SendGiftCardEmailTaskSpy : SendGiftCardEmailTask
	{
		public SendGiftCardEmailTaskSpy(string emailTypeName, ILoggingService loggingService, IEmailService emailService, CommerceConfigurationProvider commerceConfigurationProvider, IRepository<GiftCard> giftCardRepository) : base(emailTypeName, loggingService, emailService, commerceConfigurationProvider, giftCardRepository)
		{

		}


		public string GetSenderEmail(PurchaseOrder order)
		{
			return this.GetRecieverEmail(order);
		}
	}
}
