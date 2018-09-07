// There is no support of WCF Server under .Net Core, so it makes little sense to provide
// a WCF OperationContext for it. Since it adds additional heavy dependencies, it has been
// considered not desirable to provide it for .Net Standard. (It could be useful in case some
// WCF server becames available in another frameworks or if a .Net Framework application
// consumes the .Net standard distribution of NHibernate instead of the .Net Framework one)
// See https://github.com/dotnet/wcf/issues/1200 and #1842
#if NETFX
using System.Collections;
using System.ServiceModel;

using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for the current OperationContext in WCF. Works only during the lifetime of a WCF operation.
	/// </summary>
	public class WcfOperationSessionContext : MapBasedSessionContext
	{
		public WcfOperationSessionContext(ISessionFactoryImplementor factory) : base(factory) {}

		private static WcfStateExtension WcfOperationState
		{
			get
			{
				var extension = OperationContext.Current.Extensions.Find<WcfStateExtension>();

				if (extension == null)
				{
					extension = new WcfStateExtension();
					OperationContext.Current.Extensions.Add(extension);
				}

				return extension;
			}
		}

		protected override IDictionary GetMap()
		{
			return WcfOperationState.Map;
		}

		protected override void SetMap(IDictionary value)
		{
			WcfOperationState.Map = value;
		}
	}

	public class WcfStateExtension : IExtension<OperationContext>
	{
		public IDictionary Map { get; set; }

		// we don't really need implementations for these methods in this case
		public void Attach(OperationContext owner) { }
		public void Detach(OperationContext owner) { }
	}
}
#else
// 6.0 TODO: remove the whole #else
using System;
using System.Collections;
using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Obsolete class not usable in the .Net Core and .Net Standard distributions of NHibernate. Use the
	/// .Net Framework distribution of NHibernate if you need it. See
	/// https://github.com/nhibernate/nhibernate-core/issues/1842
	/// </summary>
	[Obsolete("Not supported in the .Net Core and .Net Standard distributions of NHibernate", true)]
	public class WcfOperationSessionContext : MapBasedSessionContext
	{
		public WcfOperationSessionContext(ISessionFactoryImplementor factory) : base(factory)
		{
			throw new NotSupportedException(
				"WcfOperationSessionContext is currently supported only by the .Net Framework distribution of NHibernate");
		}

		protected override IDictionary GetMap()
		{
			return null;
		}

		protected override void SetMap(IDictionary value)
		{
		}
	}
}
#endif
