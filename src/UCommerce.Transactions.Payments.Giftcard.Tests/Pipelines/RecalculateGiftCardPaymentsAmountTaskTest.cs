using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines.Test.GiftCards;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Pipelines;

namespace UCommerce.Transactions.Payments.Giftcard.Tests.Pipelines
{
    [TestFixture]
    public class RecalculateGiftCardPaymentsAmountTaskTest
    {
        public RecalculateGiftCardPaymentsAmountTaskTest()
        {
            
        }

        private Payment CreateTestPayment(decimal paymentAmount, PaymentStatus status, bool isGiftCard, string transactionId)
        {
            var payment = new Payment
                {
                    Amount = paymentAmount,
                    PaymentStatus = status,
                    TransactionId = transactionId+"1124"
                };

            if (isGiftCard)
            {
				payment["IsGiftCard"] = "True";
				payment["GiftCardCode"] = transactionId;
            }

            return payment;
        } 

        private PurchaseOrder CreateTestPurchaseOrder(decimal orderAmount)
        {
            var order = new PurchaseOrder
                {
                    OrderTotal = orderAmount
                };

            return order;
        }

        private GiftCard CreateTestGiftCard(decimal amount, decimal amountUsed, string code)
        {
            var giftCard = new GiftCard
                {
                    Amount = amount,
                    AmountUsed = amountUsed,
                    Code = code
                };

            return giftCard;
        }

