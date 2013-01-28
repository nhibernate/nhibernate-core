using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
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
		/// control this separately.
		/// </remarks>
		public virtual bool ReallyDoCascade(CascadingAction action)
		{
			return DoCascade(action);
		}

		/// <summary> Do we need to delete orphaned collection elements? </summary>
		/// <returns> True if this style need to account for orphan delete operations; false otherwise. </returns>
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

		private static void RunTypeConstructor()
		{
			// No code needed.
		}

		#endregion

		#region The CascadeStyle implementations

		[Serializable]
		private abstract class SingletonCascadeStyle<TConcrete> : CascadeStyle, ISerializable
			where TConcrete : class, new()
		{
			public static readonly TConcrete Instance = new TConcrete();

#if NET_4_0
			[SecurityCritical]
#else
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				// Don't serialize the real object, that could cause multiple objects of the same
				// singleton type after deserialization.
				info.SetType(typeof(CascadeStyleReference));
			}

			[Serializable]
			private sealed class CascadeStyleReference : IObjectReference
			{
#if NET_4_0
				[SecurityCritical]
#endif
				public Object GetRealObject(StreamingContext context)
				{
					// First make sure that the type constructor of the base class was already executed.
					// If this isn't the case 'Instance' can't be constructed and will stay null.
					CascadeStyle.RunTypeConstructor();

					// Return the singleton instance of this CascadeStyle.
					return Instance;
				}
			}
		}

		[Serializable]
		private class AllDeleteOrphanCascadeStyle : SingletonCascadeStyle<AllDeleteOrphanCascadeStyle>
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
		private class AllCascadeStyle : SingletonCascadeStyle<AllCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return true;
			}
		}

		[Serializable]
		private class UpdateCascadeStyle : SingletonCascadeStyle<UpdateCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.SaveUpdate;
			}
		}

		[Serializable]
		private class LockCascadeStyle : SingletonCascadeStyle<LockCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Lock;
			}
		}

		[Serializable]
		private class RefreshCascadeStyle : SingletonCascadeStyle<RefreshCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Refresh;
			}
		}

		[Serializable]
		private class EvictCascadeStyle : SingletonCascadeStyle<EvictCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Evict;
			}
		}

		[Serializable]
		private class ReplicateCascadeStyle : SingletonCascadeStyle<ReplicateCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Replicate;
			}
		}

		[Serializable]
		private class MergeCascadeStyle : SingletonCascadeStyle<MergeCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Merge;
			}
		}

		[Serializable]
		private class PersistCascadeStyle : SingletonCascadeStyle<PersistCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Persist || action == CascadingAction.PersistOnFlush;
			}
		}

		[Serializable]
		private class DeleteCascadeStyle : SingletonCascadeStyle<DeleteCascadeStyle>
		{
			public override bool DoCascade(CascadingAction action)
			{
				return action == CascadingAction.Delete;
			}
		}

		[Serializable]
		private class DeleteOrphanCascadeStyle : SingletonCascadeStyle<DeleteOrphanCascadeStyle>
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
		private class NoneCascadeStyle : SingletonCascadeStyle<NoneCascadeStyle>
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
		public static readonly CascadeStyle AllDeleteOrphan = AllDeleteOrphanCascadeStyle.Instance;

		/// <summary> save / delete / update / evict / lock / replicate / merge / persist</summary>
		public static readonly CascadeStyle All = AllCascadeStyle.Instance;

		/// <summary> save / update</summary>
		public static readonly CascadeStyle Update = UpdateCascadeStyle.Instance;

		/// <summary> lock</summary>
		public static readonly CascadeStyle Lock = LockCascadeStyle.Instance;

		/// <summary> refresh</summary>
		public static readonly CascadeStyle Refresh = RefreshCascadeStyle.Instance;

		/// <summary> evict</summary>
		public static readonly CascadeStyle Evict = EvictCascadeStyle.Instance;

		/// <summary> replicate</summary>
		public static readonly CascadeStyle Replicate = ReplicateCascadeStyle.Instance;

		/// <summary> merge</summary>
		public static readonly CascadeStyle Merge = MergeCascadeStyle.Instance;

		/// <summary> create</summary>
		public static readonly CascadeStyle Persist = PersistCascadeStyle.Instance;

		/// <summary> delete</summary>
		public static readonly CascadeStyle Delete = DeleteCascadeStyle.Instance;

		/// <summary> delete + delete orphans</summary>
		public static readonly CascadeStyle DeleteOrphan = DeleteOrphanCascadeStyle.Instance;

		/// <summary> no cascades</summary>
		public static readonly CascadeStyle None = NoneCascadeStyle.Instance;

		#endregion
	}
}
