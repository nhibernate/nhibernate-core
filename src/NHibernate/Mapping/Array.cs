using System;
using NHibernate.Type;
using ArrayHolder=NHibernate.Collection.ArrayHolder;

namespace NHibernate.Mapping 
{
	/// <summary>
	/// An array has a primary key consisting of the key columns + index column
	/// </summary>
	public class Array : List 
	{
		private System.Type elementClass;

		public Array(PersistentClass owner) : base(owner) { }

		public System.Type ElementClass 
		{
			get { return elementClass; }
			set { elementClass = value; }
		}

		public override PersistentCollectionType Type 
		{
			get { return TypeFactory.Array( Role, ElementClass ); }
		}

		public override System.Type WrapperClass 
		{
			get { return typeof(ArrayHolder); }
		}
		public override bool IsArray 
		{
			get { return true; }
		}


	}
}
