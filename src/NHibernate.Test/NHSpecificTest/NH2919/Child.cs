using System;

namespace NHibernate.Test.NHSpecificTest.NH2919
{
	public class Child
	{
		public static bool CREATE_WITH_TOY = false;

		public Child()
		{
			if ( CREATE_WITH_TOY )
				DynamicToy = new Toy();
		}

		public virtual Guid ID { get; set; }

		public virtual Parent Parent { get; set; }

		private Toy _dynamicToy;
		public virtual Toy DynamicToy
		{
			get
			{
				return _dynamicToy;
			}
			set
			{
				if ( _dynamicToy != null )
					_dynamicToy = value;
			}
		}
	}
}
