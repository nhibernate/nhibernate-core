using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.Util;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that combines a hi/lo algorithm with an underlying
	/// oracle-style sequence that generates hi values.
	/// </summary>
	/// <remarks>
	/// <p>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>
	///	&lt;generator class="seqhilo"&gt;
	///		&lt;param name="sequence"&gt;uid_sequence&lt;/param&gt;
	///		&lt;param name="max_lo"&gt;max_lo_value&lt;/param&gt;
	///		&lt;param name="schema"&gt;db_schema&lt;/param&gt;
	///	&lt;/generator&gt;
	///	</code>
	/// </p>
	/// <p>
	/// The <c>sequence</c> parameter is required, the <c>max_lo</c> and <c>schema</c> are optional.
	/// </p>
	/// <p>
	/// The user may specify a <c>max_lo</c> value to determine how often new hi values are
	/// fetched. If sequences are not avaliable, <c>TableHiLoGenerator</c> might be an
	/// alternative.
	/// </p>
	/// </remarks>
	public partial class SequenceHiLoGenerator : SequenceGenerator
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(SequenceHiLoGenerator));

		/// <summary>
		/// The name of the maximum low value parameter.
		/// </summary>
		public const string MaxLo = "max_lo";

		private int maxLo;
		private int lo;
		private long hi;
		private System.Type returnClass;

		#region IConfigurable Members

		/// <summary>
		/// Configures the SequenceHiLoGenerator by reading the value of <c>sequence</c>, <c>max_lo</c>, 
		/// and <c>schema</c> from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		public override void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			base.Configure(type, parms, dialect);
			maxLo = PropertiesHelper.GetInt32(MaxLo, parms, 9);
			lo = maxLo + 1; // so we "clock over" on the first invocation
			returnClass = type.ReturnedClass;
		}

		#endregion

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generate an <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/> 
		/// for the identifier by using a database sequence.
		/// </summary>
		/// <param name="session">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>The new identifier as a <see cref="Int16"/>, <see cref="Int32"/>, or <see cref="Int64"/>.</returns>
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
				lo = 1;
				hi = hival * (maxLo + 1);
				if (log.IsDebugEnabled)
					log.Debug("new hi value: " + hival);
			}
			return IdentifierGeneratorFactory.CreateNumber(hi + lo++, returnClass);
		}

		#endregion
	}
}