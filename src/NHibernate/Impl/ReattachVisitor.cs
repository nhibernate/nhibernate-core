using System;
using System.Collections;

using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Abstract superclass of visitors that reattach collections
	/// </summary>
	internal abstract class ReattachVisitor : ProxyVisitor
	{
		private readonly object key;

		protected object Key
		{
			get { return key; }
		}

		public ReattachVisitor(SessionImpl session, object key)
			: base(session)
		{
			this.key = key;
		}

		protected override object ProcessComponent(object component, IAbstractComponentType componentType)
		{
			IType[] types = componentType.Subtypes;
			if (component == null)
			{
				ProcessValues(new object[types.Length], types);
			}
			else
			{
				base.ProcessComponent(component, componentType);
			}

			return null;
		}
	}
}
