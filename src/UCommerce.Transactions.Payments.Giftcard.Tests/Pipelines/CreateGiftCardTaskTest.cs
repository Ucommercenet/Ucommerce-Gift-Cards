using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using UCommerce.EntitiesV2;
using UCommerce.Marketing.Targets;
using UCommerce.Transactions.Payments.Giftcard.Entities;
using UCommerce.Transactions.Payments.Giftcard.Pipelines;
using UCommerce.Transactions.Payments.Giftcard.Services;

namespace UCommerce.Transactions.Payments.Giftcard.Tests.Pipelines
{
	[TestFixture]
	public class CreateGiftCardTaskTest
	{
		/// <summary>
		/// Creates a purchase order with one order line with quantity of 4,
		/// which should trigger creation of 4 gift cards.
		/// </summary>
		[Test]
		public void PipeLineTask_Can_Generate_GiftCards()
		{
				//Arrange

				var currency = new Currency();

				var giftCardServiceMock = GetGiftCardServiceStub();

				//Setup expectation that CreateGiftCards with any state of the parameters was never called.
				var productRepositoryStub = GetProductRepositoryStub();

				giftCardServiceMock.Expect(x => x.IssueGiftCards(null)).Repeat.Times(1).Return(null);

				var pricegroup = new PriceGroup();
				var product = CreateTestProduct(Constants.GiftCardProductDefinition, pricegroup, 400, currency);
				var order = CreateTestOrder(product, currency, pricegroup, 200);


				productRepositoryStub.Stub(x => x.Select()).Return(new List<Product> { product }.AsQueryable());

				var task = new IssueGiftCardTaskSpy(giftCardServiceMock, productRepositoryStub, 20, product);
				
				//Act
				task.Execute(order);

				//Assert

				//The mock expects that CreateGiftCards was called two times.
				giftCardServiceMock.VerifyAllExpectations();
		}

        [Test]
        public void CreateGiftCardTask_Can_Find_OrderLines_With_GiftCards()
        {
            //Arrange
            var productRepositoryStub = GetProductRepositoryStub();

            var currency = new Currency();
            var pricegroup = new PriceGroup();
            var product = CreateTestProduct(
                productDefinitionName: "GiftCard",
                priceGroup: pricegroup,
                price: 400,
                currency: currency);

            var order = CreateTestOrder(product, currency, pricegroup, 200);

            productRepositoryStub.Stub(x => x.Select()).Return(new List<Product> { product }.AsQueryable());

            var taskSpy = new IssueGiftCardTaskSpy(GetGiftCardServiceStub(), productRepositoryStub,20,product);

            //act
            var orderLines = taskSpy.GetOrderLinesWithGiftCard(order);

            //assert
			Assert.IsTrue(orderLines.Any());
        }

		[Test]
		public void Price_Paid_Including_Tax_Is_Equal_To_GiftCard_Value()
		{
			//Arrange

			var productRepositoryStub = GetProductRepositoryStub();

			var currency = new Currency();
			var pricegroup = new PriceGroup();
			var product = CreateTestProduct(
				productDefinitionName: "GiftCard",
				priceGroup: pricegroup,
				price: 400,
				currency: currency);

			var order = CreateTestOrder(product, currency, pricegroup,200);

			productRepositoryStub.Stub(x => x.Select()).Return(new List<Product> { product }.AsQueryable());

			var taskSpy = new IssueGiftCardTaskSpy(GetGiftCardServiceStub(), productRepositoryStub, 20, product);

			//Act
			taskSpy.Execute(order);

			Assert.AreEqual(taskSpy.IssueGiftCardRequests.Count,8);
			//we expect 4800 in amount as there's 2 orderLines with 4 giftcards of value 400 and the tax is set to 200
			//hence 2*4*(400+200) = 4800
			Assert.AreEqual(taskSpy.IssueGiftCardRequests.Sum(x=> x.Amount.Value),4800);
		}

		[Test]
		public void PurchaseOrder_With_No_GiftCard_OrderLines_Does_Not_Generate_GiftCards()
		{
			//Arrange
		
            var giftCardServiceStub = GetGiftCardServiceStub();

            //setup expectation that CreateGiftCards with any state of the parameters was never called
            giftCardServiceStub.Expect(x => x.IssueGiftCards(null)).Repeat.Never();
			var productRepositoryStub = GetProductRepositoryStub();
            

			var currency = new Currency();
			var pricegroup = new PriceGroup();
			var product = CreateTestProduct("whateverDefinition", pricegroup, 400, currency);
			var order = CreateTestOrder(product, currency, pricegroup,200);


            productRepositoryStub.Stub(x => x.Select()).Return(new List<Product> { product }.AsQueryable());

            var task = new IssueGiftCardTask(20, productRepositoryStub, giftCardServiceStub);

			//Act
            task.Execute(order);
		    
            //Assert

            //the stub expects that CreateGiftCards was never called.
            giftCardServiceStub.VerifyAllExpectations();
		}

        private IGiftCardService GetGiftCardServiceStub()
        {
            var voucherGenerator = new DefaultVoucherCodeGenerator();

            return MockRepository.GenerateMock<GiftCardService>(new object[] { voucherGenerator, GetGiftCardRepositoryStub() });
        }

        private IRepository<Product> GetProductRepositoryStub()
        {
            return MockRepository.GenerateStub<IRepository<Product>>();
        }

        private IRepository<GiftCard> GetGiftCardRepositoryStub()
        {
            return MockRepository.GenerateStub<IRepository<GiftCard>>();
        } 

		private Product CreateTestProduct(string productDefinitionName, PriceGroup priceGroup, decimal price, Currency currency)
		{
			priceGroup.Currency = currency;

			var productDefinition = new ProductDefinition();
			productDefinition.Name = productDefinitionName;

			var priceGroupPrice = new PriceGroupPrice { Price = price, PriceGroup = priceGroup };

			var product = new Product();
			product.Sku = "WhateverSku";
			product.AddPriceGroupPrice(priceGroupPrice);
			product.ProductDefinition = productDefinition;

			return product;
		}

		private PurchaseOrder CreateTestOrder(Product product, Currency currency, PriceGroup priceGroup, decimal tax)
		{
			var purchaseOrder = new PurchaseOrder { BillingCurrency = currency };
			purchaseOrder.AddProduct(priceGroup, product, 4, product.GetPrice(priceGroup).Value, tax, false);
			purchaseOrder.AddProduct(priceGroup, product, 4, product.GetPrice(priceGroup).Value, tax, false);

			return purchaseOrder;
		}
	}
}
