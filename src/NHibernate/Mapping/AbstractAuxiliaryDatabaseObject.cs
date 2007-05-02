using System;
using System.Collections;

using Iesi.Collections;
using NHibernate.Engine;

namespace NHibernate.Mapping
{
	public abstract class AbstractAuxiliaryDatabaseObject : IAuxiliaryDatabaseObject
	{
		private readonly HashedSet dialectScopes;

		protected AbstractAuxiliaryDatabaseObject()
		{
			dialectScopes = new HashedSet();
		}

		protected AbstractAuxiliaryDatabaseObject(HashedSet dialectScopes)
		{
			this.dialectScopes = dialectScopes;
		}

		public void AddDialectScope(string dialectName)
		{
			dialectScopes.Add(dialectName);
		}

		public HashedSet DialectScopes
		{
			get { return dialectScopes; }
		}

		public bool AppliesToDialect(Dialect.Dialect dialect)
		{
			// empty means no scoping
			return dialectScopes.IsEmpty || dialectScopes.Contains(dialect.GetType().FullName);
		}

		public abstract string SqlCreateString(Dialect.Dialect dialect, IMapping p, string defaultSchema);
		public abstract string SqlDropString(Dialect.Dialect dialect, string defaultSchema);

		public virtual void SetParameterValues(IDictionary parameters)
		{
		}
	}
}