using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Hosting;
using UCommerce.Infrastructure.Configuration;
using UCommerce.Infrastructure.Logging;
using UCommerce.Infrastructure.Runtime;
using UCommerce.Installer;
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
		private readonly IPathService _pathService;
		private readonly ILoggingService _loggingService;

		public MigrateAppDatabase(IPathService pathService, ILoggingService loggingService)
		{
			_pathService = pathService;
			_loggingService = loggingService;
		}

		public PipelineExecutionResult Execute(InitializeArgs subject)
		{
			string webpathToUCommerceRoot = _pathService.GetPath();
			string physicalPathToUCommerce = HostingEnvironment.MapPath(webpathToUCommerceRoot);

			string pathToApp = Path.Combine(physicalPathToUCommerce, @"Apps\UCommerce.GiftCards\Database");

			var migrations = new MigrationLoader().GetDatabaseMigrations(new DirectoryInfo(pathToApp));

			new AppsDatabaseInstaller(new CommerceConfigurationConnectionStringLocator(), migrations, new InstallerLoggingServiceAdapter(_loggingService))
				.InstallDatabase();

			// trigger data migration for app
			return PipelineExecutionResult.Success;
		}
	}

	public class InstallerLoggingService : IInstallerLoggingService
	{
		public void Log<T>(string customMessage)
		{
			
		}

		public void Log<T>(Exception exception, string customMessage)
		{
			
		}

		public void Log<T>(Exception exception)
		{
			
		}
	}
}
