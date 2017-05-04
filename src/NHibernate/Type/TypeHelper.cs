using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Intercept;
using NHibernate.Properties;
using NHibernate.Tuple;

namespace NHibernate.Type
{
	/// <summary>
	/// Collection of convenience methods relating to operations across arrays of types...
	/// </summary>
	public static partial class TypeHelper
	{
		public static readonly IType[] EmptyTypeArray = new IType[0];

		/// <summary>Deep copy a series of values from one array to another</summary>
		/// <param name="values">The values to copy (the source)</param>
		/// <param name="types">The value types</param>
		/// <param name="copy">An array indicating which values to include in the copy</param>
		/// <param name="target">The array into which to copy the values</param>
		/// <param name="session">The originating session</param>
		public static void DeepCopy(object[] values, IType[] types, bool[] copy, object[] target, ISessionImplementor session)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (copy[i])
				{
					if (Equals(LazyPropertyInitializer.UnfetchedProperty, values[i]) || Equals(BackrefPropertyAccessor.Unknown, values[i]))
					{
						target[i] = values[i];
					}
					else
					{
						target[i] = types[i].DeepCopy(values[i], session.Factory);
					}
				}
			}
		}
		
		/// <summary>Apply the <see cref="ICacheAssembler.BeforeAssemble" /> operation across a series of values.</summary>
		/// <param name="row">The values</param>
		/// <param name="types">The value types</param>
		/// <param name="session">The originating session</param>
		public static void BeforeAssemble(object[] row, ICacheAssembler[] types, ISessionImplementor session)
		{
			for (int i = 0; i < types.Length; i++)
			{
				if (!Equals(LazyPropertyInitializer.UnfetchedProperty, row[i]) && !Equals(BackrefPropertyAccessor.Unknown, row[i]))
				{
					types[i].BeforeAssemble(row[i], session);
				}
			}
		}
		
		/// <summary>
		/// Apply the <see cref="ICacheAssembler.Assemble" /> operation across a series of values.
		/// </summary>
		/// <param name="row">The values</param>
		/// <param name="types">The value types</param>
		/// <param name="session">The originating session</param>
		/// <param name="owner">The entity "owning" the values</param>
		/// <returns></returns>
		public static object[] Assemble(object[] row, ICacheAssembler[] types, ISessionImplementor session, object owner)
		{
			var assembled = new object[row.Length];
			for (int i = 0; i < row.Length; i++)
			{
				if (Equals(LazyPropertyInitializer.UnfetchedProperty, row[i]) || Equals(BackrefPropertyAccessor.Unknown, row[i]))
				{
					assembled[i] = row[i];
				}
				else
				{
					assembled[i] = types[i].Assemble(row[i], session, owner);
				}
			}
			return assembled;
		}

		/// <summary>Apply the <see cref="ICacheAssembler.Disassemble" /> operation across a series of values.</summary>
		/// <param name="row">The values</param>
		/// <param name="types">The value types</param>
		/// <param name="nonCacheable">An array indicating which values to include in the disassembled state</param>
		/// <param name="session">The originating session</param>
		/// <param name="owner">The entity "owning" the values</param>
		/// <returns> The disassembled state</returns>
		public static object[] Disassemble(object[] row, ICacheAssembler[] types, bool[] nonCacheable, ISessionImplementor session, object owner)
		{
			object[] disassembled = new object[row.Length];
			for (int i = 0; i < row.Length; i++)
			{
				if (nonCacheable != null && nonCacheable[i])
				{
					disassembled[i] = LazyPropertyInitializer.UnfetchedProperty;
				}
				else if (Equals(LazyPropertyInitializer.UnfetchedProperty, row[i]) || Equals(BackrefPropertyAccessor.Unknown, row[i]))
				{
					disassembled[i] = row[i];
				}
				else
				{
					disassembled[i] = types[i].Disassemble(row[i], session, owner);
				}
			}
			return disassembled;
		}
		
		/// <summary>
		/// Apply the <see cref="IType.Replace(object, object, ISessionImplementor, object, IDictionary)" /> operation across a series of values.
		/// </summary>
		/// <param name="original">The source of the state</param>
		/// <param name="target">The target into which to replace the source values.</param>
		/// <param name="types">The value types</param>
		/// <param name="session">The originating session</param>
		/// <param name="owner">The entity "owning" the values</param>
		/// <param name="copiedAlready">Represent a cache of already replaced state</param>
		/// <returns> The replaced state</returns>
		public static object[] Replace(object[] original, object[] target, IType[] types, ISessionImplementor session,
																	 object owner, IDictionary copiedAlready)
		{
			var copied = new object[original.Length];
			for (int i = 0; i < original.Length; i++)
			{
				if (Equals(LazyPropertyInitializer.UnfetchedProperty, original[i]) || Equals(BackrefPropertyAccessor.Unknown, original[i]))
				{
					copied[i] = target[i];
				}
				else if (target[i] == LazyPropertyInitializer.UnfetchedProperty)
				{
					// Should be no need to check for target[i] == PropertyAccessStrategyBackRefImpl.UNKNOWN
					// because PropertyAccessStrategyBackRefImpl.get( object ) returns
					// PropertyAccessStrategyBackRefImpl.UNKNOWN, so target[i] == original[i].
					//
					// We know from above that original[i] != LazyPropertyInitializer.UNFETCHED_PROPERTY &&
					// original[i] != PropertyAccessStrategyBackRefImpl.UNKNOWN;
					// This is a case where the entity being merged has a lazy property
					// that has been initialized. Copy the initialized value from original.
					if (types[i].IsMutable)
					{
						copied[i] = types[i].DeepCopy(original[i], session.Factory);
					}
					else
					{
						copied[i] = original[i];
					}
				}
				else
				{
					copied[i] = types[i].Replace(original[i], target[i], session, owner, copiedAlready);
				}
			}
			return copied;
		}

		/// <summary>
		/// Apply the <see cref="IType.Replace(object, object, ISessionImplementor, object, IDictionary, ForeignKeyDirection)" />
		/// operation across a series of values.
		/// </summary>
		/// <param name="original">The source of the state</param>
		/// <param name="target">The target into which to replace the source values.</param>
		/// <param name="types">The value types</param>
		/// <param name="session">The originating session</param>
		/// <param name="owner">The entity "owning" the values</param>
		/// <param name="copyCache">A map representing a cache of already replaced state</param>
		/// <param name="foreignKeyDirection">FK directionality to be applied to the replacement</param>
		/// <returns> The replaced state</returns>
		public static object[] Replace(object[] original, object[] target, IType[] types,
			ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			object[] copied = new object[original.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (Equals(LazyPropertyInitializer.UnfetchedProperty, original[i]) || Equals(BackrefPropertyAccessor.Unknown, original[i]))
				{
					copied[i] = target[i];
				}
				else
					copied[i] = types[i].Replace(original[i], target[i], session, owner, copyCache, foreignKeyDirection);
			}
			return copied;
		}

		/// <summary>
		/// Apply the <see cref="IType.Replace(object, object, ISessionImplementor, object, IDictionary, ForeignKeyDirection)" />
		/// operation across a series of values, as long as the corresponding <see cref="IType"/> is an association.
		/// </summary>
		/// <param name="original">The source of the state</param>
		/// <param name="target">The target into which to replace the source values.</param>
		/// <param name="types">The value types</param>
		/// <param name="session">The originating session</param>
		/// <param name="owner">The entity "owning" the values</param>
		/// <param name="copyCache">A map representing a cache of already replaced state</param>
		/// <param name="foreignKeyDirection">FK directionality to be applied to the replacement</param>
		/// <returns> The replaced state</returns>
		/// <remarks>
		/// If the corresponding type is a component type, then apply <see cref="ReplaceAssociations" />
		/// across the component subtypes but do not replace the component value itself.
		/// </remarks>
		public static object[] ReplaceAssociations(object[] original, object[] target, IType[] types,
			ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection)
		{
			object[] copied = new object[original.Length];
			for (int i = 0; i < types.Length; i++)
			{
				if (Equals(LazyPropertyInitializer.UnfetchedProperty, original[i]) || Equals(BackrefPropertyAccessor.Unknown, original[i]))
				{
					copied[i] = target[i];
				}
				else if (types[i].IsComponentType)
				{
					// need to extract the component values and check for subtype replacements...
					IAbstractComponentType componentType = (IAbstractComponentType)types[i];
					IType[] subtypes = componentType.Subtypes;
					object[] origComponentValues = original[i] == null ? new object[subtypes.Length] : componentType.GetPropertyValues(original[i], session);
					object[] targetComponentValues = target[i] == null ? new object[subtypes.Length] : componentType.GetPropertyValues(target[i], session);

					object[] componentCopy = ReplaceAssociations(origComponentValues, targetComponentValues, subtypes, session, null, copyCache, foreignKeyDirection);
					
					if (!componentType.IsAnyType && target[i] != null)
						componentType.SetPropertyValues(target[i], componentCopy);
					
					copied[i] = target[i];
				}
				else if (!types[i].IsAssociationType)
				{
					copied[i] = target[i];
				}
				else
				{
					copied[i] = types[i].Replace(original[i], target[i], session, owner, copyCache, foreignKeyDirection);
				}
			}
			return copied;
		}
		
		/// <summary>
		/// <para>Determine if any of the given field values are dirty, returning an array containing
		/// indices of the dirty fields.</para>
		/// <para>If it is determined that no fields are dirty, null is returned.</para>
		/// </summary>
		/// <param name="properties">The property definitions</param>
		/// <param name="currentState">The current state of the entity</param>
		/// <param name="previousState">The baseline state of the entity</param>
		/// <param name="includeColumns">Columns to be included in the dirty checking, per property</param>
		/// <param name="anyUninitializedProperties">Does the entity currently hold any uninitialized property values?</param>
		/// <param name="session">The session from which the dirty check request originated.</param>
		/// <returns>Array containing indices of the dirty properties, or null if no properties considered dirty.</returns>
		public static int[] FindDirty(StandardProperty[] properties,
										object[] currentState,
										object[] previousState,
										bool[][] includeColumns,
										bool anyUninitializedProperties,
										ISessionImplementor session)
		{
			int[] results = null;
			int count = 0;
			int span = properties.Length;

			for (int i = 0; i < span; i++)
			{
				var dirty = Dirty(properties, currentState, previousState, includeColumns, anyUninitializedProperties, session, i);
				if (dirty)
				{
					if (results == null)
					{
						results = new int[span];
					}
					results[count++] = i;
				}
			}
			if (count == 0)
			{
				return null;
			}
			else
			{
				int[] trimmed = new int[count];
				Array.Copy(results, 0, trimmed, 0, count);
				return trimmed;
			}
		}

		private static bool Dirty(StandardProperty[] properties, object[] currentState, object[] previousState, bool[][] includeColumns, bool anyUninitializedProperties, ISessionImplementor session, int i)
		{
			if (Equals(LazyPropertyInitializer.UnfetchedProperty, currentState[i]))
				return false;
			if (Equals(LazyPropertyInitializer.UnfetchedProperty, previousState[i]))
				return true;
			return properties[i].IsDirtyCheckable(anyUninitializedProperties) &&
				   properties[i].Type.IsDirty(previousState[i], currentState[i], includeColumns[i], session);
		}

		/// <summary>
		/// <para>Determine if any of the given field values are modified, returning an array containing
		/// indices of the modified fields.</para>
		/// <para>If it is determined that no fields are dirty, null is returned.</para>
		/// </summary>
		/// <param name="properties">The property definitions</param>
		/// <param name="currentState">The current state of the entity</param>
		/// <param name="previousState">The baseline state of the entity</param>
		/// <param name="includeColumns">Columns to be included in the mod checking, per property</param>
		/// <param name="anyUninitializedProperties">Does the entity currently hold any uninitialized property values?</param>
		/// <param name="session">The session from which the dirty check request originated.</param>
		/// <returns>Array containing indices of the modified properties, or null if no properties considered modified.</returns>
		public static int[] FindModified(StandardProperty[] properties,
											object[] currentState,
											object[] previousState,
											bool[][] includeColumns,
											bool anyUninitializedProperties,
											ISessionImplementor session)
		{
			int[] results = null;
			int count = 0;
			int span = properties.Length;

			for (int i = 0; i < span; i++)
			{
				bool dirty =
					!Equals(LazyPropertyInitializer.UnfetchedProperty, currentState[i]) &&
					properties[i].IsDirtyCheckable(anyUninitializedProperties)
					&& properties[i].Type.IsModified(previousState[i], currentState[i], includeColumns[i], session);

				if (dirty)
				{
					if (results == null)
					{
						results = new int[span];
					}
					results[count++] = i;
				}
			}
			if (count == 0)
			{
				return null;
			}
			else
			{
				int[] trimmed = new int[count];
				Array.Copy(results, 0, trimmed, 0, count);
				return trimmed;
			}
		}
	}
}