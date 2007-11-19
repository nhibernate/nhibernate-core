using System;
using Lucene.Net.Documents;

namespace NHibernate.Search.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DocumentIdAttribute : Attribute
	{
		private string name = null;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}