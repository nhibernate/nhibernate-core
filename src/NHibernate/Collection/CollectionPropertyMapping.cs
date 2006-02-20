using NHibernate.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Persister.Collection
{
	/// <summary>
	/// Summary description for CollectionPropertyMapping.
	/// </summary>
	public class CollectionPropertyMapping : IPropertyMapping
	{
		public const string CollectionSize = "size";
		public const string CollectionElements = "elements";
		public const string CollectionIndices = "indices";
		public const string CollectionMaxIndex = "maxIndex";
		public const string CollectionMinIndex = "minIndex";
		public const string CollectionMaxElement = "maxElement";
		public const string CollectionMinElement = "minElement";

		private readonly IQueryableCollection memberPersister;

		private const string InvalidPropertyMessage = "expecting 'elements' or 'indices' after {0}";

		public CollectionPropertyMapping( IQueryableCollection memberPersister )
		{
			this.memberPersister = memberPersister;
		}

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
					return ColumnFunction( propertyName, "max", memberPersister.ElementColumnNames ) ;

				case CollectionMinElement:
					return ColumnFunction( propertyName, "min", memberPersister.ElementColumnNames ) ;

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

		public IType Type
		{
			get { return memberPersister.CollectionType; }
		}
	}
}
