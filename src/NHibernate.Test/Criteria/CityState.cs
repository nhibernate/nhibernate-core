using System;

namespace NHibernate.Test.Criteria
{
	[Serializable]
	public class CityState
	{
		private string city;
		private string state;
	
		public CityState() {}
	
		public CityState(string city, string state)
		{
			this.city = city;
			this.state = state;
		}
		
		public virtual string City
		{
			get { return this.city; }
			set { this.city = value; }
		}
		
		public virtual string State
		{
			get { return this.state; }
			set { this.state = value; }
		}
	}
}
