using System;
using System.Collections;
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
		CascadeOnUpdate = 0,
		/// <summary>
		/// A cascade point that occurs just after eviction of the parent entity from the
		/// session cache
		/// </summary>
		CascadeOnEvict = 0 //-1
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
			public abstract void Cascade(ISessionImplementor session, object child);

			/// <summary>
			/// Should this action be cascaded to the given (possibly unitilized) collection?
			/// </summary>
			public abstract bool ShouldCascadeCollection(object collection);

			public abstract bool DeleteOrphans();

			public static CascadingAction ActionDelete = new ActionDeleteClass();
			
			private class ActionDeleteClass : CascadingAction 
			{
				public override void Cascade(ISessionImplementor session, object child) 
				{
					log.Debug("cascading to delete()");
					if(session.IsSaved(child)) session.Delete(child);
				}

				public override bool ShouldCascadeCollection(object collection) 
				{
					return true;
				}

				public override bool DeleteOrphans()
				{
					return true;
				}

			}

			public static CascadingAction ActionEvict = new ActionEvictClass();
			
			private class ActionEvictClass : CascadingAction 
			{
				public override void Cascade(ISessionImplementor session, object child) 
				{
					log.Debug("cascading to evict()");
					session.Evict(child);
				}

				public override bool ShouldCascadeCollection(object collection) 
				{
					return CollectionIsInitialized(collection);
				}

				public override bool DeleteOrphans()
				{
					return false;
				}

			}

			public static CascadingAction ActionSaveUpdate = new ActionSaveUpdateClass();
			
			private class ActionSaveUpdateClass : CascadingAction 
			{
				public override void Cascade(ISessionImplementor session, object child) 
				{
					log.Debug("cascading to SaveOrUpdate()");
					session.SaveOrUpdate(child);
				}

				public override bool ShouldCascadeCollection(object collection) {
					return CollectionIsInitialized(collection);
					// saves/updates don't cascade to uninitialized collections
				}

				public override bool DeleteOrphans()
				{
					return true;
				}

			}

			private static bool CollectionIsInitialized( object collection ) 
			{
				return !(collection is PersistentCollection) || ( (PersistentCollection) collection).WasInitialized;
			}
		}

		public abstract class CascadeStyle 
		{
			protected CascadeStyle() { }

			/// <summary>
			/// Should the given action be cascaded?
			/// </summary>
			/// <param name="action"></param>
			/// <returns></returns>
			public abstract bool DoCascade(CascadingAction action);

			//TODO: H2.0.3 - it looks like the CascadeStyle subclasses are defined outside of the CascadeStyle
			// class 
			
			/// <summary>
			/// Save/Delete/Update/Evict + delete orphans
			/// </summary>
			public static CascadeStyle StyleAllGC = new StyleAllGCClass();
			
			private class StyleAllGCClass : CascadeStyle
			{
				public override bool DoCascade(CascadingAction action) 
				{
					return true;
				}
			}

			/// <summary>
			/// Save / Delete / Update / Evict
			/// </summary>
			public static CascadeStyle StyleAll = new StyleAllClass();

			
			private class StyleAllClass : CascadeStyle 
			{
				public override bool DoCascade(CascadingAction action) 
				{
					return true;
				}
			}

			/// <summary>
			/// Save / Update
			/// </summary>
			public static CascadeStyle StyleSaveUpdate = new StyleSaveUpdateClass();

			private class StyleSaveUpdateClass : CascadeStyle 
			{
				public override bool DoCascade(CascadingAction action) 
				{
					return action == CascadingAction.ActionSaveUpdate;
				}
			}

			/// <summary>
			/// Delete
			/// </summary>
			public static CascadeStyle StyleOnlyDelete = new StyleOnlyDeleteClass();
			
			private class StyleOnlyDeleteClass : CascadeStyle 
			{
				public override bool DoCascade(CascadingAction action) 
				{
					return action == CascadingAction.ActionDelete;
				}
			}


			/// <summary>
			/// No Cascades
			/// </summary>
			public static CascadeStyle StyleNone = new StyleNoneClass();

			private class StyleNoneClass : CascadeStyle 
			{
				public override bool DoCascade(CascadingAction action) 
				{
					return false;
				}
			}
		}

		/// <summary>
		/// A strategy for determining if an identifier value is an identifier of a new 
		/// transient instance or a previously persistent transient instance. The strategy
		/// is determined by the <c>Unsaved-Value</c> attribute in the mapping file.
		/// </summary>
		public class IdentifierValue 
		{
			private object value;

			protected IdentifierValue() 
			{
				this.value = null;
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if its identifier is null or
			/// equal to <c>Value</c>
			/// </summary>
			/// <param name="value"></param>
			public IdentifierValue(object value) 
			{
				this.value = value;
			}

			/// <summary>
			/// Does the given identifier belong to a new instance
			/// </summary>
			public virtual bool IsUnsaved(object id) 
			{
				if ( log.IsDebugEnabled ) log.Debug("unsaved-value: " + value);
				return id==null || value.Equals(id);
			}

			/// <summary>
			/// Always assume the transient instance is newly instantiated
			/// </summary>
			public static IdentifierValue SaveAny = new SaveAnyClass();

			private class SaveAnyClass : IdentifierValue 
			{
				public override bool IsUnsaved(object id) 
				{
					log.Debug("unsaved-value strategy ANY");
					return true;
				}
			}

			/// <summary>
			/// Never assume that transient instance is newly instantiated
			/// </summary>
			public static IdentifierValue SaveNone = new SaveNoneClass();

			private class SaveNoneClass : IdentifierValue 
			{
				public override bool IsUnsaved(object id) 
				{
					log.Debug("unsaved-value strategy NONE");
					return false;
				}
			}

			/// <summary>
			/// Assume the transient instance is newly instantiated if the identifier
			/// is null.
			/// </summary>
			public static IdentifierValue SaveNull = new SaveNullClass();
			
			private class SaveNullClass : IdentifierValue 
			{
				public override bool IsUnsaved(object id) 
				{
					log.Debug("unsaved-value strategy NULL");
					return id==null;
				}
			}
		}

		[Obsolete("This is being replaced by the Cascade with a deleteOrphans as the last param.")]
		private static void Cascade(ISessionImplementor session, object child, IType type, CascadingAction action, CascadePoint cascadeTo) 
		{
			Cascade(session, child, type, action, cascadeTo, false);
		}

		/// <summary>
		/// Cascade an action to the Child.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="child"></param>
		/// <param name="type"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		/// <param name="deleteOrphans"></param>
		private static void Cascade(ISessionImplementor session, object child, IType type, CascadingAction action, CascadePoint cascadeTo, bool deleteOrphans) 
		{
			if (child != null) 
			{
				if ( type.IsAssociationType ) 
				{
					if ( ((IAssociationType)type).ForeignKeyType.CascadeNow(cascadeTo) ) 
					{
						if ( type.IsEntityType || type.IsObjectType ) 
						{
							action.Cascade(session, child);
						} 
						else if ( type.IsPersistentCollectionType ) 
						{
							CascadePoint cascadeVia;
							if ( cascadeTo == CascadePoint.CascadeAfterInsertBeforeDelete ) 
							{
								cascadeVia = CascadePoint.CascadeAfterInsertBeforeDeleteViaCollection;
							} 
							else 
							{
								cascadeVia = cascadeTo;
							}

							PersistentCollectionType pctype = (PersistentCollectionType) type;
							CollectionPersister persister = session.Factory.GetCollectionPersister( pctype.Role );
							IType elemType = persister.ElementType;
							
							// cascade to current collection elements
							ICollection iter;
							if (action.ShouldCascadeCollection(child)) 
							{
								if ( log.IsDebugEnabled ) log.Debug( "cascading to collection: " + pctype.Role );
								iter = pctype.GetElementsCollection(child);
							} 
							else 
							{
								//TODO: this hack assumes that shouldCascadeCollection() always
								// returns true for an initialized collection, which is corruption
								// of the semantics - what we need is to change
								// shouldCascadeCollection() to getCollectionCascadeIterator()
								if (child is PersistentCollection) 
								{
									PersistentCollection pc = (PersistentCollection) child;
									if ( pc.HasQueuedAdds ) 
									{
										iter = pc.QueuedAddsCollection;
									} 
									else 
									{
										iter = null;
									}
								} 
								else 
								{
									iter = null;
								}
							} 
							if (iter!=null) 
							{
								foreach(object obj in iter) 
								{
									Cascade(session, obj, elemType, action, cascadeVia, false);
								}
							}

							// handle oprhaned entities!!
							if( deleteOrphans && action.DeleteOrphans() && child is PersistentCollection) 
							{
								PersistentCollection pc = (PersistentCollection)child;
								if(pc.WasInitialized) 
								{
									//TODO: H2.0.3 - need the GetOrphans in ISessionImpl
//									ICollection orphanColl = session.GetOrphans(pc);
//									foreach(object obj in orphanColl) 
//									{
//										session.Delete(obj);
//									}
								}
							}
						}
					}
				} 
				else if (type.IsComponentType ) 
				{
					IAbstractComponentType ctype = ((IAbstractComponentType) type);
					object[] children = ctype.GetPropertyValues(child, session);
					IType[] types = ctype.Subtypes;
					for (int i=0; i<types.Length; i++) 
					{
						if (ctype.Cascade(i).DoCascade(action) )
							Cascade(session, children[i], types[i], action, cascadeTo, deleteOrphans);
					}
				}
			}
		}

		/// <summary>
		/// Cascade an action from the parent object to all its children.
		/// </summary>
		/// <param name="session"></param>
		/// <param name="persister"></param>
		/// <param name="parent"></param>
		/// <param name="action"></param>
		/// <param name="cascadeTo"></param>
		public static void Cascade(ISessionImplementor session, IClassPersister persister, object parent, Cascades.CascadingAction action, CascadePoint cascadeTo) {
			if ( persister.HasCascades ) 
			{
				if ( log.IsDebugEnabled ) log.Debug( "processing cascasdes for: " + persister.ClassName);
				IType[] types = persister.PropertyTypes;
				Cascades.CascadeStyle[] cascadeStyles = persister.PropertyCascadeStyles;
				for (int i=0; i<types.Length; i++) 
				{
					if ( cascadeStyles[i].DoCascade(action) )
						Cascade(session, persister.GetPropertyValue(parent, i), types[i], action, cascadeTo, cascadeStyles[i]==CascadeStyle.StyleAllGC );
				}
				if ( log.IsDebugEnabled ) log.Debug( "done processing cascades for: " + persister.ClassName );
			}
		}		
	}
}

