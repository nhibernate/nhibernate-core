using System;

namespace NHibernate.DomainModel 
{
	/// <summary>
	/// Summary description for INameable.
	/// </summary>
	public interface INameable 
	{
		string Name { get; set; }
		long Key { get; set; }
	}
}
