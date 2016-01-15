using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UCommerce.EntitiesV2;
using UCommerce.EntitiesV2.Definitions;
using UCommerce.Extensions;
using UCommerce.Pipelines;

namespace SampleApp.Extensions.Pipelines.ProductDefinition
{
	/// <summary>
	/// Modifies the ProductDefinition with content like
	/// Name, DefinitionFields and DefinitionFieldDescriptions. 
	/// </summary>
	public class ModifyShoeDefinitionTask : IPipelineTask<UCommerce.EntitiesV2.ProductDefinition>
	{
		private readonly IRepository<DataType> _dataTypeRepository;

		public ModifyShoeDefinitionTask(IRepository<DataType> dataTypeRepository)
		{
			_dataTypeRepository = dataTypeRepository;
		}

		public PipelineExecutionResult Execute(UCommerce.EntitiesV2.ProductDefinition subject)
		{
			subject.Name = "Shoe";

			var definitionsFields = CreateProductDefinitionFields();
			definitionsFields.ForEach(subject.AddProductDefinitionField);
			
			return PipelineExecutionResult.Success;
		}

		private IEnumerable<ProductDefinitionField> CreateProductDefinitionFields()
		{
			return new List<ProductDefinitionField>
			{
				CreateProductDefinitionField("Awesomeness", new BooleanDataTypeDefinition()),
				CreateProductDefinitionField("ShortStory", new ShortTextDataTypeDefinition())
			};
		}

		private ProductDefinitionField CreateProductDefinitionField(string name, DataTypeDefinition definition)
		{
			var productDefinitionField = new ProductDefinitionField()
			{
				DataType = _dataTypeRepository.Select().FirstOrDefault(x => x.DefinitionName == definition.Name),
				Name = name,
				RenderInEditor = true,
			};
			
			var descriptions = CreateDefinitionFieldsDescriptions(name);
			descriptions.ForEach(productDefinitionField.AddProductDefinitionFieldDescription);

			return productDefinitionField;
		}

		/// <summary>
		/// Creates multilingual descriptions for the product definition field. 
		/// </summary>
		/// <param name="displayName"></param>
		/// <returns></returns>
		private IEnumerable<ProductDefinitionFieldDescription> CreateDefinitionFieldsDescriptions(string displayName)
		{
			return new Collection<ProductDefinitionFieldDescription>
			{
				new ProductDefinitionFieldDescription
				{
					DisplayName = string.Format("{0}-US", displayName), 
					CultureCode = "en-US"
				},new ProductDefinitionFieldDescription
				{
					DisplayName = string.Format("{0}-GB", displayName), 
					CultureCode = "en-GB"
				}
			};
		}
	}
}
