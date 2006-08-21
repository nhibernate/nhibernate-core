using System;
using NHibernate.Loader;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A simple-point association (ie. a reference to another entity).
	/// </summary>
	public abstract class ToOne : SimpleValue, IFetchable
	{
		private FetchMode fetchMode;
		private bool lazy = true;
		private string referencedPropertyName;
		private string referencedEntityName;

		/// <summary>
		/// 
		/// </summary>
		public ToOne( Table table ) : base( table )
		{
		}

		/// <summary></summary>
		public override FetchMode FetchMode
		{
			get { return fetchMode; }
			set { fetchMode = value; }
		}

		/// <summary></summary>
		public string ReferencedPropertyName
		{
			get { return referencedPropertyName; }
			set { referencedPropertyName = value; }
		}

		public string ReferencedEntityName
		{
			get { return referencedEntityName; }
			set { referencedEntityName = value == null ? null : string.Intern(value); }
		}

		public bool IsLazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract override void CreateForeignKey( );
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public abstract override void SetTypeByReflection( System.Type propertyClass, string propertyName, string propertyAccess );
	}
}
