using System;

namespace NHibernate.DomainModel
{
	[Serializable]
	public class Super
	{
		protected string _name;

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	}
}