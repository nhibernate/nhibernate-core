﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.Tuple;
using NHibernate.Tuple.Component;
using NHibernate.Util;

namespace NHibernate.Type
{
	using System.Threading.Tasks;
	using System.Threading;
	/// <content>
	/// Contains generated async methods
	/// </content>
	public partial class ComponentType : AbstractType, IAbstractComponentType
	{

		public override async Task<bool> IsDirtyAsync(object x, object y, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (x == y)
			{
				return false;
			}
			/* 
			 * NH Different behavior : we don't use the shortcut because NH-1101 
			 * let the tuplizer choose how cosiderer properties when the component is null.
			 */
			if (EntityMode != EntityMode.Poco && (x == null || y == null))
			{
				return true;
			}
			object[] xvalues = GetPropertyValues(x);
			object[] yvalues = GetPropertyValues(y);
			for (int i = 0; i < xvalues.Length; i++)
			{
				if (await (propertyTypes[i].IsDirtyAsync(xvalues[i], yvalues[i], session, cancellationToken)).ConfigureAwait(false))
				{
					return true;
				}
			}
			return false;
		}

		public override async Task<bool> IsDirtyAsync(object x, object y, bool[] checkable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (x == y)
			{
				return false;
			}
			/* 
			 * NH Different behavior : we don't use the shortcut because NH-1101 
			 * let the tuplizer choose how cosiderer properties when the component is null.
			 */
			if (EntityMode != EntityMode.Poco && (x == null || y == null))
			{
				return true;
			}
			object[] xvalues = GetPropertyValues(x);
			object[] yvalues = GetPropertyValues(y);
			int loc = 0;
			for (int i = 0; i < xvalues.Length; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				if (len <= 1)
				{
					bool dirty = (len == 0 || checkable[loc]) &&
								 await (propertyTypes[i].IsDirtyAsync(xvalues[i], yvalues[i], session, cancellationToken)).ConfigureAwait(false);
					if (dirty)
					{
						return true;
					}
				}
				else
				{
					bool[] subcheckable = new bool[len];
					Array.Copy(checkable, loc, subcheckable, 0, len);
					bool dirty = await (propertyTypes[i].IsDirtyAsync(xvalues[i], yvalues[i], subcheckable, session, cancellationToken)).ConfigureAwait(false);
					if (dirty)
					{
						return true;
					}
				}
				loc += len;
			}
			return false;
		}

		public override async Task<object> NullSafeGetAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (ResolveIdentifierAsync(await (HydrateAsync(rs, names, session, owner, cancellationToken)).ConfigureAwait(false), session, owner, cancellationToken)).ConfigureAwait(false);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="st"></param>
		/// <param name="value"></param>
		/// <param name="begin"></param>
		/// <param name="session"></param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
		public override async Task NullSafeSetAsync(DbCommand st, object value, int begin, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object[] subvalues = NullSafeGetValues(value);

			for (int i = 0; i < propertySpan; i++)
			{
				await (propertyTypes[i].NullSafeSetAsync(st, subvalues[i], begin, session, cancellationToken)).ConfigureAwait(false);
				begin += propertyTypes[i].GetColumnSpan(session.Factory);
			}
		}

		public override async Task NullSafeSetAsync(DbCommand st, object value, int begin, bool[] settable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			object[] subvalues = NullSafeGetValues(value);

			int loc = 0;
			for (int i = 0; i < propertySpan; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				if (len == 0)
				{
					//noop
				}
				else if (len == 1)
				{
					if (settable[loc])
					{
						await (propertyTypes[i].NullSafeSetAsync(st, subvalues[i], begin, session, cancellationToken)).ConfigureAwait(false);
						begin++;
					}
				}
				else
				{
					bool[] subsettable = new bool[len];
					Array.Copy(settable, loc, subsettable, 0, len);
					await (propertyTypes[i].NullSafeSetAsync(st, subvalues[i], begin, subsettable, session, cancellationToken)).ConfigureAwait(false);
					begin += ArrayHelper.CountTrue(subsettable);
				}
				loc += len;
			}
		}

		public override Task<object> NullSafeGetAsync(DbDataReader rs, string name, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return NullSafeGetAsync(rs, new string[] {name}, session, owner, cancellationToken);
		}

		public Task<object> GetPropertyValueAsync(object component, int i, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object>(GetPropertyValue(component, i, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<object>(ex);
			}
		}

		public Task<object[]> GetPropertyValuesAsync(object component, ISessionImplementor session, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object[]>(cancellationToken);
			}
			try
			{
				return Task.FromResult<object[]>(GetPropertyValues(component, session));
			}
			catch (Exception ex)
			{
				return Task.FromException<object[]>(ex);
			}
		}

