using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Infrastructure.Logging;
using UCommerce.Transactions.Payments.GiftCard.Entities;
using UCommerce.Transactions.Payments.GiftCard.Pipelines;

namespace UCommerce.Transactions.Payments.GiftCard.Tests.Pipelines
{
	class SendGiftCardEmailTaskSpy : SendGiftCardEmailTask
	{
		public SendGiftCardEmailTaskSpy(
            string emailTypeName, 
            ILoggingService loggingService, 
            IEmailService emailService,
            IRepository<Entities.GiftCard> giftCardRepository) : base(emailTypeName, loggingService, emailService, giftCardRepository)
		{}

		public string GetSenderEmail(PurchaseOrder order)
		{
			return this.GetRecieverEmail(order);
		}
	}
}
