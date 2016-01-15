using UCommerce.Pipelines;
using UCommerce.Pipelines.Initialization;

namespace UCommerce.Transactions.Payments.GiftCard.Pipelines.Installation
{
	/// <summary>
	/// Migrates the app database according to the deployed migrations
	/// and the current schema version.
	/// </summary>
	public class MigrateAppDatabase : IPipelineTask<InitializeArgs>
	{
		public PipelineExecutionResult Execute(InitializeArgs subject)
		{
			// trigger data migration for app
			return PipelineExecutionResult.Success;
		}
	}
}
