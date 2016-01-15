using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Pipelines;

namespace SampleApp.Extensions.Pipelines.ProductDefinition
{
	/// <summary>
	/// Save the new ProductDefintion 
	/// if ProductDefinition.Name dosn't exist. 
	/// </summary>
	public class SaveShoeDefinitionTask : IPipelineTask<UCommerce.EntitiesV2.ProductDefinition>
	{
		private readonly IRepository<UCommerce.EntitiesV2.ProductDefinition> _productDefinitionRepository;
		private readonly IPipeline<IDefinition> _saveDefinitionPipeline;

		public SaveShoeDefinitionTask(IRepository<UCommerce.EntitiesV2.ProductDefinition> productDefinitionRepository,
			IPipeline<IDefinition> saveDefinitionPipeline)
		{
			_productDefinitionRepository = productDefinitionRepository;
			_saveDefinitionPipeline = saveDefinitionPipeline;
		}

		public PipelineExecutionResult Execute(UCommerce.EntitiesV2.ProductDefinition subject)
		{
			if (_productDefinitionRepository.Select().Any(x => x.Name == subject.Name)) return PipelineExecutionResult.Success;
						
			_saveDefinitionPipeline.Execute(subject);

			return PipelineExecutionResult.Success;
		}
	}
}
