using System;
using System.Collections;
using System.Runtime.CompilerServices;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;
using System.Collections.Generic;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that returns an <c>Int64</c>, constructed using
	/// a hi/lo algorithm.
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>
	///	&lt;generator class="hilo"&gt;
	///		&lt;param name="table"&gt;table&lt;/param&gt;
	///		&lt;param name="column"&gt;id_column&lt;/param&gt;
	///		&lt;param name="max_lo"&gt;max_lo_value&lt;/param&gt;
	///		&lt;param name="schema"&gt;db_schema&lt;/param&gt;
	///	&lt;/generator&gt;
	///	</code>
	/// </p>
	/// <p>
	/// The <c>table</c> and <c>column</c> parameters are required, the <c>max_lo</c> and 
	/// <c>schema</c> are optional.
	/// </p>
	/// <p>
	/// The hi value MUST be fecthed in a seperate transaction to the <c>ISession</c>
	/// transaction so the generator must be able to obtain a new connection and 
	/// commit it. Hence this implementation may not be used when the user is supplying
	/// connections.  In that case a <see cref="SequenceHiLoGenerator"/> would be a 
	/// better choice (where supported).
	/// </p>
	/// </remarks>
	public class TableHiLoGenerator : TableGenerator
	{
		private static readonly ILogger log = LoggerProvider.LoggerFor(typeof(TableHiLoGenerator));

		/// <summary>
		/// The name of the max lo parameter.
		/// </summary>
		public const string MaxLo = "max_lo";

		private long hi;
		private long lo;
		private long maxLo;
		private System.Type returnClass;

		#region IConfigurable Members

		/// <summary>
		/// Configures the TableHiLoGenerator by reading the value of <c>table</c>, 
		/// <c>column</c>, <c>max_lo</c>, and <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public override void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			base.Configure(type, parms, dialect);
			maxLo = PropertiesHelper.GetInt64(MaxLo, parms, Int16.MaxValue);
			lo = maxLo + 1; // so we "clock over" on the first invocation
			returnClass = type.ReturnedClass;
		}

		#endregion

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate a <see cref="Int64"/> for the identifier by selecting and updating a value in a table.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Int64"/>.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public override object Generate(ISessionImplementor session, object obj)
		{
			if (maxLo < 1)
			{
				//keep the behavior consistent even for boundary usages
				long val = Convert.ToInt64(base.Generate(session, obj));
				if (val == 0)
					val = Convert.ToInt64(base.Generate(session, obj));
				return IdentifierGeneratorFactory.CreateNumber(val, returnClass);
			}
			if (lo > maxLo)
			{
				long hival = Convert.ToInt64(base.Generate(session, obj));
				lo = (hival == 0) ? 1 : 0;
				hi = hival * (maxLo + 1);
				log.Debug("New high value: " + hival);
			}

			return IdentifierGeneratorFactory.CreateNumber(hi + lo++, returnClass);
		}

		#endregion
	}
}