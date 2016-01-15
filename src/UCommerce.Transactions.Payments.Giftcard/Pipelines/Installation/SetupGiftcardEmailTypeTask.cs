using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.Giftcard.Pipelines.Installation
{
    /// <summary>
    /// Set up <see cref="EmailType"/> to use to notify the customer
    /// that a new gift card was created.
    /// </summary>
    public class SetupGiftCardEmailTypeTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<EmailType> _emailTypeRepository;

        public SetupGiftCardEmailTypeTask(IRepository<EmailType> emailTypeRepository)
        {
			_emailTypeRepository = emailTypeRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            if (_emailTypeRepository.Select().Any(x => x.Name == Constants.GiftCardEmailTypeName))
                return PipelineExecutionResult.Success;

            var emailType = new EmailType
            {
                Name = Constants.GiftCardEmailTypeName,
				Description = "E-mail which will be sent to the customer with the gift card code after order is completed."

			};

            _emailTypeRepository.Save(emailType);

            return PipelineExecutionResult.Success;
        }
    }
}
