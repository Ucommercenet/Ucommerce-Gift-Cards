using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Globalization;
using UCommerce.Transactions.Payments.GiftCard.Entities;
using UCommerce.Transactions.Payments.GiftCard.Pipelines;

namespace UCommerce.Transactions.Payments.GiftCard.Tests.Pipelines
{
    [TestFixture]
    public class RedeemGiftCardsTaskTest
    {
        [Test]
        public void Task_Can_Redeem_GiftCards()
        {
			var currency = new Currency();

            var giftCards = new List<Entities.GiftCard>();
            var giftcard1 = CreateGiftCard(400, 300, "12345", DateTime.Now, DateTime.Now.AddDays(40), true, currency);
            var giftcard2 = CreateGiftCard(500, 300, "123456", DateTime.Now, DateTime.Now.AddDays(40), true, currency);
            var giftcard3 = CreateGiftCard(600, 300, "1234586", DateTime.Now, DateTime.Now.AddDays(40), true, currency);

            giftCards.Add(giftcard1);
            giftCards.Add(giftcard2);
            giftCards.Add(giftcard3);

        	var paymentPropertyRepositoryStub = GetPaymentPropertyRepositoryStub();
            var giftCardRepositoryStub = GetGiftCardRepositoryStub(giftCards);

            var task = new RedeemGiftCardsTask();

            var purchaseOrder = new PurchaseOrder();
            purchaseOrder.OrderTotal = 600;
            purchaseOrder.BillingCurrency = currency;

			var propertiesForPayment1 = new List<PaymentProperty>();
        	propertiesForPayment1.Add(new PaymentProperty {Key = "GiftCardCode" , Value = "12345" });
			propertiesForPayment1.Add(new PaymentProperty { Key = "IsGiftCard", Value = "True" });

			var propertiesForPayment2 = new List<PaymentProperty>();
			propertiesForPayment2.Add(new PaymentProperty { Key = "GiftCardCode", Value = "123456" });
			propertiesForPayment2.Add(new PaymentProperty { Key = "IsGiftCard", Value = "True" });

			var propertiesForPayment3 = new List<PaymentProperty>();
			propertiesForPayment3.Add(new PaymentProperty { Key = "GiftCardCode", Value = "1234586" });
			propertiesForPayment3.Add(new PaymentProperty { Key = "IsGiftCard", Value = "True" });

			var paymentProperties = new List<PaymentProperty>();
			paymentProperties.AddRange(propertiesForPayment1);
			paymentProperties.AddRange(propertiesForPayment2);
			paymentProperties.AddRange(propertiesForPayment3);
			
        	paymentPropertyRepositoryStub.Stub(x => x.Select()).Return(paymentProperties.AsQueryable());

			purchaseOrder.AddPayment(CreateTestPayment(100, propertiesForPayment1, giftCardRepositoryStub, GetPaymentStatusRepo()));
			purchaseOrder.AddPayment(CreateTestPayment(200, propertiesForPayment2, giftCardRepositoryStub, GetPaymentStatusRepo()));
			purchaseOrder.AddPayment(CreateTestPayment(300, propertiesForPayment3, giftCardRepositoryStub, GetPaymentStatusRepo()));

            task.Execute(purchaseOrder);

            //Added giftcards should all be used as available amount on all of them are equal to the amount on their associated payment.
            Assert.True(giftcard1.AmountUsed == 400);
            Assert.True(giftcard2.AmountUsed == 500);
            Assert.True(giftcard3.AmountUsed == 600);

        }

    	private IRepository<PaymentProperty> GetPaymentPropertyRepositoryStub()
    	{
    		var repo = MockRepository.GenerateStub<IRepository<PaymentProperty>>();
    		return repo;
    	}

    	[Test]
        public void Task_Will_Not_Redeem_GiftCards_That_Are_Invalid()
        {
            var currency = new Currency();
            
			var giftCards = new List<Entities.GiftCard>();
            giftCards.Add(CreateGiftCard(400, 400, "12345", DateTime.Now, DateTime.Now.AddDays(40), true, currency));

			var paymentPropertyRepositoryStub = GetPaymentPropertyRepositoryStub();			
			var giftCardRepositoryStub = GetGiftCardRepositoryStub(giftCards);

			var task = new RedeemGiftCardsTask();

    		var properties = new List<PaymentProperty>();
			properties.Add(new PaymentProperty { Key = "GiftCardCode" , Value = "12345"} );
			properties.Add(new PaymentProperty { Key = "IsGiftCard", Value = "True" });


            var purchaseOrder = new PurchaseOrder();
            purchaseOrder.OrderTotal = 600;
            purchaseOrder.BillingCurrency = currency;
			purchaseOrder.AddPayment(CreateTestPayment(100, properties, giftCardRepositoryStub, GetPaymentStatusRepo()));

            Assert.Throws(Is.InstanceOf<InvalidOperationException>()
                            .And.Message
							.EqualTo("There's no available funds on the giftCard with code: 12345."), () => task.Execute(purchaseOrder));
        }

        private Payment CreateTestPayment(decimal amount, IList<PaymentProperty> properties, IRepository<Entities.GiftCard> giftCardRepo, IRepository<PaymentStatus> paymentStatusRepo)
        {
            
            var paymentMethod = MockRepository.GenerateStub<PaymentMethod>();
            paymentMethod.Stub(x => x.GetPaymentMethodService()).Return(GetPaymentMethodService(giftCardRepo,paymentStatusRepo));
            
            var payment = new Payment();
            payment.PaymentMethod = paymentMethod;
            payment.Amount = amount;
			payment.TransactionId = Guid.NewGuid().ToString();
        	foreach (var paymentProperty in properties)
        	{
        		payment.AddPaymentProperty(paymentProperty);
        	}

			return payment;
        }

        private IPaymentMethodService GetPaymentMethodService(IRepository<Entities.GiftCard> giftCardRepo, IRepository<PaymentStatus> paymentStatusRepo)
        {
            var paymentRepository = MockRepository.GenerateMock<IRepository<Payment>>();
            var resourceManager = MockRepository.GenerateMock<IResourceManager>();

            var service = new GiftCardPaymentMethodService(giftCardRepo, paymentStatusRepo, resourceManager, new OrderService(),paymentRepository);

            return service;
        }

        private Entities.GiftCard CreateGiftCard(decimal amount, decimal amountUsed, string code, DateTime createdOn, DateTime expiresOn, bool enabled, Currency currency)
        {
            var giftCard = new GiftCardSpy();

            giftCard.Amount = amount;
            giftCard.AmountUsed = amountUsed;
            giftCard.Code = code;
            giftCard.CreatedOn = createdOn;
            giftCard.ExpiresOn = expiresOn;
            giftCard.Currency = currency;
            giftCard.Enabled = enabled;

            return giftCard;
        }

        private static IRepository<Entities.GiftCard> GetGiftCardRepositoryStub(IList<Entities.GiftCard> giftCardsToReturn)
        {
            var dummyGiftCardRepository = MockRepository.GenerateStub<IRepository<Entities.GiftCard>>();
            dummyGiftCardRepository.Stub(x => x.Select()).Return(giftCardsToReturn.AsQueryable());

            return dummyGiftCardRepository;
        }

        private IRepository<PaymentStatus> GetPaymentStatusRepo()
        {
            var acquiredStatus = new PaymentStatus();
            var paymentStatusRepositoryStub = MockRepository.GenerateStub<IRepository<PaymentStatus>>();
            paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Acquired)).Return(acquiredStatus);    
        
            return paymentStatusRepositoryStub;
        } 
    }
}
