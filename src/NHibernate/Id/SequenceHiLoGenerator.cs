using System.Collections;
using System.Runtime.CompilerServices;
using log4net;
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
	/// <para>
	/// The user may specify a maximum lo value to determine how often new hi values are
	/// fetched. If sequences are not avaliable, <c>TableHiLoGenerator</c> might be an
	/// alternative.
	/// </para>
	/// <para>
	/// The mapping parameters supported are: <c>sequence</c>, <c>max_lo</c>
	/// </para>
	/// </remarks>
	public class SequenceHiLoGenerator : SequenceGenerator
	{
		private static readonly ILog log = LogManager.GetLogger( typeof( SequenceHiLoGenerator ) );

		/// <summary></summary>
		public const string MaxLo = "max_lo";

		private int maxLo;
		private int lo;
		private long hi;
		private System.Type returnClass;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="type"></param>
		/// <param name="parms"></param>
		/// <param name="d"></param>
		public override void Configure( IType type, IDictionary parms, Dialect.Dialect d )
		{
			base.Configure( type, parms, d );
			maxLo = PropertiesHelper.GetInt32( MaxLo, parms, 9 );
			lo = maxLo + 1; // so we "clock over" on the first invocation
			returnClass = type.ReturnedClass;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="session"></param>
		/// <param name="obj"></param>
		/// <returns></returns>
		[MethodImpl( MethodImplOptions.Synchronized )]
		public override object Generate( ISessionImplementor session, object obj )
		{
			if( lo > maxLo )
			{
				long hival = ( ( long ) base.Generate( session, obj ) );
				lo = 1;
				hi = hival*( maxLo + 1 );
				log.Debug( "new hi value: " + hival );
			}
			return IdentifierGeneratorFactory.CreateNumber( hi + lo++, returnClass );
		}
	}
}