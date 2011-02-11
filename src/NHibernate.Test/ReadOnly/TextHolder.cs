using System;

namespace NHibernate.Test.ReadOnly
{
	public class TextHolder
	{
		private long id;
		private string theText;
		
		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}
		
		public virtual string TheText
		{
			get { return theText; }
			set { theText = value; }
		}
		
		public TextHolder()
		{
		}
		
		public TextHolder(string text)
		{
			this.theText = text;
		}
	}
}
