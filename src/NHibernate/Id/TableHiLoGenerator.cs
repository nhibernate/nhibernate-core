using System;
using System.Data;
using System.Collections;
using System.Runtime.CompilerServices;

using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;


namespace NHibernate.Id 
{
	/// <summary>
	/// An <c>IIdentifierGenerator</c> that returns an <c>Int64</c>, constructed using
	/// a hi/lo algorithm.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The hi value MUST be fecthed in a seperate transaction to the <c>Session</c>
	/// transaction so the generator must be able to obtain a new connection and 
	/// commit it. Hence this implmentation may not be used when the user is supplying
	/// connections.  In that case a <see cref="SequenceHiLoGenerator"/> would be a 
	/// better choice (where supported).
	/// </para>
	/// <para>
	/// Mapping parameters supported are: <c>table</c>, <c>column</c>, and <c>max_lo</c>
	/// </para>
	/// </remarks>
	public class TableHiLoGenerator : TableGenerator 
	{
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TableHiLoGenerator));

		public const string MaxLo = "max_lo";

		private long hi;
		private int lo;
		private int maxLo;
		private System.Type returnClass;

		public override void Configure(IType type, IDictionary parms, Dialect.Dialect d) 
		{
			base.Configure(type, parms, d);
			maxLo = PropertiesHelper.GetInt32(MaxLo, parms, short.MaxValue);
			lo = maxLo + 1; // so we "clock over" on the first invocation
			returnClass = type.ReturnedClass;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public override object Generate(ISessionImplementor session, object obj) 
		{
			if (lo > maxLo) 
			{
				int hival = ( (int) base.Generate(session, obj) );
				lo = 1;
				hi = hival * (maxLo+1);
				log.Debug("new hi value: " + hival);
			}

			return IdentifierGeneratorFactory.CreateNumber( hi + lo++, returnClass );
		}
	}
}
