using NHibernate.Persister;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Collection
{
	/// <summary>
	/// Summary description for CollectionPropertyMapping.
	/// </summary>
	public class CollectionPropertyMapping : IPropertyMapping
	{
		/// <summary></summary>
		public const string CollectionSize = "size";
		/// <summary></summary>
		public const string CollectionElements = "elements";
		/// <summary></summary>
		public const string CollectionIndices = "indices";
		/// <summary></summary>
		public const string CollectionMaxIndex = "maxIndex";
		/// <summary></summary>
		public const string CollectionMinIndex = "minIndex";
		/// <summary></summary>
		public const string CollectionMaxElement = "maxElement";
		/// <summary></summary>
		public const string CollectionMinElement = "minElement";

		private readonly IQueryableCollection memberPersister;

		private const string InvalidPropertyMessage = "expecting 'elements' or 'indicies' after {0}";

		/// <summary>
		/// 
		/// </summary>
		/// <param name="memberPersister"></param>
		public CollectionPropertyMapping( IQueryableCollection memberPersister )
		{
			this.memberPersister = memberPersister;
		}

		/// <summary>
		/// 
		/// </summary>
		public IType Type
		{
			get { return memberPersister.CollectionType; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public IType ToType( string propertyName )
		{
			switch ( propertyName )
			{
				case CollectionElements:
				case CollectionMaxElement:
				case CollectionMinElement:
					return memberPersister.ElementType;

				case CollectionIndices:
				case CollectionMaxIndex:
				case CollectionMinIndex:
					CheckIndex( propertyName );
					return memberPersister.IndexType;

				case CollectionSize:
					return NHibernateUtil.Int32;

				default:
					throw new QueryException( string.Format( InvalidPropertyMessage, propertyName ) );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string[] ToColumns( string alias, string propertyName )
		{
			string[] cols;

			switch ( propertyName )
			{
				case CollectionElements:
					cols = memberPersister.ElementColumnNames;
					return StringHelper.Qualify( alias, cols );

				case CollectionIndices:
					CheckIndex( propertyName );
					cols = memberPersister.IndexColumnNames;
					return StringHelper.Qualify( alias, cols );

				case CollectionSize:
					return new string[] { "count(*)" };

				case CollectionMaxIndex:
					CheckIndex( propertyName );
					return ColumnFunction( propertyName, "max", memberPersister.IndexColumnNames ) ;

				case CollectionMinIndex:
					CheckIndex( propertyName );
					return ColumnFunction( propertyName, "min", memberPersister.IndexColumnNames ) ;

				case CollectionMaxElement:
					return ColumnFunction( propertyName, "max", memberPersister.IndexColumnNames ) ;

				case CollectionMinElement:
					return ColumnFunction( propertyName, "min", memberPersister.IndexColumnNames ) ;

				default:
					throw new QueryException( string.Format( InvalidPropertyMessage, propertyName ) );
			}
		}

		private void CheckIndex( string propertyName )
		{
			if ( !memberPersister.HasIndex )
			{
				throw new QueryException( string.Format( "unindexed collection before {0}", propertyName ) );
			}
		}
			
		private string[] ColumnFunction( string propertyName, string function, string[] cols )
		{
			if ( cols.Length !=1 ) 
			{
				throw new QueryException( string.Format( "composite collection element in {0}", propertyName ) );
			}
			return new string[] { function + StringHelper.OpenParen + cols[0] + StringHelper.ClosedParen };
		}
	}
}
