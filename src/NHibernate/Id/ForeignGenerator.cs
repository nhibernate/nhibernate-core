using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
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
	public class ForeignGenerator : IIdentifierGenerator, IConfigurable
	{
		private string propertyName;
		private string entityName;

		#region IIdentifierGenerator Members

		/// <summary>
		/// Generates an identifer from the value of a Property. 
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
			ISession session = (ISession)sessionImplementor;

			object associatedObject = sessionImplementor.Factory
				.GetClassMetadata(obj.GetType())
				.GetPropertyValue(obj, propertyName, sessionImplementor.EntityMode);

			if (associatedObject == null)
			{
				throw new IdentifierGenerationException("attempted to assign id from null one-to-one property: " + propertyName);
			}

			EntityType type = (EntityType)sessionImplementor.Factory.GetClassMetadata(obj.GetType()).GetPropertyType(propertyName);

			object id;
			try
			{
				id = ForeignKeys.GetEntityIdentifierIfNotUnsaved(type.GetAssociatedEntityName(), associatedObject, sessionImplementor);
			}
			catch (TransientObjectException)
			{
				id = session.Save(associatedObject);
			}


			if (session.Contains(obj))
			{
				//abort the save (the object is already saved by a circular cascade)
				return IdentifierGeneratorFactory.ShortCircuitIndicator;
			}

			return id;
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