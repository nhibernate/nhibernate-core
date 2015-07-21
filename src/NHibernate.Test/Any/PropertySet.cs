using System.Collections;

namespace NHibernate.Test.Any
{
	public class PropertySet
	{
		private IDictionary generalProperties = new Hashtable();
		private long id;
		private string name;
		private IPropertyValue someSpecificProperty;
		public PropertySet() {}

		public PropertySet(string name)
		{
			this.name = name;
		}

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

		public virtual IPropertyValue SomeSpecificProperty
		{
			get { return someSpecificProperty; }
			set { someSpecificProperty = value; }
		}

		public virtual IDictionary GeneralProperties
		{
			get { return generalProperties; }
			set { generalProperties = value; }
		}
	}
}