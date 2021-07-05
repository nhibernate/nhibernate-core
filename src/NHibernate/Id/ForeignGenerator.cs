using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Id
{
	/// <summary>
	/// An <see cref="IIdentifierGenerator" /> that uses the value of 
	/// the id property of an associated object
	/// </summary>
	/// <remarks>
	/// <para>
	///	This id generation strategy is specified in the mapping file as 
	///	<code>
	///	&lt;generator class="foreign"&gt;
	///		&lt;param name="property"&gt;AssociatedObject&lt;/param&gt;
	///	&lt;/generator&gt;
	///	</code>
	/// </para>
	/// The mapping parameter <c>property</c> is required.
	/// </remarks>
	public partial class ForeignGenerator : IIdentifierGenerator, IConfigurable
	{
		private string propertyName;
		private string entityName;

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generates an identifier from the value of a Property. 
		/// </summary>
		/// <param name="sessionImplementor">The <see cref="ISessionImplementor"/> this id is being generated in.</param>
		/// <param name="obj">The entity for which the id is being generated.</param>
		/// <returns>
		/// The identifier value from the associated object or  
		/// <see cref="IdentifierGeneratorFactory.ShortCircuitIndicator"/> if the <c>session</c>
		/// already contains <c>obj</c>.
		/// </returns>
		public object Generate(ISessionImplementor sessionImplementor, object obj)
		{
			var persister = sessionImplementor.Factory.GetEntityPersister(entityName);
			object associatedObject = persister.GetPropertyValue(obj, propertyName);

			if (associatedObject == null)
			{
				throw new IdentifierGenerationException("attempted to assign id from null one-to-one property: " + propertyName);
			}

			var foreignValueSourceType = GetForeignValueSourceType(persister);

			object id;
			try
			{
				id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(
					foreignValueSourceType.GetAssociatedEntityName(),
					associatedObject,
					sessionImplementor);
			}
			catch (TransientObjectException)
			{
				if (sessionImplementor is ISession session)
				{
					id = session.Save(foreignValueSourceType.GetAssociatedEntityName(), associatedObject);
				}
				else if (sessionImplementor is IStatelessSession statelessSession)
				{
					id = statelessSession.Insert(foreignValueSourceType.GetAssociatedEntityName(), associatedObject);
				}
				else
				{
					throw new IdentifierGenerationException("sessionImplementor is neither Session nor StatelessSession");
				}
			}

			if (Contains(sessionImplementor, obj))
			{
				//abort the save (the object is already saved by a circular cascade)
				return IdentifierGeneratorFactory.ShortCircuitIndicator;
			}

			return id;
		}

		private EntityType GetForeignValueSourceType(IEntityPersister persister)
		{
			var propertyType = persister.GetPropertyType(propertyName);
			if (propertyType.IsEntityType)
			{
				return (EntityType) propertyType;
			}

			// try identifier mapper
			return (EntityType) persister.GetPropertyType("_identifierMapper." + propertyName);
		}

		private static bool Contains(ISessionImplementor sessionImplementor, object obj)
		{
			return sessionImplementor is ISession session && session.Contains(obj);
		}

		#endregion

		#region IConfigurable Members

		/// <summary>
		/// Configures the ForeignGenerator by reading the value of <c>property</c> 
		/// from the <c>parms</c> parameter.
		/// </summary>
		/// <param name="type">The <see cref="IType"/> the identifier should be.</param>
		/// <param name="parms">An <see cref="IDictionary"/> of Param values that are keyed by parameter name.</param>
		/// <param name="dialect">The <see cref="Dialect.Dialect"/> to help with Configuration.</param>
		/// <exception cref="MappingException">
		/// Thrown if the key <c>property</c> is not found in the <c>parms</c> parameter.
		/// </exception>
		public void Configure(IType type, IDictionary<string, string> parms, Dialect.Dialect dialect)
		{
			parms.TryGetValue(IdGeneratorParmsNames.EntityName, out entityName);
			parms.TryGetValue("property", out propertyName);
			if (propertyName == null || propertyName.Length == 0)
			{
				throw new MappingException("param named \"property\" is required for foreign id generation strategy");
			}
		}

		#endregion
	}
}
