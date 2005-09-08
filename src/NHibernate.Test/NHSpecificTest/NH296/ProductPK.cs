using System;

namespace NHibernate.Test.NHSpecificTest.NH296
{
	public class ProductPK
	{
		int _type;
		int _number;

		public int Type
		{
			get { return _type; }
			set { _type = value; }
		}

		public int Number
		{
			get { return _number; }
			set { _number = value; }
		}

		public override bool Equals(object obj)
		{
			if( !(obj is ProductPK))
			{
				return false;
			}

			ProductPK other = (ProductPK) obj;
			return other._type == _type && other._number == _number;
		}

		public override int GetHashCode()
		{
			return _type.GetHashCode();
		}

	}
}
