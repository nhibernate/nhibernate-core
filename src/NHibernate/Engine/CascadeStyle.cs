using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Engine
{
	/// <summary> A contract for defining the aspects of cascading various persistence actions. </summary>
	/// <seealso cref="CascadingAction"/>
	[Serializable]
	public abstract class CascadeStyle
	{
		/// <summary> package-protected constructor</summary>
		internal CascadeStyle() {}
		static CascadeStyle()
		{
			Styles["all"] = All;
			Styles["all-delete-orphan"] = AllDeleteOrphan;
			Styles["save-update"] = Update;
			Styles["persist"] = Persist;
			Styles["merge"] = Merge;
			Styles["lock"] = Lock;
			Styles["refresh"] = Refresh;
			Styles["replicate"] = Replicate;
			Styles["evict"] = Evict;
			Styles["delete"] = Delete;
			Styles["remove"] = Delete; // adds remove as a sort-of alias for delete...
			Styles["delete-orphan"] = DeleteOrphan;
			Styles["none"] = None;
		}

		#region The CascadeStyle contract

		/// <summary> For this style, should the given action be cascaded? </summary>
		/// <param name="action">The action to be checked for cascade-ability. </param>
		/// <returns> True if the action should be cascaded under this style; false otherwise. </returns>
		public abstract bool DoCascade(CascadingAction action);

		/// <summary> 
		/// Probably more aptly named something like doCascadeToCollectionElements(); 
		/// it is however used from both the collection and to-one logic branches...
		/// </summary>
		/// <param name="action">The action to be checked for cascade-ability. </param>
		/// <returns> True if the action should be really cascaded under this style; false otherwise. </returns>
		/// <remarks>
		/// For this style, should the given action really be cascaded?  The default
		/// implementation is simply to return {@link #doCascade}; for certain
		/// styles (currently only delete-orphan), however, we need to be able to
		/// control this seperately.
		/// </remarks>
		public virtual bool ReallyDoCascade(CascadingAction action)
		{
			return DoCascade(action);
		}

		/// <summary> Do we need to delete orphaned collection elements? </summary>
		/// <returns> True if this style need to account for orphan delete operations; false othwerwise. </returns>
		public virtual bool HasOrphanDelete
		{
			get { return false; }
		}

		#endregion

		#region  Static helper methods

		internal static readonly Dictionary<string, CascadeStyle> Styles = new Dictionary<string, CascadeStyle>();

		/// <summary> Factory method for obtaining named cascade styles </summary>
		/// <param name="cascade">The named cascade style name. </param>
		/// <returns> The appropriate CascadeStyle </returns>
		public static CascadeStyle GetCascadeStyle(string cascade)
		{
			CascadeStyle style;
			if (!Styles.TryGetValue(cascade, out style))
				throw new MappingException("Unsupported cascade style: " + cascade);
			else
				return style;
		}

		#endregion

		#region The CascadeStyle implementations

		[Serializable]
		private class AllDeleteOrphanCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return true;
			}
			public override bool HasOrphanDelete
			{
				get { return true; }
			}
		}

		[Serializable]
		private class AllCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return true;
			}
		}

		[Serializable]
		private class UpdateCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.SaveUpdate || action == CascadingAction.SaveUpdateCopy;
			}
		}

		[Serializable]
		private class LockCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Lock;
			}
		}

		[Serializable]
		private class RefreshCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Refresh;
			}
		}

		[Serializable]
		private class EvictCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Evict;
			}
		}

		[Serializable]
		private class ReplicateCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Replicate;
			}
		}

		[Serializable]
		private class MergeCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Merge;
			}
		}

		[Serializable]
		private class PersistCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Persist || action == CascadingAction.PersistOnFlush;
			}
		}

		[Serializable]
		private class DeleteCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Delete;
			}
		}

		[Serializable]
		private class DeleteOrphanCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Delete || action == CascadingAction.SaveUpdate;
			}

			public override bool ReallyDoCascade(CascadingAction action)
			{
				return action == CascadingAction.Delete;
			}

			public override bool HasOrphanDelete
			{
				get { return true; }
			}
		}

		[Serializable]
		private class NoneCascadeStyle : CascadeStyle
		{
			public override bool DoCascade(CascadingAction action)
			{
				return false;
			}
		}

		[Serializable]
		public sealed class MultipleCascadeStyle : CascadeStyle
		{
			private readonly CascadeStyle[] styles;
			public MultipleCascadeStyle(CascadeStyle[] styles)
			{
				this.styles = styles;
			}

			public override bool DoCascade(CascadingAction action)
			{
				for (int i = 0; i < styles.Length; i++)
				{
					if (styles[i].DoCascade(action))
						return true;
				}
				return false;
			}

			public override bool ReallyDoCascade(CascadingAction action)
			{
				for (int i = 0; i < styles.Length; i++)
				{
					if (styles[i].ReallyDoCascade(action))
						return true;
				}
				return false;
			}

			public override bool HasOrphanDelete
			{
				get
				{
					for (int i = 0; i < styles.Length; i++)
						if (styles[i].HasOrphanDelete)
							return true;

					return false;
				}
			}

			public override string ToString()
			{
				return ArrayHelper.ToString(styles);
			}
		}

		/// <summary> save / delete / update / evict / lock / replicate / merge / persist + delete orphans</summary>
		public static readonly CascadeStyle AllDeleteOrphan = new AllDeleteOrphanCascadeStyle();

		/// <summary> save / delete / update / evict / lock / replicate / merge / persist</summary>
		public static readonly CascadeStyle All = new AllCascadeStyle();

		/// <summary> save / update</summary>
		public static readonly CascadeStyle Update = new UpdateCascadeStyle();

		/// <summary> lock</summary>
		public static readonly CascadeStyle Lock = new LockCascadeStyle();

		/// <summary> refresh</summary>
		public static readonly CascadeStyle Refresh = new RefreshCascadeStyle();

		/// <summary> evict</summary>
		public static readonly CascadeStyle Evict = new EvictCascadeStyle();

		/// <summary> replicate</summary>
		public static readonly CascadeStyle Replicate = new ReplicateCascadeStyle();

		/// <summary> merge</summary>
		public static readonly CascadeStyle Merge = new MergeCascadeStyle();

		/// <summary> create</summary>
		public static readonly CascadeStyle Persist = new PersistCascadeStyle();

		/// <summary> delete</summary>
		public static readonly CascadeStyle Delete = new DeleteCascadeStyle();

		/// <summary> delete + delete orphans</summary>
		public static readonly CascadeStyle DeleteOrphan = new DeleteOrphanCascadeStyle();

		/// <summary> no cascades</summary>
		public static readonly CascadeStyle None = new NoneCascadeStyle();

		#endregion
	}
}
