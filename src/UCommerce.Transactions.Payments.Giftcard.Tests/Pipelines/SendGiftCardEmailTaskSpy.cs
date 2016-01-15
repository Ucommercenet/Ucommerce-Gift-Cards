using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Infrastructure.Logging;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Pipelines;

namespace UCommerce.Transactions.Payments.Giftcard.Tests.Pipelines
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
