using System;
using System.Collections;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Proxy;
using NHibernate.Type;

namespace NHibernate.Util
{
	internal static class ParameterHelper
	{
		/// <summary>
		/// Guesses the <see cref="IType"/> from the <c>param</c>'s value.
		/// </summary>
		/// <param name="param">The object to guess the <see cref="IType"/> of.</param>
		/// <param name="sessionFactory">The session factory to search for entity persister.</param>
		/// <param name="isCollection">Whether <paramref name="param"/> is a collection.</param>
		/// <returns>An <see cref="IType"/> for the object.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>param</c> is null because the <see cref="IType"/>
		/// can't be guess from a null value.
		/// </exception>
		public static IType TryGuessType(object param, ISessionFactoryImplementor sessionFactory, bool isCollection)
		{
			if (param == null)
			{
				return null;
			}

			if (param is IEnumerable enumerable && isCollection)
			{
				var firstValue = enumerable.Cast<object>().FirstOrDefault();
				return firstValue == null
					? TryGuessType(enumerable.GetCollectionElementType(), sessionFactory)
					: TryGuessType(firstValue, sessionFactory, false);
			}

			var clazz = NHibernateProxyHelper.GetClassWithoutInitializingProxy(param);
			return TryGuessType(clazz, sessionFactory);
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <c>param</c>'s value.
		/// </summary>
		/// <param name="param">The object to guess the <see cref="IType"/> of.</param>
		/// <param name="sessionFactory">The session factory to search for entity persister.</param>
		/// <returns>An <see cref="IType"/> for the object.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>param</c> is null because the <see cref="IType"/>
		/// can't be guess from a null value.
		/// </exception>
		public static IType GuessType(object param, ISessionFactoryImplementor sessionFactory)
		{
			if (param == null)
			{
				throw new ArgumentNullException(nameof(param), "The IType can not be guessed for a null value.");
			}

			System.Type clazz = NHibernateProxyHelper.GetClassWithoutInitializingProxy(param);
			return GuessType(clazz, sessionFactory);
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to guess the <see cref="IType"/> of.</param>
		/// <param name="sessionFactory">The session factory to search for entity persister.</param>
		/// <param name="isCollection">Whether <paramref name="clazz"/> is a collection.</param>
		/// <returns>An <see cref="IType"/> for the <see cref="System.Type"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>clazz</c> is null because the <see cref="IType"/>
		/// can't be guess from a null type.
		/// </exception>
		public static IType TryGuessType(System.Type clazz, ISessionFactoryImplementor sessionFactory, bool isCollection)
		{
			if (clazz == null)
			{
				return null;
			}

			if (isCollection)
			{
				return TryGuessType(ReflectHelper.GetCollectionElementType(clazz), sessionFactory, false);
			}

			return TryGuessType(clazz, sessionFactory);
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to guess the <see cref="IType"/> of.</param>
		/// <param name="sessionFactory">The session factory to search for entity persister.</param>
		/// <returns>An <see cref="IType"/> for the <see cref="System.Type"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>clazz</c> is null because the <see cref="IType"/>
		/// can't be guess from a null type.
		/// </exception>
		public static IType GuessType(System.Type clazz, ISessionFactoryImplementor sessionFactory)
		{
			if (clazz == null)
			{
				throw new ArgumentNullException(nameof(clazz), "The IType can not be guessed for a null value.");
			}

			return TryGuessType(clazz, sessionFactory) ??
					throw new HibernateException("Could not determine a type for class: " + clazz.AssemblyQualifiedName);
		}

		/// <summary>
		/// Guesses the <see cref="IType"/> from the <see cref="System.Type"/>.
		/// </summary>
		/// <param name="clazz">The <see cref="System.Type"/> to guess the <see cref="IType"/> of.</param>
		/// <param name="sessionFactory">The session factory to search for entity persister.</param>
		/// <returns>An <see cref="IType"/> for the <see cref="System.Type"/>.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when the <c>clazz</c> is null because the <see cref="IType"/>
		/// can't be guess from a null type.
		/// </exception>
		public static IType TryGuessType(System.Type clazz, ISessionFactoryImplementor sessionFactory)
		{
			if (clazz == null)
			{
				return null;
			}

			var type = TypeFactory.HeuristicType(clazz);
			if (type == null || type is SerializableType)
			{
				if (sessionFactory.TryGetEntityPersister(clazz.FullName) != null)
				{
					return NHibernateUtil.Entity(clazz);
				}
			}

			return type;
		}
	}
}
