using System;
using NHibernate.Type;

namespace NHibernate.Impl
{
	/// <summary>
	/// Abstract superclass of visitors that reattach collections
	/// </summary>
	internal abstract class ReattachVisitor : ProxyVisitor
	{
		private readonly object _key;

		protected object Key
		{
			get { return _key; }
		}

		public ReattachVisitor(SessionImpl session, object key)
			: base( session )
		{
			_key = key;
		}

		protected override object ProcessComponent(object component, IAbstractComponentType componentType)
		{
			IType[] types = componentType.Subtypes;
			if( component == null )
			{
				ProcessValues( new object[types.Length], types );
			}
			else
			{
				base.ProcessComponent( component, componentType );
			}

			return null;
		}
	}
}