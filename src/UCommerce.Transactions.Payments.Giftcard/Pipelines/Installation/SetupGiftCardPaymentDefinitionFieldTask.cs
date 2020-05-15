using System;
using System.Linq;
using Ucommerce.EntitiesV2;
using Ucommerce.Infrastructure.Logging;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
    public class SetupGiftCardPaymentDefinitionFieldTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<Definition> _definitionRepository;
        private readonly IRepository<DataType> _dataTypeRepository;

        public SetupGiftCardPaymentDefinitionFieldTask(
            IRepository<Definition> definitionRepository,
            IRepository<DataType> dataTypeRepository)
        {
            _definitionRepository = definitionRepository;
            _dataTypeRepository = dataTypeRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var giftCardPaymentDefinition = _definitionRepository.Select().FirstOrDefault(x => x.Name == Constants.GiftCardPaymentMethodName);

            if (giftCardPaymentDefinition == null)
                return PipelineExecutionResult.Success;

            if (giftCardPaymentDefinition.GetDefinitionFields().Any(x => x.Name == Constants.GiftCardPaymentDefinitionFieldAcceptUrl))
                return PipelineExecutionResult.Success;

            var giftCardPaymentDefinitionField = new DefinitionField()
            {
                DataType = _dataTypeRepository.Select().First(),
                Definition = giftCardPaymentDefinition,
                Name = Constants.GiftCardPaymentDefinitionFieldAcceptUrl,
                DisplayOnSite = true,
                Multilingual = false,
                RenderInEditor = true,
                Searchable = false,
                SortOrder = 1,
                Deleted = false,
                BuiltIn = true,
                DefaultValue = null
            };

            giftCardPaymentDefinition.AddDefinitionField(giftCardPaymentDefinitionField);
            giftCardPaymentDefinition.Save();
            return PipelineExecutionResult.Success;
        }
    }
}
