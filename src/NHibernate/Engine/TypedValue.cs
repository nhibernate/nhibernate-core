using System;
using NHibernate.Type;

namespace NHibernate.Engine {
	
	public sealed class TypedValue {
		private IType type;
		private object value;

		public TypedValue(IType type, object value) {
			this.type = type;
			this.value = value;
		}

		public object Value {
			get { return value; }
			set { this.value = value; }
		}

		public IType Type {
			get { return type; }
			set { type = value; }
		}
	}
}
