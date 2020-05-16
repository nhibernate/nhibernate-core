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
			var useParent =
				parent != null &&
				//TODO: Yuck! This is not quite good enough, it's a quick
				//hack around the problem of having a to-one association
				//that refers to an embedded component:
				ReturnedClass.IsInstanceOfType(parent);

			return useParent ? parent : base.Instantiate(parent, session);
		}
	}
}
