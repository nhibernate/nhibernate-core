using NHibernate.Persister.Entity;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Engine
{
	internal interface IJoin
	{
		IJoinable Joinable { get;  }
		string[] LHSColumns { get; }
		string Alias { get;  }
		IAssociationType AssociationType { get;  }
		JoinType JoinType { get; }
	}
}
