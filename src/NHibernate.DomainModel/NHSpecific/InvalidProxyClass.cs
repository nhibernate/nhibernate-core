using System;

namespace NHibernate.DomainModel.NHSpecific
{
	/// <summary>
	/// A class that cannot be used with lazy="true" if proxy validation
	/// is enabled.
	/// </summary>
	public class InvalidProxyClass
	{
		private InvalidProxyClass()
		{
		}

		public int Id
		{
			get { return 1; }
			set { }
		}
	}
}
