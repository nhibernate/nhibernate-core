using System;

namespace NHibernate.Mapping 
{
	public class PrimitiveArray : Array 
	{
		
		public PrimitiveArray(PersistentClass owner) : base(owner) {}

		public override bool IsPrimitiveArray 
		{
			get { return true; }
		}
	}
}
