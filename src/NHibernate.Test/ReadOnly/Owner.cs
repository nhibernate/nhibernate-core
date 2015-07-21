using System;

namespace NHibernate.Test.ReadOnly
{
	[Serializable]
	public class Owner
	{
		private long id;
		private string name;
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
		
		public Owner()
		{
		}
		
		public Owner(string name)
		{
			this.name = name;
		}
	}
}
