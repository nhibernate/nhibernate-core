using NHibernate.Engine;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public interface IRelationalModel
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		string SqlCreateString( Dialect.Dialect dialect, IMapping p );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dialect"></param>
		/// <returns></returns>
		string SqlDropString( Dialect.Dialect dialect );
	}
}