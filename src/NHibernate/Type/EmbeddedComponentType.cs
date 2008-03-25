using System;
using NHibernate.Engine;
using NHibernate.Tuple.Component;

namespace NHibernate.Type
{
	[Serializable]
	public class EmbeddedComponentType : ComponentType
	{
		public EmbeddedComponentType(ComponentMetamodel metamodel)
			: base(metamodel)
		{
		}

		public override bool IsEmbedded
		{
			get { return true; }
		}

		public override object Instantiate(object parent, ISessionImplementor session)
		{
			bool useParent = parent != null && base.ReturnedClass.IsInstanceOfType(parent);

			return useParent ? parent : base.Instantiate(parent, session);
		}
	}
}
