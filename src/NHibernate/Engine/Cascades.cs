using System;
using NHibernate.Type;
using NHibernate.Proxy;
using NHibernate.Persister;
using NHibernate.Collection;


namespace NHibernate.Engine {

	/// <summary>
	/// The types of children to cascade to
	/// </summary>
	public enum CascadePoint {
		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent
		/// entity and just before deletion
		/// </summary>
		CascadeAfterInsertBeforeDelete = 1,
		/// <summary>
		/// A cascade point that occurs just before the insertion of the parent entity
		/// and just after deletion
		/// </summary>
		CascadeBeforeInsertAfterDelete = 2,
		/// <summary>
		/// A cascade point that occurs just after the insertion of the parent entity
		/// and just before deletion, inside a collection
		/// </summary>
		CascadeAfterInsertBeforeDeleteViaCollection = 3,
		/// <summary>
		/// A cascade point that occurs just after the update of the parent entity
		/// </summary>
		CascadeOnUpdate = 0
	}

	/// <summary>
	/// Summary description for Cascades.
	/// </summary>
	public class Cascades {
		private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Cascades));
	
		/// <summary>
		/// A session action that may be cascaded from parent entity to its children
		/// </summary>
		public abstract class CascadingAction {
			protected CascadingAction() {}

			/// <summary>
			/// Cascade the action to the child object
			/// </summary>
			public abstract void Cascade(ISession session, object child);

			/// <summary>
			/// Should this action be cascaded to the given (possibly unitilized) collection?
			/// </summary>
			public abstract bool ShouldCascadeCollection(object collection);


			public static CascadingAction ActionDelete = new ActionDeleteClass();
			private class ActionDeleteClass : CascadingAction {
				public override void Cascade(ISession session, object child) {
					log.Debug("cascading to delete()");
					session.Delete(child);
				}
				public override bool ShouldCascadeCollection(object collection) {
					return true;
				}
			}

			public static CascadingAction ActionSaveUpdate = new ActionSaveUpdateClass();
			private class ActionSaveUpdateClass : CascadingAction {
				public override void Cascade(ISession session, object child) {
					if ( ! (child is IHibernateProxy) ) { //TODO: Add proxy test (see hibernate) || !HibernateProxyHelper.
						log.Debug("cascading to SaveOrUpdate()");
						session.SaveOrUpdate(child);
					}
				}
				public override bool ShouldCascadeCollection(object collection) {
					return !(collection is PersistentCollection) || ( (PersistentCollection) collection).WasInitialized;
				}
			}
		}

		public abstract class CascadeStyle {
			protected CascadeStyle() { }

			public abstract bool DoCascade(CascadingAction action);


			public static CascadeStyle StyleAll = new StyleAllClass();
			private class StyleAllClass : CascadeStyle {
				public override bool DoCascade(CascadingAction action) {
					return true;
				}
			}
			public static CascadeStyle StyleExceptDelete = new StyleExceptDeleteClass();
			private class StyleExceptDeleteClass : CascadeStyle {
				public override bool DoCascade(CascadingAction action) {
					return action != CascadingAction.ActionDelete;
				}
			}
			public static CascadeStyle StyleOnlyDelete = new StyleOnlyDeleteClass();
			private class StyleOnlyDeleteClass : CascadeStyle {
				public override bool DoCascade(CascadingAction action) {
					return action == CascadingAction.ActionDelete;
				}
			}
			public static CascadeStyle StyleNone = new StyleNoneClass();
			private class StyleNoneClass : CascadeStyle {
				public override bool DoCascade(CascadingAction action) {
					return false;
				}
			}
		}

		/// <summary>
		/// A strategy for determining if an identifier value is an identifier of a new 
		/// transient instance or a previously persistent transient instance. The strategy
		/// is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
		/// </summary>
		public class IdentifierValue {
			private object value;
			protected IdentifierValue() {
				this.value = null;
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if its identifier is null or
			/// equal to <c>Value</c>
			/// </summary>
			/// <param name="value"></param>
			public IdentifierValue(object value) {
				this.value = value;
			}

			/// <summary>
			/// Does the given identifier belong to a new instance
			/// </summary>
			public virtual bool IsUnsaved(object id) {
				if ( log.IsDebugEnabled ) log.Debug("unsaved-value: " + value);
				return id==null || value.Equals(id);
			}

			public static IdentifierValue SaveAny = new SaveAnyClass();
			private class SaveAnyClass : IdentifierValue {
				public override bool IsUnsaved(object id) {
					log.Debug("unsaved-value strategy ANY");
					return true;
				}
			}
			public static IdentifierValue SaveNone = new SaveNoneClass();
			private class SaveNoneClass : IdentifierValue {
				public override bool IsUnsaved(object id) {
					log.Debug("unsaved-value strategy NONE");
					return false;
				}
			}
			public static IdentifierValue SaveNull = new SaveNullClass();
			private class SaveNullClass : IdentifierValue {
				public override bool IsUnsaved(object id) {
					log.Debug("unsaved-value strategy NULL");
					return id==null;
				}
			}
		}

		private static void Cascade(ISessionImplementor session, object child, IType type, CascadingAction action, CascadePoint cascadeTo) {
			if (child != null) {
				if ( type.IsAssociationType ) {
					if ( ((IAssociationType)type).ForeignKeyType.CascadeNow(cascadeTo) ) {
						if ( type.IsEntityType ) {
							action.Cascade(session, child);
						} else if ( type.IsPersistentCollectionType ) {
							CascadePoint cascadeVia;
							if ( cascadeTo == CascadePoint.CascadeAfterInsertBeforeDelete ) {
								cascadeVia = CascadePoint.CascadeAfterInsertBeforeDeleteViaCollection;
							} else {
								cascadeVia = cascadeTo;
							}
							PersistentCollectionType pctype = (PersistentCollectionType) type;
							//TODO: finish
						}
					}
				}
			}
		}

		public static void Cascade(ISessionImplementor session, IClassPersister persister, object parent, Cascades.CascadingAction action, CascadePoint cascadeTo) {
			//TODO: Finish
		}
		


		
	}
}
