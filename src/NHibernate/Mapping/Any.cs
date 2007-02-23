using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary>
	/// A NHibernate <c>any</c> type.
	/// </summary>
	/// <remarks>
	/// Polymorphic association to one of several tables.
	/// </remarks>
	public class Any : SimpleValue
	{
		private IType identifierType;
		private IType metaType = NHibernateUtil.Class;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public Any(Table table) : base(table)
		{
		}

		/// <summary>
		/// Get or set the identifier type 
		/// </summary>
		public virtual IType IdentifierType
		{
			get { return identifierType; }
			set { this.identifierType = value; }
		}

		/// <summary></summary>
		public override IType Type
		{
			get { return new AnyType(metaType, identifierType); }
			set { throw new NotSupportedException("cannot set type of an Any"); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		/// <param name="propertyAccess"></param>
		public override void SetTypeByReflection(System.Type propertyClass, string propertyName, string propertyAccess)
		{
		}

		/// <summary>
		/// Get or set the metatype 
		/// </summary>
		public virtual IType MetaType
		{
			get { return metaType; }
			set { metaType = value; }
		}
	}
}