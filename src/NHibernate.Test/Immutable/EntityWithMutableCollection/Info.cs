using System;

namespace NHibernate.Test.Immutable.EntityWithMutableCollection
{
	[Serializable]
	public class Info
	{
		private long id;
		private string text;
		private long version;
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string Text
		{
			get { return text; }
			set { text = value; }
		}

		public virtual long Version
		{
			get { return version; }
			set { version = value; }
		}
		
		public Info()
		{
		}
		
		public Info(string text)
		{
			this.text = text;
		}
	}
}
