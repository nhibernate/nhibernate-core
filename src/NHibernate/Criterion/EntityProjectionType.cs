using System;
using System.Data.Common;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Criterion
{
	/// <summary>
	/// Specialized type for retrieving entity from Criteria/QueryOver API projection.
	/// Intended to be used only inside <see cref="EntityProjection"/>
	/// </summary>
	[Serializable]
	internal partial class EntityProjectionType : ManyToOneType, IType
	{
		private readonly EntityProjection _projection;

		public EntityProjectionType(EntityProjection projection) : base(projection.RootEntity.FullName, projection.Lazy)
		{
			_projection = projection;
		}

		object IType.NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
		{
			//names parameter is ignored (taken from projection)
			return NullSafeGet(rs, string.Empty, session, owner);
		}

		public override object NullSafeGet(DbDataReader rs, string name, ISessionImplementor session, object owner)
		{
			var identifier = _projection.Persister.IdentifierType.NullSafeGet(rs, _projection.IdentifierColumnAliases, session, null);

			if (identifier == null)
			{
				return null;
			}

			return _projection.Lazy
				? ResolveIdentifier(identifier, session)
				: GetInitializedEntityFromProjection(rs, session, identifier);
		}

		private object GetInitializedEntityFromProjection(DbDataReader rs, ISessionImplementor session, object identifier)
		{
			var entity = CreateInitializedEntity(
				rs,
				session,
				_projection.Persister,
				identifier,
				_projection.PropertyColumnAliases,
				LockMode.None,
				_projection.FetchLazyProperties,
				_projection.IsReadOnly);

			return entity;
		}

		private static object CreateInitializedEntity(DbDataReader rs, ISessionImplementor session, IQueryable persister, object identifier, string[][] propertyAliases, LockMode lockMode, bool fetchLazyProperties, bool readOnly)
		{
			var eventSource = session as IEventSource;
			PostLoadEvent postLoadEvent = null;
			PreLoadEvent preLoadEvent = null;
			object entity;
			if (eventSource != null)
			{
				preLoadEvent = new PreLoadEvent(eventSource);
				postLoadEvent = new PostLoadEvent(eventSource);
				entity = eventSource.Instantiate(persister, identifier);
			}
			else
			{
				entity = session.Instantiate(persister.EntityName, identifier);
			}

			TwoPhaseLoad.AddUninitializedEntity(
				session.GenerateEntityKey(identifier, persister),
				entity,
				persister,
				lockMode,
				!fetchLazyProperties,
				session);

			var hydrated = persister.Hydrate(
				rs,
				identifier,
				entity,
				persister,
				propertyAliases,
				fetchLazyProperties,
				session);

			TwoPhaseLoad.PostHydrate(persister, identifier, hydrated, null, entity, lockMode, !fetchLazyProperties, session);
			TwoPhaseLoad.InitializeEntity(entity, readOnly, session, preLoadEvent, postLoadEvent);
			return entity;
		}
	}
}
