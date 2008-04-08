using System;
using NHibernate.Engine;

namespace NHibernate.Event.Default
{
	[Serializable]
	public class DefaultSaveOrUpdateCopyEventListener : DefaultMergeEventListener
	{
		protected override CascadingAction CascadeAction
		{
			get { return CascadingAction.SaveUpdateCopy; }
		}
	}
}
