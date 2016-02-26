using System;
using System.Collections.Generic;
using System.IO;
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
	public class MigrateAppDatabase : UCommerce.Pipelines.IPipelineTask<UCommerce.Pipelines.Initialization.InitializeArgs>
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
			//Find the virtual path to the uCommerce root folder	
			string webpathToUCommerceRoot = _pathService.GetPath();

			//Map to the physical path
			string physicalPathToUCommerce = HostingEnvironment.MapPath(webpathToUCommerceRoot);

			//Join the sub folder where our migration scripts are located
			string pathToApp = Path.Combine(physicalPathToUCommerce, @"Apps\UCommerce.GiftCards\Database");

			//Use MigrationLoader to get the migrations
			IList<UCommerce.Installer.Migration> migrations = new UCommerce.Installer.MigrationLoader().GetDatabaseMigrations(new DirectoryInfo(pathToApp));

			//Create a new instance of the database installer
			var appsDatabaseInstaller = new UCommerce.Installer.AppsDatabaseInstaller(
					new UCommerce.Infrastructure.Configuration.CommerceConfigurationConnectionStringLocator(),
					migrations,
					new UCommerce.Infrastructure.Logging.InstallerLoggingServiceAdapter(_loggingService));

			//Run the actual migration scripts
			appsDatabaseInstaller.InstallDatabase();

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
