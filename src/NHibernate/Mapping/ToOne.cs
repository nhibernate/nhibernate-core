using System;
using NHibernate.Loader;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A simple-point association (ie. a reference to another entity).
	/// </summary>
	public abstract class ToOne : SimpleValue, IFetchable
	{
		private OuterJoinFetchStrategy joinedFetch;
		private string referencedPropertyName;

		/// <summary>
		/// 
		/// </summary>
		public ToOne( Table table ) : base( table )
		{
		}

		/// <summary></summary>
		public override OuterJoinFetchStrategy OuterJoinFetchSetting
		{
			get { return joinedFetch; }
			set { joinedFetch = value; }
		}

		/// <summary></summary>
		public string ReferencedPropertyName
		{
			get { return referencedPropertyName; }
			set { referencedPropertyName = value; }
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
		public abstract override void SetTypeByReflection( System.Type propertyClass, string propertyName );
	}
}
