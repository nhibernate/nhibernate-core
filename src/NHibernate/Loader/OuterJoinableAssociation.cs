using System;
using System.Collections;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Persister;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Loader
{
	public sealed class OuterJoinableAssociation // struct?
	{
		public IJoinable Joinable;
			
		// belong to other persister
		public string[ ] ForeignKeyColumns;
		public string Subalias;
		public string[] PrimaryKeyColumns;
		public string TableName;
			
		// the position of the persister we came from in the list
		public int Owner;
		public JoinType JoinType;
		public bool IsOneToOne;
	}

}