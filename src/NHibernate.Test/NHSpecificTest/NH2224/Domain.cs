using System;

namespace NHibernate.Test.NHSpecificTest.NH2224
{
	public class Class1
	{
		// Setted by reflection by NHibernate
#pragma warning disable CS0649 // Field 'Class1._number' is never assigned to, and will always have its default value 0
		private long _number;
#pragma warning restore CS0649 // Field 'Class1._number' is never assigned to, and will always have its default value 0
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
