using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// An array has a primary key consisting of the key columns + index column
	/// </summary>
	public class Array : List
	{
		private System.Type elementClass;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		public Array( PersistentClass owner ) : base( owner )
		{
		}

		/// <summary></summary>
		public System.Type ElementClass
		{
			get { return elementClass; }
			set { elementClass = value; }
		}

		/// <summary></summary>
		public override CollectionType CollectionType
		{
			get { return TypeFactory.Array( Role, ReferencedPropertyName, ElementClass ); }
		}

		/// <summary></summary>
		public override bool IsArray
		{
			get { return true; }
		}
	}
}