		public override async Task<object> ReplaceAsync(object original, object target, ISessionImplementor session, object owner,
									   IDictionary copiedAlready, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (original == null)
				return null;

			object result = target ?? Instantiate(owner, session);

			object[] values = await (TypeHelper.ReplaceAsync(GetPropertyValues(original), GetPropertyValues(result), propertyTypes, session, owner, copiedAlready, cancellationToken)).ConfigureAwait(false);

			SetPropertyValues(result, values);
			return result;
		}

		public override async Task<object> ReplaceAsync(object original, object target, ISessionImplementor session, object owner, IDictionary copyCache, ForeignKeyDirection foreignKeyDirection, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (original == null)
				return null;

			object result = target ?? Instantiate(owner, session);

			object[] values = await (TypeHelper.ReplaceAsync(GetPropertyValues(original), GetPropertyValues(result), propertyTypes, session, owner, copyCache, foreignKeyDirection, cancellationToken)).ConfigureAwait(false);

			SetPropertyValues(result, values);
			return result;
		}

		public override async Task<object> DisassembleAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (value == null)
			{
				return null;
			}
			else
			{
				object[] values = GetPropertyValues(value);
				for (int i = 0; i < propertyTypes.Length; i++)
				{
					values[i] = await (propertyTypes[i].DisassembleAsync(values[i], session, owner, cancellationToken)).ConfigureAwait(false);
				}
				return values;
			}
		}

		public override async Task<object> AssembleAsync(object obj, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (obj == null)
			{
				return null;
			}
			else
			{
				object[] values = (object[]) obj;
				object[] assembled = new object[values.Length];
				for (int i = 0; i < propertyTypes.Length; i++)
				{
					assembled[i] = await (propertyTypes[i].AssembleAsync(values[i], session, owner, cancellationToken)).ConfigureAwait(false);
				}
				object result = Instantiate(owner, session);
				SetPropertyValues(result, assembled);
				return result;
			}
		}

		public override async Task<object> HydrateAsync(DbDataReader rs, string[] names, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			int begin = 0;
			bool notNull = false;
			object[] values = new object[propertySpan];
			for (int i = 0; i < propertySpan; i++)
			{
				int length = propertyTypes[i].GetColumnSpan(session.Factory);
				string[] range = ArrayHelper.Slice(names, begin, length); //cache this
				object val = await (propertyTypes[i].HydrateAsync(rs, range, session, owner, cancellationToken)).ConfigureAwait(false);
				if (val == null)
				{
					if (isKey)
					{
						return null; //different nullability rules for pk/fk
					}
				}
				else
				{
					notNull = true;
				}
				values[i] = val;
				begin += length;
			}

			if (ReturnedClass.IsValueType)
				return values;
			else
				return notNull ? values : null;
		}

		public override async Task<object> ResolveIdentifierAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (value != null)
			{
				object result = Instantiate(owner, session);
				object[] values = (object[])value;
				object[] resolvedValues = new object[values.Length]; //only really need new array during semiresolve!
				for (int i = 0; i < values.Length; i++)
				{
					resolvedValues[i] = await (propertyTypes[i].ResolveIdentifierAsync(values[i], session, owner, cancellationToken)).ConfigureAwait(false);
				}
				SetPropertyValues(result, resolvedValues);
				return result;
			}
			else
			{
				return null;
			}
		}

		public override Task<object> SemiResolveAsync(object value, ISessionImplementor session, object owner, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			//note that this implementation is kinda broken
			//for components with many-to-one associations
			return ResolveIdentifierAsync(value, session, owner, cancellationToken);
		}

		public override async Task<bool> IsModifiedAsync(object old, object current, bool[] checkable, ISessionImplementor session, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (current == null)
			{
				return old != null;
			}
			if (old == null)
			{
				return current != null;
			}
			object[] currentValues = await (GetPropertyValuesAsync(current, session, cancellationToken)).ConfigureAwait(false);
			object[] oldValues = (Object[]) old;
			int loc = 0;
			for (int i = 0; i < currentValues.Length; i++)
			{
				int len = propertyTypes[i].GetColumnSpan(session.Factory);
				bool[] subcheckable = new bool[len];
				Array.Copy(checkable, loc, subcheckable, 0, len);
				if (await (propertyTypes[i].IsModifiedAsync(oldValues[i], currentValues[i], subcheckable, session, cancellationToken)).ConfigureAwait(false))
				{
					return true;
				}
				loc += len;
			}
			return false;
		}
	}
}