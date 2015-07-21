using System;

namespace NHibernate.Test.NHSpecificTest.NH2224
{
	public class Class1
	{
		private long _number;
		private DateTime _dateOfChange;
		
		public virtual long Number 
		{
			get 
			{ 
				return _number; 
			}
		}

		
		public virtual DateTime DateOfChange 
		{
			get 
			{ 
				return _dateOfChange; 
			}
			set 
			{ 
				if (_dateOfChange != value)
					_dateOfChange = value;
			}
		}
	}
}