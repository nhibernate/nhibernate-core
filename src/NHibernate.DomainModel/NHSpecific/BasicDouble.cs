using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for BasicDouble.
	/// </summary>
	public class BasicDouble
	{
		int _id;
		Double _doubleValue;

		public BasicDouble()
		{
		}

		public int Id 
		{
			get { return _id; }
			set { _id = value; }
		}

		public double DoubleValue
		{
			get {return _doubleValue;}
			set {_doubleValue = value;}
		}
		
	}
}

