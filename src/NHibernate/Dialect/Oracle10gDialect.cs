using NHibernate.SqlCommand;

namespace NHibernate.Dialect
{
	/// <summary> 
	/// A dialect specifically for use with Oracle 10g.
	/// </summary>
	/// <remarks>
	/// The main difference between this dialect and <see cref="Oracle9iDialect"/>
	/// is the use of "ANSI join syntax" here...
	/// </remarks>
	public class Oracle10gDialect : Oracle9iDialect
	{
		public override JoinFragment CreateOuterJoinFragment()
		{
			return new ANSIJoinFragment();
		}
	}
}