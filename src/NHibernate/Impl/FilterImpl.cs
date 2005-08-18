using System.Collections;
using NHibernate.Engine;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Implementation of the <see cref="IQuery"/> interface for collection filters.
	/// </summary>
	internal class FilterImpl : QueryImpl
	{
		private object collection;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="queryString"></param>
		/// <param name="collection"></param>
		/// <param name="session"></param>
		public FilterImpl( string queryString, object collection, ISessionImplementor session ) : base( queryString, session )
		{
			this.collection = collection;
		}

		/// <summary></summary>
		public override IEnumerable Enumerable()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.EnumerableFilter( collection, BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		/// <summary></summary>
		public override IList List()
		{
			VerifyParameters();
			IDictionary namedParams = NamedParams;
			return Session.Filter( collection, BindParameterLists( namedParams ), GetQueryParameters( namedParams ) );
		}

		public override IType[ ] TypeArray()
		{
			IList typeList = Types;
			int size = typeList.Count;
			IType[ ] result = new IType[size + 1];
			for( int i = 0; i < size; i++ )
			{
				result[ i + 1 ] = ( IType ) typeList[ i ];
			}
			return result;
		}

		public override object[ ] ValueArray()
		{
			IList valueList = Values;
			int size = valueList.Count;
			object[ ] result = new object[size + 1];
			for( int i = 0; i < size; i++ )
			{
				result[ i + 1 ] = valueList[ i ];
			}
			return result;
		}
	}
}