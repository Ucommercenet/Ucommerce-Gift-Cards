using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ucommerce.EntitiesV2;
using Ucommerce.Pipelines;
using Ucommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
    public class SetupGiftCardDefinitionFieldTask : IPipelineTask<InitializeArgs>
    {
        private readonly IRepository<ProductDefinition> _productDefinitionRepository;
        private readonly IRepository<DataType> _dataTypeRepository;

        public SetupGiftCardDefinitionFieldTask(IRepository<ProductDefinition> productDefinitionRepository, IRepository<DataType> dataTypeRepository)
        {
            _productDefinitionRepository = productDefinitionRepository;
            _dataTypeRepository = dataTypeRepository;
        }

        public PipelineExecutionResult Execute(InitializeArgs subject)
        {
            var giftCardProductDefinition = _productDefinitionRepository.Select().FirstOrDefault(x => x.Name == Constants.GiftCardProductDefinition);

            if (giftCardProductDefinition == null)
                return PipelineExecutionResult.Success;

            if(giftCardProductDefinition.ProductDefinitionFields.Any(x => x.Name == Constants.GiftCardProductDefinitionField))
                return PipelineExecutionResult.Success;

            var giftCardDefinitionField = new ProductDefinitionField()
            {
                Name = Constants.GiftCardProductDefinitionField,
                DataType = _dataTypeRepository.Select().First(),
                Deleted = false,
                DisplayOnSite = false,
                Facet = false,
                IsVariantProperty = true,
                Multilingual = false,
                ProductDefinition = giftCardProductDefinition,
                RenderInEditor = false
            };

            giftCardProductDefinition.AddProductDefinitionField(giftCardDefinitionField);
            giftCardProductDefinition.Save();

            return PipelineExecutionResult.Success;
        }
    }
}
