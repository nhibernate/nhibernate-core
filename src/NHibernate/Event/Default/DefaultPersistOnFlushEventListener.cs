using System;
using NHibernate.Engine;

namespace NHibernate.Event.Default
{
	/// <summary>  When persist is used as the cascade action, persistOnFlush should be used</summary>
	[Serializable]
	public class DefaultPersistOnFlushEventListener : DefaultPersistEventListener
	{
		protected override CascadingAction CascadeAction
		{
			get { return CascadingAction.PersistOnFlush; }
		}
	}
}
