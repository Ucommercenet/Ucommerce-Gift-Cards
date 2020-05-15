using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Configuration;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Transactions.Payments.GiftCard.Entities;
using Ucommerce.Transactions.Payments.GiftCard.Pipelines;

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
