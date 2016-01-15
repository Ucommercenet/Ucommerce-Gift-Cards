using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.Giftcard.Pipelines.Installation
{
    /// <summary>
    /// Set up <see cref="UCommerce.Transactions.Payments.Giftcard"/> definition for products />
    /// </summary>
    public class SetupGiftCardDefinitionTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<ProductDefinition> _productDefinitionRepository;

        public SetupGiftCardDefinitionTask(IRepository<ProductDefinition> productDefinitionRepository)
        {
            _productDefinitionRepository = productDefinitionRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            if (_productDefinitionRepository.Select().Any(x => x.Name == Constants.GiftCardProductDefinition))
                return PipelineExecutionResult.Success;

            var productDefinition = new ProductDefinition
            {
                Name = Constants.GiftCardProductDefinition,
				Guid = Constants.GiftCardProductDefinitionGuid
            };

            _productDefinitionRepository.Save(productDefinition);

            return PipelineExecutionResult.Success;
        }
    }
}
