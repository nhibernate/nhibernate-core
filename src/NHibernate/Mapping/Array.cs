using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An array has a primary key consisting of the key columns + index column
	/// </summary>
	public class Array : List
	{
		private System.Type elementClass;

		public Array( PersistentClass owner ) : base( owner )
		{
		}

		public System.Type ElementClass
		{
			get { return elementClass; }
			set { elementClass = value; }
		}

		public override CollectionType DefaultCollectionType
		{
			get { return TypeFactory.Array( Role, ReferencedPropertyName, ElementClass ); }
		}

		public override bool IsArray
		{
			get { return true; }
		}
	}
}