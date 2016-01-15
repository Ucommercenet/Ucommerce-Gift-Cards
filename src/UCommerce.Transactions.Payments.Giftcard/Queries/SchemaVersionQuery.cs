using System.Collections.Generic;
using UCommerce.EntitiesV2.Queries;

namespace SampleApp.Extensions.Queries
{
	/// <summary>
	/// Resposible for getting the schema version from the database. 
	/// </summary>
	public class SchemaVersionQuery : ICannedQuery<int>
	{
		public IEnumerable<int> Execute(NHibernate.ISession session)
		{
			//CreateSQLQuery allows to execute standard TSQL through NHibernate APIs
			return session.CreateSQLQuery("SELECT SchemaVersion FROM uCommerce_SystemVersion").List<int>();
		}
	}
}
