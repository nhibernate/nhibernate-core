using System;
using System.Collections;

namespace NHibernate.Mapping
{
	/// <summary>
	/// Simple holder for named sql queries
	/// </summary>
	public class NamedSQLQuery
	{
		private string query;
		private ArrayList aliasedClasses;
		private ArrayList synchronizedTables;
		private ArrayList aliases;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="query"></param>
		public NamedSQLQuery( string query )
		{
			this.aliases = new ArrayList();
			this.aliasedClasses = new ArrayList();
			this.query = query;
			this.synchronizedTables = new ArrayList();
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] ReturnAliases
		{
			get { return (string[]) aliases.ToArray( typeof( string[ ] ) ); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] ReturnClasses
		{
			get { return (string[]) aliasedClasses.ToArray( typeof( string[ ] ) ); }
		}

		/// <summary>
		/// 
		/// </summary>
		public IList SynchronizedTables
		{
			get { return synchronizedTables; }
		}

		/// <summary>
		/// 
		/// </summary>
		public string QueryString
		{
			get { return query; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public void AddSynchronizedTable( string table )
		{
			synchronizedTables.Add( table );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="clazz"></param>
		public void AddAliasedClass( string alias, object clazz)
		{
			aliases.Add( alias );
			aliasedClasses.Add( clazz );
		}
	}
}
