using System;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
    public class SetupGiftCardPaymentDefinitionFieldTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<Definition> _definitionRepository;
        private readonly IRepository<DataType> _dataTypeRepository;
        private readonly ILoggingService _logging;

        public SetupGiftCardPaymentDefinitionFieldTask(
            IRepository<Definition> definitionRepository,
            IRepository<DataType> dataTypeRepository,
            ILoggingService logging)
        {
            _definitionRepository = definitionRepository;
            _dataTypeRepository = dataTypeRepository;
            _logging = logging;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            _logging.Log<String>("LESSGO PaymentDefinitionFieldTask step");

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

            _logging.Log<String>("LESSGO PaymentDefinitionFieldTask created");
            _logging.Log<String>(giftCardPaymentDefinitionField.ToString());

            giftCardPaymentDefinition.AddDefinitionField(giftCardPaymentDefinitionField);
            giftCardPaymentDefinition.Save();

            _logging.Log<String>("LESSGO PaymentDefinitionFieldTask added");

            return PipelineExecutionResult.Success;
        }
    }
}
