using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for Component.
	/// </summary>
	public class Component
	{
		private string _name;
		private SubComponent _subComponent;

		public Component()
		{
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public SubComponent SubComponent
		{
			get { return _subComponent; }
			set { _subComponent = value; }
		}
	}
}