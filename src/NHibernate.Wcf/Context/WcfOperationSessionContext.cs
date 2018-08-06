using System.Collections;
using System.Runtime.CompilerServices;
using System.ServiceModel;

using NHibernate.Engine;

namespace NHibernate.Context
{
	/// <summary>
	/// Provides a <see cref="ISessionFactory.GetCurrentSession()">current session</see>
	/// for the current OperationContext in WCF. Works only during the lifetime of a WCF operation.
	/// </summary>
	[TypeForwardedFrom("NHibernate, Version=5.1.0.0, Culture=neutral, PublicKeyToken=aa95f207798dfdb4")]
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
