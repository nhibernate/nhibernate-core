using System;
using NHibernate.Type;

namespace NHibernate.Mapping
{
	/// <summary></summary>
	public class Any : Value
	{
		private IType identifierType;
		private IType metaType = TypeFactory.GetTypeType();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="table"></param>
		public Any( Table table ) : base( table )
		{
		}

		/// <summary></summary>
		public override bool IsAny
		{
			get { return true; }
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
			get { return new ObjectType( metaType, identifierType ); }
			set { throw new NotSupportedException( "cannot set type of an Any" ); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyClass"></param>
		/// <param name="propertyName"></param>
		public override void SetTypeByReflection( System.Type propertyClass, string propertyName )
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