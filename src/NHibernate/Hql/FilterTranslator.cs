using System.Collections;
using System.Runtime.CompilerServices;
using NHibernate.Engine;

namespace NHibernate.Hql
{
	/// <summary></summary>
	public class FilterTranslator : QueryTranslator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="d"></param>
		public FilterTranslator( Dialect.Dialect d ) : base( d )
		{
		}

		/// <summary>
		/// Compile a filter. This method may be called multiple
		/// times. Subsequent invocations are no-ops.
		/// </summary>
		[MethodImpl( MethodImplOptions.Synchronized )]
		public void Compile( string collectionRole, ISessionFactoryImplementor factory, string queryString, IDictionary replacements, bool scalar )
		{
			if( !Compiled )
			{
				this.factory = factory; // yick!
				AddFromCollection( "this", collectionRole );
				base.Compile( factory, queryString, replacements, scalar );
			}
		}
	}
}