using System;

using NHibernate.Type;

namespace NHibernate.Mapping
{
	public class Any : Value
	{
		private IType identifierType;
		private IType metaType = TypeFactory.GetTypeType();

		public Any(Table table) : base(table){
		}

		public override bool IsAny {
			get {
				return true;
			}
		}

		/// <summary>
		/// Get or set the identifier type 
		/// </summary>
		public virtual IType IdentifierType {
			get {
				return identifierType;
			}
			set {
				this.identifierType = value;
			}
		}

		public override IType Type {
			get {
				return new ObjectType(metaType, identifierType);
			}
			set {
				throw new NotSupportedException("cannot set type of an Any");
			}
		}

		public override void SetTypeByReflection(System.Type propertyClass, string propertyName) {}

		/// <summary>
		/// Get or set the metatype 
		/// </summary>
		public virtual IType MetaType {
			get {
				return metaType;
			}
			set {
				metaType = value;
			}
		}
	}
}