		[Test]
		public void Task_Can_Recalculate_A_Single_Payment_On_Order()
		{
			//ARANGE 
			var giftCardRepositoryStub = MockRepository.GenerateStub<IRepository<GiftCard>>();
			var paymentStatusRepositoryStub = MockRepository.GenerateStub<IRepository<PaymentStatus>>();

			var acquiredStatus = new PaymentStatus();
			var authorizedStatus = new PaymentStatus();
			var pending = new PaymentStatus();

			var giftCard1 = CreateTestGiftCard(500, 000, "GC-1");

			giftCardRepositoryStub.Stub(x => x.Select()).Return(new List<GiftCard> { giftCard1 }.AsQueryable());

			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Acquired)).Return(acquiredStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Authorized)).Return(authorizedStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.PendingAuthorization)).Return(pending);

			var testOrder = CreateTestPurchaseOrder(500);
			testOrder.AddPayment(CreateTestPayment(0, pending, true, giftCard1.Code));

			var task = new RecalculateGiftCardPaymentsAmountTask(giftCardRepositoryStub);

			//ACT
			task.Execute(testOrder);

			//ASSERT
			Assert.AreEqual(500, testOrder.Payments.Single().Amount);
			
		}

		[Test]
		public void Task_Can_Recalculate_Multiple_Payments_On_Order()
		{
			//ARANGE 
			var giftCardRepositoryStub = MockRepository.GenerateStub<IRepository<GiftCard>>();
			var paymentStatusRepositoryStub = MockRepository.GenerateStub<IRepository<PaymentStatus>>();

			var acquiredStatus = new PaymentStatus();
			var authorizedStatus = new PaymentStatus();
			var pending = new PaymentStatus();

			var giftCard1 = CreateTestGiftCard(600, 000, "GC-1");
			var giftCard2 = CreateTestGiftCard(500, 000, "GC-2");
			
			giftCardRepositoryStub.Stub(x => x.Select()).Return(new List<GiftCard> { giftCard1, giftCard2 }.AsQueryable());

			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Acquired)).Return(acquiredStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Authorized)).Return(authorizedStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.PendingAuthorization)).Return(pending);

			var testOrder = CreateTestPurchaseOrder(new decimal(1000.2535));

			var payment = CreateTestPayment(0, pending, true, giftCard1.Code);

			testOrder.AddPayment(payment);
			testOrder.AddPayment(CreateTestPayment(0, pending, true, giftCard2.Code));

			var task = new RecalculateGiftCardPaymentsAmountTask(giftCardRepositoryStub);

			//ACT
			task.Execute(testOrder);

			//ASSERT
			Assert.AreEqual(new decimal(1000.2535), testOrder.Payments.Sum(x => x.Amount));
			Assert.AreEqual(new decimal(500.2535), payment.Amount);

		}

		[Test]
		public void Task_Ignores_Non_Gift_Gift_Payments_On_Order_That_Are_Not_Auth_Or_Acquired()
		{
			//ARANGE 
			var giftCardRepositoryStub = MockRepository.GenerateStub<IRepository<GiftCard>>();
			var paymentStatusRepositoryStub = MockRepository.GenerateStub<IRepository<PaymentStatus>>();

			var acquiredStatus = new PaymentStatusSpy((int)PaymentStatusCode.Acquired);
			var authorizedStatus = new PaymentStatusSpy((int)PaymentStatusCode.Authorized);
			var pending = new PaymentStatusSpy((int)PaymentStatusCode.PendingAuthorization);

			var giftCard1 = CreateTestGiftCard(1000, 100, "GC-1");

			giftCardRepositoryStub.Stub(x => x.Select()).Return(new List<GiftCard> { giftCard1 }.AsQueryable());

			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Acquired)).Return(acquiredStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Authorized)).Return(authorizedStatus);
			paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.PendingAuthorization)).Return(pending);

			var testOrder = CreateTestPurchaseOrder(1200);
			testOrder.AddPayment(CreateTestPayment(500, pending, false, "What-ever-transaction-id"));
			testOrder.AddPayment(CreateTestPayment(300, authorizedStatus, false, "another-What-ever-transaction-id"));
			testOrder.AddPayment(CreateTestPayment(0, pending, true, giftCard1.Code));

			var task = new RecalculateGiftCardPaymentsAmountTask(giftCardRepositoryStub);

			//ACT
			task.Execute(testOrder);

			//ASSERT
			Assert.AreEqual(900, testOrder.Payments.First(x => x["GiftCardCode"] == "GC-1").Amount);
		}

        [Test]
        public void Task_Will_Set_Remaining_Payments_On_Order_To_Zero_If_Order_Total_Are_Covered()
        {
            //ARANGE 
            var giftCardRepositoryStub = MockRepository.GenerateStub<IRepository<GiftCard>>();
            var paymentStatusRepositoryStub = MockRepository.GenerateStub<IRepository<PaymentStatus>>();
            
            var acquiredStatus = new PaymentStatus();
            var authorizedStatus = new PaymentStatus();
            var pending = new PaymentStatus();

            var giftCard1 = CreateTestGiftCard(200, 100, "GC-1");
            var giftCard2 = CreateTestGiftCard(300, 100, "GC-2");
            var giftCard3 = CreateTestGiftCard(800, 100, "GC-3");
            var giftCard4 = CreateTestGiftCard(800, 100, "GC-4");

            giftCardRepositoryStub.Save(giftCard1);
            giftCardRepositoryStub.Save(giftCard2);
            giftCardRepositoryStub.Save(giftCard3);
            giftCardRepositoryStub.Save(giftCard4);
			
            giftCardRepositoryStub.Stub(x => x.Select()).Return(new List<GiftCard> { giftCard1, giftCard2, giftCard3, giftCard4 }.AsQueryable());
			
            paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Acquired)).Return(acquiredStatus);
            paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.Authorized)).Return(authorizedStatus);
            paymentStatusRepositoryStub.Stub(x => x.Get((int)PaymentStatusCode.PendingAuthorization)).Return(pending);
            
            var testOrder = CreateTestPurchaseOrder(900);
            testOrder.AddPayment(CreateTestPayment(100, acquiredStatus, false, "what-ever-transaction-id"));
            testOrder.AddPayment(CreateTestPayment(100, authorizedStatus, false, "what-ever-transaction-id"));

            testOrder.AddPayment(CreateTestPayment(200, pending, true, giftCard1.Code));
            testOrder.AddPayment(CreateTestPayment(200, pending, true, giftCard2.Code));
            testOrder.AddPayment(CreateTestPayment(200, pending, true, giftCard3.Code));
            testOrder.AddPayment(CreateTestPayment(200, pending, true, giftCard4.Code));

            var task = new RecalculateGiftCardPaymentsAmountTask(giftCardRepositoryStub);

            //ACT
            task.Execute(testOrder);

            //ASSERT
        	Assert.True(testOrder.Payments.Any(x => x.Amount == 0));
        }
    }
}
