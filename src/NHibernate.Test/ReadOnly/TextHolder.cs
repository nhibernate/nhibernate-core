using System;
using NHibernate.Dialect;

namespace NHibernate.Test.ReadOnly
{
	public class TextHolder
	{
		/// <summary>
		/// Return true if the dialect supports the "text" type.
		/// </summary>
		public static bool SupportedForDialect(Dialect.Dialect dialect)
		{
			return !(dialect is FirebirdDialect || dialect is Oracle8iDialect);
		}


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
