using System;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	[Serializable]
	public class Owner
	{
		private long id;
		private long version;
		private Plan plan;
		private string name;
		
		public Owner()
		{
		}
		
		public Owner(string name)
		{
			this.name = name;
		}
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public virtual Plan Plan
		{
			get { return plan; }
			set { plan = value; }
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
