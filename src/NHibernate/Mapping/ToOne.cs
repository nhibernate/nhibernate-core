using System;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A simple-point association (ie. a reference to another entity).
	/// </summary>
	public abstract class ToOne : SimpleValue, IFetchable
	{
		private FetchMode fetchMode;
		private bool lazy = true;
		protected internal string referencedPropertyName;
		private string referencedEntityName;
		private bool embedded;
		private bool unwrapProxy;

		/// <summary>
		/// 
		/// </summary>
		public ToOne(Table table) : base(table)
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
			set { referencedPropertyName = StringHelper.InternedIfPossible(value); }
		}

		public string ReferencedEntityName
		{
			get { return referencedEntityName; }
			set { referencedEntityName = StringHelper.InternedIfPossible(value); }
		}

		public bool IsLazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		public bool Embedded
		{
			get { return embedded; }
			set { embedded = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract override void CreateForeignKey();

		public override void SetTypeUsingReflection(string className, string propertyName, string accesorName)
		{
			if (referencedEntityName == null)
			{
				referencedEntityName = ReflectHelper.ReflectedPropertyClass(className, propertyName, accesorName).FullName;
			}
		}

		public bool UnwrapProxy
		{
			get { return unwrapProxy; }
			set { unwrapProxy = value; }
		}
	}
}
