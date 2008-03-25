using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary> 
	/// Convenience base class for <see cref="IAuxiliaryDatabaseObject">AuxiliaryDatabaseObjects</see>.
	/// </summary>
	/// <remarks>
	/// This implementation performs dialect scoping checks strictly based on
	/// dialect name comparisons.  Custom implementations might want to do
	/// instanceof-type checks. 
	/// </remarks>
	[Serializable]
	public abstract class AbstractAuxiliaryDatabaseObject : IAuxiliaryDatabaseObject
	{
		private readonly HashedSet<string> dialectScopes;

		protected AbstractAuxiliaryDatabaseObject()
		{
			dialectScopes = new HashedSet<string>();
		}

		protected AbstractAuxiliaryDatabaseObject(HashedSet<string> dialectScopes)
		{
			this.dialectScopes = dialectScopes;
		}

		public void AddDialectScope(string dialectName)
		{
			dialectScopes.Add(dialectName);
		}

		public HashedSet<string> DialectScopes
		{
			get { return dialectScopes; }
		}

		public bool AppliesToDialect(Dialect.Dialect dialect)
		{
			// empty means no scoping
			return dialectScopes.IsEmpty || dialectScopes.Contains(dialect.GetType().FullName);
		}

		public abstract string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultCatalog, string defaultSchema);
		public abstract string SqlDropString(Dialect.Dialect dialect, string defaultCatalog, string defaultSchema);

		public void SetParameterValues(IDictionary<string, string> parameters) {}
	}
}