using System;
using NHibernate.Engine;

namespace NHibernate.UserTypes
{
	/// <summary> 
	/// Marker interface for user types which want to perform custom
	/// logging of their corresponding values 
	/// </summary>
	// Since 5.2
	[Obsolete("This interface has no usage and will be removed in a future version")]
	public interface ILoggableUserType
	{
		/// <summary> Generate a loggable string representation of the collection (value). </summary>
		/// <param name="value">The collection to be logged; guaranteed to be non-null and initialized. </param>
		/// <param name="factory">The factory. </param>
		/// <returns> The loggable string representation. </returns>
		string ToLoggableString(object value, ISessionFactoryImplementor factory);
	}
}
