using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary> 
	/// Nothing more than a distinguishing subclass of Insert used to indicate
	/// intent.  
	/// Some subclasses of this also provided some additional
	/// functionality or semantic to the genernated SQL statement string.
	///  </summary>
	public class IdentifierGeneratingInsert : SqlInsertBuilder
	{
		public IdentifierGeneratingInsert(ISessionFactoryImplementor factory) : base(factory) { }
	}
}
