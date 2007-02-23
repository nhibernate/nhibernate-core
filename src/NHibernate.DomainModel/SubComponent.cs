using System;

namespace NHibernate.DomainModel
{
	/// <summary>
	/// Summary description for SubComponent.
	/// </summary>
	public class SubComponent
	{
		private string _subName;
		private string _subName1;

		public SubComponent()
		{
		}

		public string SubName
		{
			get { return _subName; }
			set { _subName = value; }
		}

		public string SubName1
		{
			get { return _subName1; }
			set { _subName1 = value; }
		}
	}
}