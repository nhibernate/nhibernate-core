using System;
using NHibernate.Type;

namespace NHibernate.Mapping {
	
	public class IntegerValue : Value{
		
		public IntegerValue(Table table) : base(table) { }

		public override IType Type {
			get { return NHibernate.Integer; }
		}
	}
}
