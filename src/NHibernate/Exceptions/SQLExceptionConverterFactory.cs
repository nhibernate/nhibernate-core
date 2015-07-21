using System;
using System.Collections.Generic;
using System.Reflection;

using NHibernate.Util;

namespace NHibernate.Exceptions
{
	/// <summary> A factory for building SQLExceptionConverter instances. </summary>
	public static class SQLExceptionConverterFactory
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(SQLExceptionConverterFactory));

		private class MinimalSQLExceptionConverter : ISQLExceptionConverter
		{
			#region ISQLExceptionConverter Members

			public Exception Convert(AdoExceptionContextInfo exceptionContextInfo)
			{
				throw new GenericADOException(exceptionContextInfo.Message, exceptionContextInfo.SqlException, exceptionContextInfo.Sql);
			}

			#endregion
		}

		/// <summary> Build a SQLExceptionConverter instance. </summary>
		/// <param name="dialect">The defined dialect. </param>
		/// <param name="properties">The configuration properties. </param>
		/// <returns> An appropriate <see cref="ISQLExceptionConverter"/> instance. </returns>
		/// <remarks>
		/// First, looks for a <see cref="Cfg.Environment.SqlExceptionConverter"/> property to see
		/// if the configuration specified the class of a specific converter to use.  If this
		/// property is set, attempt to construct an instance of that class.  If not set, or
		/// if construction fails, the converter specific to the dialect will be used.
		/// </remarks>
		public static ISQLExceptionConverter BuildSQLExceptionConverter(Dialect.Dialect dialect, IDictionary<string, string> properties)
		{
			ISQLExceptionConverter converter = null;

			string converterClassName;
			properties.TryGetValue(Cfg.Environment.SqlExceptionConverter, out converterClassName);
			if (!string.IsNullOrEmpty(converterClassName))
			{
				converter = ConstructConverter(converterClassName, dialect.ViolatedConstraintNameExtracter);
			}

			if (converter == null)
			{
				log.Info("Using dialect defined converter");
				converter = dialect.BuildSQLExceptionConverter();
			}

			IConfigurable confConv = converter as IConfigurable;
			if (confConv != null)
			{
				try
				{
					confConv.Configure(properties);
				}
				catch (HibernateException e)
				{
					log.Warn("Unable to configure SQLExceptionConverter", e);
					throw;
				}
			}

			return converter;
		}

		/// <summary> 
		/// Builds a minimal converter.  The instance returned here just always converts to <see cref="GenericADOException"/>. 
		/// </summary>
		/// <returns> The minimal converter. </returns>
		public static ISQLExceptionConverter BuildMinimalSQLExceptionConverter()
		{
			return new MinimalSQLExceptionConverter();
		}

		private static ISQLExceptionConverter ConstructConverter(string converterClassName, IViolatedConstraintNameExtracter violatedConstraintNameExtracter)
		{
			try
			{
				log.Debug("Attempting to construct instance of specified SQLExceptionConverter [" + converterClassName + "]");
				System.Type converterClass = ReflectHelper.ClassForName(converterClassName);

				// First, try to find a matching constructor accepting a ViolatedConstraintNameExtracter param...
				ConstructorInfo[] ctors = converterClass.GetConstructors(ReflectHelper.AnyVisibilityInstance);
				foreach (ConstructorInfo ctor in ctors)
				{
					ParameterInfo[] parameters = ctor.GetParameters();

					if (parameters == null || parameters.Length != 1) continue;

					if (typeof(IViolatedConstraintNameExtracter).IsAssignableFrom(parameters[0].ParameterType))
					{
						try
						{
							return (ISQLExceptionConverter)ctor.Invoke(new object[] { violatedConstraintNameExtracter });
						}
						catch (Exception)
						{
							// eat it and try next
						}
					}
				}

				// Otherwise, try to use the no-arg constructor
				return (ISQLExceptionConverter) Cfg.Environment.BytecodeProvider.ObjectsFactory.CreateInstance(converterClass);
			}
			catch (Exception t)
			{
				log.Warn("Unable to construct instance of specified SQLExceptionConverter", t);
			}

			return null;
		}
	}
}
