using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;
using UCommerce.Transactions.Payments.GiftCard.Entities;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
	/// <summary>
	/// Set up the required payment method for <see cref="GiftCard" /> products
	/// to make gift cards sellable.
	/// </summary>
	public class SetupGiftCardPaymentMethod : IPipelineTask<InitializeArgs>
	{
		private readonly IRepository<PaymentMethod> _paymentMethodRepository;

		public SetupGiftCardPaymentMethod(IRepository<PaymentMethod> paymentMethodRepository)
		{
			_paymentMethodRepository = paymentMethodRepository;
		}

		public PipelineExecutionResult Execute(InitializeArgs subject)
		{
			if (_paymentMethodRepository.Select().Any(x => x.Name == Constants.GiftCardPaymentMethodName))
				return PipelineExecutionResult.Success;
			
			var paymentMethod = new PaymentMethod
			{
				Name = Constants.GiftCardPaymentMethodName,
				PaymentMethodServiceName = Constants.GiftCardPaymentMethodName,
				Enabled = true,
                Pipeline = "Checkout"
			};

			_paymentMethodRepository.Save(paymentMethod);
			return PipelineExecutionResult.Success;
		}
	}
}