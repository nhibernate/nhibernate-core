using System.Collections;
using System.Text;

namespace NHibernate.Test.Any
{
	public class ComplexPropertyValue : IPropertyValue
	{
		private long id;
		private IDictionary subProperties = new Hashtable();

		public virtual long Id
		{
			get { return id; }
			set { id = value; }
		}

		public virtual IDictionary SubProperties
		{
			get { return subProperties; }
			set { subProperties = value; }
		}

		#region IPropertyValue Members

		public virtual string AsString()
		{
			return "complex[" + KeyString() + "]";
		}

		#endregion

		private string KeyString()
		{
			StringBuilder buff = new StringBuilder();
			foreach (object key in subProperties.Keys)
			{
				if (buff.Length > 0)
				{
					buff.Append(", ");
				}
				buff.Append(key.ToString());
			}
			return buff.ToString();
		}
	}
}