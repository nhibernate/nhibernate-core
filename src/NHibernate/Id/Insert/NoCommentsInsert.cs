using NHibernate.Engine;
using NHibernate.SqlCommand;

namespace NHibernate.Id.Insert
{
	/// <summary>
	/// Disable comments on insert.
	/// </summary>
	public class NoCommentsInsert : IdentifierGeneratingInsert
	{
		public NoCommentsInsert(ISessionFactoryImplementor factory) : base(factory) {}
		public override SqlInsertBuilder SetComment(string comment)
		{
			return this;
		}
	}
}