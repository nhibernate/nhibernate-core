using System;
using System.Data.Common;

using NHibernate.Dialect;
using NHibernate.Exceptions;

namespace NHibernate.Engine.Transaction
{
	/// <summary>
	/// Class which provides the isolation semantics required by
	/// an <see cref="IIsolatedWork"/>.
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
  /// <listheader>
	///      <description>Processing comes in two flavors:</description>
  ///  </listheader>
  ///  <item>
	///      <term><see cref="DoIsolatedWork"/> </term>
	///      <description>makes sure the work to be done is performed in a separate, distinct transaction</description>
  ///  </item>
	///  <item>
	///      <term><see cref="DoNonTransactedWork"/> </term>
	///      <description>makes sure the work to be done is performed outside the scope of any transaction</description>
	///  </item>
	/// </list>
	/// </remarks>
	public partial class Isolater
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(Isolater));

		/// <summary> 
		/// Ensures that all processing actually performed by the given work will
		/// occur on a separate transaction. 
		/// </summary>
		/// <param name="work">The work to be performed. </param>
		/// <param name="session">The session from which this request is originating. </param>
		public static void DoIsolatedWork(IIsolatedWork work, ISessionImplementor session)
		{
			session.Factory.TransactionFactory.ExecuteWorkInIsolation(session, work, true);
		}

		/// <summary> 
		/// Ensures that all processing actually performed by the given work will
		/// occur outside of a transaction. 
		/// </summary>
		/// <param name="work">The work to be performed. </param>
		/// <param name="session">The session from which this request is originating. </param>
		public static void DoNonTransactedWork(IIsolatedWork work, ISessionImplementor session)
		{
			session.Factory.TransactionFactory.ExecuteWorkInIsolation(session, work, false);
		}
	}
}