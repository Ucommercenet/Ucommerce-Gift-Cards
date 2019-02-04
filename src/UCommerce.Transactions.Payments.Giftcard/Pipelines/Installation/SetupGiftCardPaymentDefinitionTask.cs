using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
    /// <summary>
    /// Set up <see cref="UCommerce.Transactions.Payments.GiftCard"/> definition for payment definition/>
    /// </summary>
    public class SetupGiftCardPaymentDefinitionTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<Definition> _definitionRepository;
        private readonly IRepository<DefinitionType> _definitionTypeRepository;

        public SetupGiftCardPaymentDefinitionTask(
            IRepository<Definition> definitionRepository,
            IRepository<DefinitionType> definitionTypeRepository)
        {
            _definitionRepository = definitionRepository;
            _definitionTypeRepository = definitionTypeRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            if (_definitionRepository.Select().Any(x => x.Name == Constants.GiftCardPaymentMethodName))
                return PipelineExecutionResult.Success;

            var definition = new Definition
            {
                Name = Constants.GiftCardPaymentMethodName,
                Description = "Definition for GiftCard payment method",
                Guid = Guid.NewGuid(),
                DefinitionType = _definitionTypeRepository.Select(x => x.Name == Constants.GiftCardPaymentDefinitionName).FirstOrDefault()
            };

            _definitionRepository.Save(definition);
            return PipelineExecutionResult.Success;
        }
    }
}
