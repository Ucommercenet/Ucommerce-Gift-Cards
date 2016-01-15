using UCommerce.Infrastructure.Logging;
using UCommerce.Pipelines;

namespace SampleApp.Extensions.Pipelines.ProductDefinition
{
	/// <summary>
	/// Pipeline responsible for tasks associated with maintaining data on specific definition during app start
	/// </summary>
	public class AddShoeDefinitionPipeline : Pipeline<UCommerce.EntitiesV2.ProductDefinition>
	{
		public AddShoeDefinitionPipeline(IPipelineTask<UCommerce.EntitiesV2.ProductDefinition>[] tasks, ILoggingService loggingService) : base(tasks, loggingService)
		{
		}
	}
}
