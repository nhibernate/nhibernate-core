using System;
using NHibernate.Engine;

namespace NHibernate.Event.Default
{
	/// <summary>  When persist is used as the cascade action, persistOnFlush should be used</summary>
	[Serializable]
	public class DefaultPersistOnFlushEventListener : DefaultPersistEventListener
	{
		protected internal override Cascades.CascadingAction CascadeAction
		{
			get { return Cascades.CascadingAction.ActionPersistOnFlush; }
		}
	}
}
