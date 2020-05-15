using NUnit.Framework;
using Ucommerce.EntitiesV2;

namespace UCommerce.Transactions.Payments.GiftCard.Tests.Pipelines
{
	[TestFixture]
	public class SendGiftCardEmailTaskTest
	{
		[Test]
		public void Task_Can_FallBack_To_BillingAddress_If_No_Shipments_Are_Present()
		{
			var expectedEmail = "whatever@billingaddress.com";
			var order = new PurchaseOrder();
			order.BillingAddress = new OrderAddress { EmailAddress = expectedEmail };

			var task = new SendGiftCardEmailTaskSpy(null, null, null, null);

			var email = task.GetSenderEmail(order);

			Assert.AreEqual(expectedEmail, email);
		}

		[Test]
		public void Task_Can_FallBack_To_BillingAddress_If_More_Than_One_Shipments_Are_Present()
		{
			var expectedEmail = "whatever@billingaddress.com";
			var order = new PurchaseOrder();
			order.BillingAddress = new OrderAddress { EmailAddress = expectedEmail };
			order.AddShipment(new Shipment { ShipmentAddress = new OrderAddress { EmailAddress = "whatever@shipment.com"}});
			order.AddShipment(new Shipment { ShipmentAddress = new OrderAddress { EmailAddress = "whatever1@shipment.com" } });


			var task = new SendGiftCardEmailTaskSpy(null, null, null, null);

			var email = task.GetSenderEmail(order);

			Assert.AreEqual(expectedEmail, email);
		}

		[Test]
		public void Task_Can_FallBack_To_BillingAddress_If_No_Email_Are_Configured_On_ShipmentAddress()
		{
			var expectedEmail = "whatever@billingaddress.com";
			var order = new PurchaseOrder();
			order.BillingAddress = new OrderAddress { EmailAddress = expectedEmail };
			order.AddShipment(new Shipment { ShipmentAddress = new OrderAddress() });
			
			var task = new SendGiftCardEmailTaskSpy(null, null, null, null);

			var email = task.GetSenderEmail(order);

			Assert.AreEqual(expectedEmail, email);
		}

		[Test]
		public void Task_Can_FallBack_To_BillingAddress_If_No_ShipmentAddress_Are_Configured_On_Shipment()
		{
			var expectedEmail = "whatever@billingaddress.com";
			var order = new PurchaseOrder();
			order.BillingAddress = new OrderAddress { EmailAddress = expectedEmail };
			order.AddShipment(new Shipment());
			
			var task = new SendGiftCardEmailTaskSpy(null, null, null, null);

			var email = task.GetSenderEmail(order);

			Assert.AreEqual(expectedEmail, email);
		}

		[Test]
		public void Task_Can_Use_Shipment_Address_If_Exactly_One_Are_Configured_On_Order()
		{
			var expectedEmail = "ThisEmailShouldBe@used.com";
			var redundantemail = "whatever@billingaddress.com";

			var order = new PurchaseOrder();
			order.BillingAddress = new OrderAddress { EmailAddress = redundantemail };
			order.AddShipment(new Shipment { ShipmentAddress = new OrderAddress { EmailAddress = expectedEmail } });
			
			var task = new SendGiftCardEmailTaskSpy(null, null, null, null);

			var email = task.GetSenderEmail(order);

			Assert.AreEqual(expectedEmail, email);
		}
	}
}
