using System;
using NHibernate.Engine;

namespace NHibernate.Event.Default
{
	[Serializable]
	public class DefaultSaveOrUpdateCopyEventListener : DefaultMergeEventListener
	{
		protected internal override Cascades.CascadingAction CascadeAction
		{
			get { return Cascades.CascadingAction.ActionSaveUpdateCopy; }
		}
	}
}
