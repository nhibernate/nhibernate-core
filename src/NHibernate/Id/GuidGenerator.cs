using System;
using NHibernate.Engine;

namespace NHibernate.Id
{
	/// <summary>
	/// Generates <c>Guid</c> values using <see cref="System.Guid.NewGuid()"/>. 
	/// </summary>
	public class GuidGenerator : IIdentifierGenerator
	{
		#region IIdentifierGenerator Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		public object Generate( ISessionImplementor session, object obj )
		{
			return Guid.NewGuid();
		}

		#endregion
	}
}