using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// Summary description for AvalonProxyImpl.
	/// </summary>
	public class AvalonProxyImpl : AvalonProxy
	{
		private int _id;
		private string _name;

		#region AvalonProxy Members

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		
		#endregion

	}
}
