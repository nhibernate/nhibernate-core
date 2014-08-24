using System;
using System.Collections.Generic;
using NHibernate.Util;

namespace NHibernate.Tuple
{
	/// <summary> Centralizes handling of <see cref="EntityMode"/> to <see cref="ITuplizer"/> mappings. </summary>
	[Serializable]
	public abstract class EntityModeToTuplizerMapping
	{
		// NH-1660
		private readonly IDictionary<EntityMode, ITuplizer> tuplizers
												= new LinkedHashMap<EntityMode, ITuplizer>(5, new EntityModeEqualityComparer());

		protected internal void AddTuplizer(EntityMode entityMode, ITuplizer tuplizer)
		{
			tuplizers[entityMode] = tuplizer;
		}

		/// <summary> Given a supposed instance of an entity/component, guess its entity mode. </summary>
		/// <param name="obj">The supposed instance of the entity/component.</param>
		/// <returns> The guessed entity mode. </returns>
		public virtual EntityMode? GuessEntityMode(object obj)
		{
			foreach (KeyValuePair<EntityMode, ITuplizer> entry in tuplizers)
			{
				ITuplizer tuplizer = entry.Value;
				if (tuplizer.IsInstance(obj))
				{
					return entry.Key;
				}
			}
			return null;
		}

		/// <summary> 
		/// Locate the contained tuplizer responsible for the given entity-mode.  If
		/// no such tuplizer is defined on this mapping, then return null. 
		/// </summary>
		/// <param name="entityMode">The entity-mode for which the caller wants a tuplizer. </param>
		/// <returns> The tuplizer, or null if not found. </returns>
		public virtual ITuplizer GetTuplizerOrNull(EntityMode entityMode)
		{
			ITuplizer result;
			tuplizers.TryGetValue(entityMode, out result);
			return result;
		}

		/// <summary> Locate the tuplizer contained within this mapping which is responsible
		/// for the given entity-mode.  If no such tuplizer is defined on this
		/// mapping, then an exception is thrown.
		/// 
		/// </summary>
		/// <param name="entityMode">The entity-mode for which the caller wants a tuplizer.
		/// </param>
		/// <returns> The tuplizer.
		/// </returns>
		/// <throws>  HibernateException Unable to locate the requested tuplizer. </throws>
		public virtual ITuplizer GetTuplizer(EntityMode entityMode)
		{
			ITuplizer tuplizer = GetTuplizerOrNull(entityMode);
			if (tuplizer == null)
			{
				throw new HibernateException("No tuplizer found for entity-mode [" + entityMode + "]");
			}
			return tuplizer;
		}
	}
}
