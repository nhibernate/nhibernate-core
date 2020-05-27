using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Exceptions;
using NHibernate.Mapping.ByCode;
using NHibernate.SqlTypes;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;
using Environment = System.Environment;

namespace NHibernate.Test.DriverTest
{
	public class DriverNumericTypesFixture : TestCaseMappingByCode
	{
		private static readonly MethodInfo SelectDefinition =
			ReflectHelper.GetMethodDefinition(() => Queryable.Select(null, default(Expression<Func<object, object>>)));
		private static readonly MethodInfo FirstDefinition =
			ReflectHelper.GetMethodDefinition(() => Queryable.First<object>(null));
		private static readonly MethodInfo LambdaDefinition =
			ReflectHelper.GetMethodDefinition(() => Expression.Lambda<object>(null));
		private static readonly ObjectType ObjectTypeInstance = new ObjectType();
		private static readonly Dictionary<IType, HashSet<System.Type>> DriverNotSupportedTypes =
			new Dictionary<IType, HashSet<System.Type>>
			{
				{
					NHibernateUtil.UInt16, new HashSet<System.Type>
					{
						typeof(OracleManagedDataClientDriver),
						typeof(Sql2008ClientDriver),
						typeof(NpgsqlDriver),
						typeof(SqlServerCeDriver),
						typeof(FirebirdClientDriver),
						typeof(OdbcDriver)
					}
				},
				{
					NHibernateUtil.UInt32, new HashSet<System.Type>
					{
						typeof(OracleManagedDataClientDriver),
						typeof(Sql2008ClientDriver),
						typeof(NpgsqlDriver),
						typeof(SqlServerCeDriver),
						typeof(FirebirdClientDriver),
						typeof(OdbcDriver)
					}
				},
				{
					NHibernateUtil.UInt64, new HashSet<System.Type>
					{
						typeof(OracleManagedDataClientDriver),
						typeof(Sql2008ClientDriver),
						typeof(NpgsqlDriver),
						typeof(SqlServerCeDriver),
						typeof(FirebirdClientDriver),
						typeof(OdbcDriver)
					}
				}
			};

		private static readonly Dictionary<System.Type, HashSet<OperatorTest>> AllowedDriverDifferentValues =
			new Dictionary<System.Type, HashSet<OperatorTest>>
			{
				{
					// This tests should be enabled when MySqlDataDriver will support prepared statements.
					typeof(MySqlDataDriver),
					new HashSet<OperatorTest>
					{
						new OperatorTest('%', nameof(NumericEntity.Float), nameof(NumericEntity.Float), typeof(float), true, false),
						new OperatorTest('%', nameof(NumericEntity.Float), nameof(NumericEntity.Float), typeof(float), false, true),
						new OperatorTest('-', nameof(NumericEntity.Float), nameof(NumericEntity.Float), typeof(float), true, false),
						new OperatorTest('-', nameof(NumericEntity.Float), nameof(NumericEntity.Float), typeof(float), false, true)
					}
				}
			};

		private static string GetColumnName(string propertyName)
		{
			return propertyName + "Column";
		}

		private readonly HashSet<string> _ignoreProperties = new HashSet<string> {nameof(NumericEntity.Id)};
		private readonly NumericEntity _originalEntity = new NumericEntity
		{
			Short = 123,
			Integer = 12345,
			Long = 1234567L,
			UnsignedShort = 123,
			UnsignedInteger = 12345,
			UnsignedLong = 1234567L,
			Currency = 12345.5432m,
			Decimal = 1234567.54321m,
			DecimalLowScale = 123.32m,
			// The floating-point numbers where carefully selected so that they will stay the same when they are saved
			// into the database. From testing, MySql seem the most problematic in term of compatibility with .NET
			// as the values are differently rounded, which causes TestTypesAndValuesAfterSave to fail. For instance, MySql
			// will throw when float.MaxValue is stored (https://stackoverflow.com/a/18832334), due to different rounding.
			Double = 3.1415926535897932E+30,
			Float = 3.14159E+15f
		};
		private List<PropertyMetadata> _properties;

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();
			var driverType = ReflectHelper.ClassForName(cfg.GetProperty(Cfg.Environment.ConnectionDriver));

			mapper.Class<NumericEntity>(o =>
			{
				o.Table(nameof(NumericEntity));
				o.EntityName(nameof(NumericEntity));
				o.Id(x => x.Id, map => map.Generator(Generators.Native));
				o.Property(
					x => x.Short,
					map =>
					{
						map.Type(NHibernateUtil.Int16);
						map.Column(GetColumnName(nameof(NumericEntity.Short)));
					});
				o.Property(
					x => x.Integer,
					map =>
					{
						map.Type(NHibernateUtil.Int32);
						map.Column(GetColumnName(nameof(NumericEntity.Integer)));
					});
				o.Property(
					x => x.Long,
					map =>
					{
						map.Type(NHibernateUtil.Int64);
						map.Column(GetColumnName(nameof(NumericEntity.Long)));
					});

				if (DriverNotSupportedTypes[NHibernateUtil.UInt16].Contains(driverType))
				{
					_ignoreProperties.Add(nameof(NumericEntity.UnsignedShort));
				}
				else
				{
					o.Property(
						x => x.UnsignedShort,
						map =>
						{
							map.Type(NHibernateUtil.UInt16);
							map.Column(GetColumnName(nameof(NumericEntity.UnsignedShort)));
						});
				}

				if (DriverNotSupportedTypes[NHibernateUtil.UInt32].Contains(driverType))
				{
					_ignoreProperties.Add(nameof(NumericEntity.UnsignedInteger));
				}
				else
				{
					o.Property(
						x => x.UnsignedInteger,
						map =>
						{
							map.Type(NHibernateUtil.UInt32);
							map.Column(GetColumnName(nameof(NumericEntity.UnsignedInteger)));
						});
				}

				if (DriverNotSupportedTypes[NHibernateUtil.UInt64].Contains(driverType))
				{
					_ignoreProperties.Add(nameof(NumericEntity.UnsignedLong));
				}
				else
				{
					o.Property(
						x => x.UnsignedLong,
						map =>
						{
							map.Type(NHibernateUtil.UInt64);
							map.Column(GetColumnName(nameof(NumericEntity.UnsignedLong)));
						});
				}

				o.Property(
					x => x.Decimal,
					map =>
					{
						map.Type(NHibernateUtil.Decimal);
						map.Column(GetColumnName(nameof(NumericEntity.Decimal)));
					});
				o.Property(
					x => x.DecimalLowScale,
					map =>
					{
						map.Type(NHibernateUtil.Decimal);
						map.Precision(5);
						map.Scale(2);
						map.Column(GetColumnName(nameof(NumericEntity.DecimalLowScale)));
					});
				o.Property(
					x => x.Currency,
					map =>
					{
						map.Type(NHibernateUtil.Currency);
						map.Column(GetColumnName(nameof(NumericEntity.Currency)));
					});
				o.Property(
					x => x.Double,
					map =>
					{
						map.Type(NHibernateUtil.Double);
						map.Column(GetColumnName(nameof(NumericEntity.Double)));
					});
				o.Property(
					x => x.Float,
					map =>
					{
						map.Type(NHibernateUtil.Single);
						map.Column(GetColumnName(nameof(NumericEntity.Float)));
					});
			});

			_properties = typeof(NumericEntity)
			                 .GetProperties()
			                 .Where(o => !_ignoreProperties.Contains(o.Name))
			                 .Select(o => new PropertyMetadata(o))
			                 .ToList();

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.SetProperty(Cfg.Environment.OracleUseBinaryFloatingPointTypes, "true");
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(_originalEntity);
				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			base.OnTearDown();

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.CreateQuery("delete from NumericEntity").ExecuteUpdate();
				t.Commit();
			}
		}

		/// <summary>
		/// Tested drivers:
		/// - SqlServerCeDriver (net461)
		/// - OracleManagedDataClientDriver (net461, netcoreapp2.0)
		/// - OracleClientDriver (netcoreapp2.0)
		/// - OdbcDriver - SqlServer (net461, netcoreapp2.0)
		/// - Sql2008ClientDriver (net461, netcoreapp2.0)
		/// - NpgsqlDriver (net461, netcoreapp2.0)
		/// - MySqlDataDriver (net461, netcoreapp2.0)
		/// - FirebirdClientDriver (net461, netcoreapp2.0)
		/// - SQLite20Driver (net461, netcoreapp2.0)
		/// The following drivers fails the test:
		/// - OracleManagedDataClientDriver:
		///   1)   Property 'Short' returned type is not the same as original
		///        Expected: System.Int16
		///        But was:  System.Int32
		///   2)   Property 'Integer' returned type is not the same as original
		///        Expected: System.Int32
		///        But was:  System.Int64
		///   3)   Property 'Long' returned type is not the same as original
		///        Expected: System.Int64
		///        But was:  System.Decimal
		///   4)   Property 'DecimalLowScale' returned type is not the same as original
		///        Expected: System.Decimal
		///        But was:  System.Single
		/// - MySqlDataDriver:
		///   1)   Property 'Float' value is not the same as original
		///        Expected: 3.14159275E+15f
		///        But was:  3.14159007E+15f
		/// - SQLite20Driver:
		///   1)   Property 'UnsignedShort' returned type is not the same as original
		///        Expected: System.UInt16
		///        But was:  System.Int64
		///   2)   Property 'UnsignedInteger' returned type is not the same as original
		///        Expected: System.UInt32
		///        But was:  System.Int64>
		///   3)   Property 'UnsignedLong' returned type is not the same as original
		///        Expected: System.UInt64
		///        But was:  System.Int64
		///   4)   Property 'Decimal' returned type is not the same as original
		///        Expected: System.Decimal
		///        But was:  System.Double
		///   5)   Property 'DecimalLowScale' returned type is not the same as original
		///        Expected: System.Decimal
		///        But was:  System.Double
		///   6)   Property 'Currency' returned type is not the same as original
		///        Expected: System.Decimal
		///        But was:  System.Double
		///   7)   Property 'Float' returned type is not the same as original
		///        Expected: System.Single
		///        But was:  System.Double
		/// </summary>
		[Explicit]
		public void TestRawTypesAndValuesAfterSave()
		{
			TestTypesAndValuesAfterSave(true);
		}

		[Test]
		public void TestTypesAndValuesAfterSave()
		{
			TestTypesAndValuesAfterSave(false);
		}

		private void TestTypesAndValuesAfterSave(bool testRawValue)
		{
			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				var selectColumns = string.Join(",", _properties.Select(o => o.ColumnName));
				var query = session.CreateSQLQuery($"select {selectColumns} from NumericEntity");
				foreach (var property in _properties)
				{
					query.AddScalar(property.ColumnName, testRawValue ? ObjectTypeInstance : NHibernateUtil.GuessType(property.Type));
				}

				var result = (object[]) query.UniqueResult();
				Assert.Multiple(() =>
				{
					for (var i = 0; i < _properties.Count; i++)
					{
						var value = result[i];
						var property = _properties[i];
						Assert.That(value.GetType(), Is.EqualTo(property.Type), $"Property '{property.Name}' returned type is not the same as original");
						Assert.That(value, Is.EqualTo(property.GetValue(_originalEntity)), $"Property '{property.Name}' value is not the same as original");
					}
				});

				transaction.Commit();
			}
		}

		/// <summary>
		/// Tested driver (testRawValue=false was used):
		/// - SqlServerCeDriver (net461)
		/// - OracleManagedDataClientDriver (net461, netcoreapp2.0)
		/// - OracleClientDriver (net461, netcoreapp2.0)
		/// - OdbcDriver - SqlServer (net461, netcoreapp2.0)
		/// - Sql2008ClientDriver (net461, netcoreapp2.0)
		/// - NpgsqlDriver (net461, netcoreapp2.0)
		/// - MySqlDataDriver (net461, netcoreapp2.0)
		/// - FirebirdClientDriver (net461, netcoreapp2.0)
		/// - SQLite20Driver (net461, netcoreapp2.0)
		/// The following drivers fails the test:
		/// - MySqlDataDriver:
		///   1)   Expression 'pFloat % Float' value is not as expected
		///        Expected: 0.0f
		///        But was:  3.14159007E+15f
		///   2)   Expression 'Float % pFloat' value is not as expected
		///        Expected: 0.0f
		///        But was:  67445760.0f
		///   3)   Expression 'pFloat - Float' value is not as expected
		///        Expected: 0.0f
		///        But was:  -67445760.0f
		///   4)   Expression 'Float - pFloat' value is not as expected
		///        Expected: 0.0f
		///        But was:  67445760.0f
		/// </summary>
		/// <param name="testRawValue">Whether to execute a sql query without adding casts and without having a <see cref="IType"/> that alters
		/// the returned value form the driver. Use this in order to check what value and type is returned for a specific arithmetic operation
		/// by the driver.</param>
		[TestCase(false)]
		public void TestArithmeticOperators(bool testRawValue)
		{
			var tests = new List<OperatorTest>();
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Short), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Integer), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Long), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.UnsignedShort), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.UnsignedInteger), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.Short), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Integer), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Long), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.UnsignedShort), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.UnsignedInteger), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.Integer), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.Long), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.UnsignedShort), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.UnsignedInteger), typeof(long), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.Long), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.UnsignedShort), typeof(int), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.UnsignedInteger), typeof(uint), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.UnsignedLong), typeof(ulong), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedShort), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.UnsignedInteger), typeof(uint), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.UnsignedLong), typeof(ulong), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedInteger), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.UnsignedLong), typeof(ulong), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.Currency), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.UnsignedLong), nameof(NumericEntity.Float), typeof(float), tests);

			FillOperationTests(nameof(NumericEntity.Decimal), nameof(NumericEntity.Decimal), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Decimal), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.Decimal), nameof(NumericEntity.Currency), typeof(decimal), tests);

			FillOperationTests(nameof(NumericEntity.DecimalLowScale), nameof(NumericEntity.DecimalLowScale), typeof(decimal), tests);
			FillOperationTests(nameof(NumericEntity.DecimalLowScale), nameof(NumericEntity.Currency), typeof(decimal), tests);

			FillOperationTests(nameof(NumericEntity.Currency), nameof(NumericEntity.Currency), typeof(decimal), tests);

			FillOperationTests(nameof(NumericEntity.Double), nameof(NumericEntity.Double), typeof(double), tests);
			FillOperationTests(nameof(NumericEntity.Double), nameof(NumericEntity.Float), typeof(double), tests);

			FillOperationTests(nameof(NumericEntity.Float), nameof(NumericEntity.Float), typeof(float), tests);

			using (var session = OpenSession())
			using (var transaction = session.BeginTransaction())
			{
				TestOperators(tests, testRawValue, session);
				transaction.Commit();
			}
		}

		private static void FillOperationTests(
			string leftPropertyName,
			string rightPropertyName,
			System.Type operatorType,
			List<OperatorTest> tests)
		{
			FillOperationTests('+', leftPropertyName, rightPropertyName, operatorType, tests);
			FillOperationTests('-', leftPropertyName, rightPropertyName, operatorType, tests);
			FillOperationTests('*', leftPropertyName, rightPropertyName, operatorType, tests);
			FillOperationTests('/', leftPropertyName, rightPropertyName, operatorType, tests);
			FillOperationTests('%', leftPropertyName, rightPropertyName, operatorType, tests);
		}

		private static void FillOperationTests(
			char @operator,
			string leftPropertyName,
			string rightPropertyName,
			System.Type operatorType,
			List<OperatorTest> tests)
		{
			tests.Add(new OperatorTest(@operator, leftPropertyName, rightPropertyName, operatorType, false, false));
			tests.Add(new OperatorTest(@operator, leftPropertyName, rightPropertyName, operatorType, true, false));
			tests.Add(new OperatorTest(@operator, leftPropertyName, rightPropertyName, operatorType, false, true));
		}

		private void TestOperators(List<OperatorTest> tests, bool testRawValue, ISession session)
		{
			Console.WriteLine($"Total tests: {tests.Count}");
			Assert.Multiple(
				() =>
				{
					var skippedTests = 0;
					foreach (var test in tests)
					{
						var leftProperty = _properties.FirstOrDefault(o => o.Name == test.LeftPropertyName);
						var rightProperty = _properties.FirstOrDefault(o => o.Name == test.RightPropertyName);
						if (leftProperty == null || 
						    rightProperty == null)
						{
							skippedTests++;
							continue; // Not supported
						}

						if (AllowedDriverDifferentValues.TryGetValue(Sfi.ConnectionProvider.Driver.GetType(), out var allowedFailures) &&
						    allowedFailures.Contains(test))
						{
							skippedTests++;
							continue;
						}

						var lambda = GetLambdaExpression(leftProperty, test, rightProperty);
						object expectedResult;
						try
						{
							expectedResult = lambda.Compile().DynamicInvoke(_originalEntity);
						}
						catch (TargetInvocationException e) when (e.InnerException is OverflowException)
						{
							skippedTests++;
							continue; // Skip overflows (e.g. UnsignedShort - UnsignedInteger)
						}

						TestOperator(testRawValue, session, leftProperty, test, rightProperty, expectedResult);
					}

					Console.WriteLine($"Skipped tests: {skippedTests}");
				});
		}

		private void TestOperator(
			bool testRawValue,
			ISession session,
			PropertyMetadata leftProperty,
			OperatorTest test,
			PropertyMetadata rightProperty,
			object expectedResult)
		{
			object result;
			var selectExpression =
				$"{(test.LeftAsParameter ? $"p{test.LeftPropertyName}" : test.LeftPropertyName)} " +
				$"{test.Operator} " +
				$"{(test.RightAsParameter ? $"p{test.RightPropertyName}" : test.RightPropertyName)}";
			try
			{
				result = testRawValue 
					? ExecuteSqlQuery(true, session, leftProperty, test, rightProperty)
					: ExecuteLinqQuery(session, leftProperty, test, rightProperty);
			}
			catch (GenericADOException e)
			{
				Assert.Fail($"  Expression '{selectExpression}' failed to execute.{Environment.NewLine}  {e}");
				return;
			}

			// Don't assert the type as there will be a lot of failures, the important thing is that the value is correct.
			//Assert.That(result.GetType(), Is.EqualTo(expectedResult.GetType()), $"Expression '{select}' returned type is not as expected");
			try
			{
				Assert.That(result, Is.EqualTo(expectedResult), $"Expression '{selectExpression}' value is not as expected");
			}
			catch (OverflowException) // Can happen when a negative value is returned for an unsigned number
			{
				// Generate the same message as NUnit
				Assert.Fail(
					$"  Expression '{selectExpression}' value is not as expected." + Environment.NewLine +
					$"  Expected: {expectedResult}" + Environment.NewLine +
					$"  But was: {result}" + Environment.NewLine);
			}
		}

		private object ExecuteSqlQuery(
			bool testRawValue,
			ISession session,
			PropertyMetadata leftProperty,
			OperatorTest test,
			PropertyMetadata rightProperty)
		{
			var selectSql = $"{(test.LeftAsParameter ? "?" : GetColumnName(test.LeftPropertyName))} " +
			                $"{test.Operator} " +
			                $"{(test.RightAsParameter ? "?" : GetColumnName(test.RightPropertyName))}";
			var query = session.CreateSQLQuery($"select {selectSql} as col1 from NumericEntity")
			                   .AddScalar(
				                   "col1",
				                   testRawValue ? ObjectTypeInstance : NHibernateUtil.GuessType(test.OperandType));
			if (test.LeftAsParameter)
			{
				query.SetParameter(
					0,
					leftProperty.GetValue(_originalEntity),
					Sfi.GetClassMetadata(nameof(NumericEntity)).GetPropertyType(test.LeftPropertyName));
			}
			else if (test.RightAsParameter)
			{
				query.SetParameter(
					0,
					rightProperty.GetValue(_originalEntity),
					Sfi.GetClassMetadata(nameof(NumericEntity)).GetPropertyType(test.RightPropertyName));
			}

			return query.UniqueResult();
		}

		private object ExecuteLinqQuery(
			ISession session,
			PropertyMetadata leftProperty,
			OperatorTest test,
			PropertyMetadata rightProperty)
		{
			var query = session.Query<NumericEntity>();
			var lambda = GetLambdaExpression(leftProperty, test, rightProperty);
			var queryable = SelectDefinition.MakeGenericMethod(typeof(NumericEntity), test.OperandType)
			                                .Invoke(null, new object[] {query, lambda});
			try
			{
				return FirstDefinition.MakeGenericMethod(test.OperandType)
				                      .Invoke(null, new[] { queryable });
			}
			catch (TargetInvocationException e)
			{
				ExceptionDispatchInfo.Capture(e.InnerException).Throw();
			}

			return null;
		}

		private LambdaExpression GetLambdaExpression(
			PropertyMetadata leftProperty,
			OperatorTest test,
			PropertyMetadata rightProperty)
		{
			var parameter = Expression.Parameter(typeof(NumericEntity), "o");
			var leftExpression = test.LeftAsParameter
				? (Expression) Expression.Constant(leftProperty.GetValue(_originalEntity))
				: Expression.MakeMemberAccess(parameter, leftProperty.PropertyInfo);
			if (leftProperty.Type != test.OperandType)
			{
				leftExpression = Expression.Convert(leftExpression, test.OperandType);
			}

			var rightExpression = test.RightAsParameter
				? (Expression) Expression.Constant(rightProperty.GetValue(_originalEntity))
				: Expression.MakeMemberAccess(parameter, rightProperty.PropertyInfo);
			if (rightProperty.Type != test.OperandType)
			{
				rightExpression = Expression.Convert(rightExpression, test.OperandType);
			}

			var operatorExpression = GetOperatorExpression(test, leftExpression, rightExpression);
			
			return (LambdaExpression) LambdaDefinition
	                          .MakeGenericMethod(typeof(Func<,>).MakeGenericType(typeof(NumericEntity), test.OperandType))
	                          .Invoke(null, new object[] {operatorExpression, new[] {parameter}});
		}

		private static Expression GetOperatorExpression(
			OperatorTest test,
			Expression leftExpression,
			Expression rightExpression)
		{
			switch (test.Operator)
			{
				case '+':
					return Expression.AddChecked(leftExpression, rightExpression);
				case '-':
					return Expression.SubtractChecked(leftExpression, rightExpression);
				case '*':
					return Expression.MultiplyChecked(leftExpression, rightExpression);
				case '/':
					return Expression.Divide(leftExpression, rightExpression);
				case '%':
					return Expression.Modulo(leftExpression, rightExpression);
				default:
					throw new InvalidOperationException("Invalid operator");
			}
		}

		public class NumericEntity
		{
			public virtual int Id { get; set; }
			public virtual short Short { get; set; }
			public virtual int Integer { get; set; }
			public virtual long Long { get; set; }
			public virtual ushort UnsignedShort { get; set; }
			public virtual uint UnsignedInteger { get; set; }
			public virtual ulong UnsignedLong { get; set; }
			public virtual decimal Decimal { get; set; }
			public virtual decimal DecimalLowScale { get; set; }
			public virtual decimal Currency { get; set; }
			public virtual double Double { get; set; }
			public virtual float Float { get; set; }
		}

		private class OperatorTest
		{
			public OperatorTest(
				char @operator,
				string leftPropertyName,
				string rightPropertyName,
				System.Type operandType,
				bool leftAsParameter,
				bool rightAsParameter)
			{
				Operator = @operator;
				LeftPropertyName = leftPropertyName;
				RightPropertyName = rightPropertyName;
				OperandType = operandType;
				LeftAsParameter = leftAsParameter;
				RightAsParameter = rightAsParameter;
			}

			public char Operator { get; }
			public string LeftPropertyName { get; }
			public string RightPropertyName { get; }
			public System.Type OperandType { get; }
			public bool LeftAsParameter { get; }
			public bool RightAsParameter { get; }

			public override bool Equals(object obj)
			{
				if (!(obj is OperatorTest test))
				{
					return false;
				}

				return
					test.Operator == Operator &&
					test.LeftPropertyName == LeftPropertyName &&
					test.RightPropertyName == RightPropertyName &&
					test.OperandType == OperandType &&
					test.LeftAsParameter == LeftAsParameter &&
					test.RightAsParameter == RightAsParameter;
			}

			public override int GetHashCode()
			{
				return
					Operator.GetHashCode() ^
					LeftPropertyName.GetHashCode() ^
					RightPropertyName.GetHashCode() ^
					OperandType.GetHashCode() ^
					LeftAsParameter.GetHashCode() ^
					RightAsParameter.GetHashCode();
			}
		}

		private class PropertyMetadata
		{
			private readonly MethodInfo _getterMethodInfo;

			public PropertyMetadata(PropertyInfo propertyInfo)
			{
				PropertyInfo = propertyInfo;
				Name = propertyInfo.Name;
				ColumnName = GetColumnName(Name);
				Type = propertyInfo.PropertyType;
				_getterMethodInfo = propertyInfo.GetMethod;
			}

			public string Name { get; }

			public string ColumnName { get; }

			public System.Type Type { get; }

			public PropertyInfo PropertyInfo { get; }

			public object GetValue(object instance)
			{
				return _getterMethodInfo.Invoke(instance, null);
			}
		}

		private class ObjectType : PrimitiveType
		{
			public ObjectType() : base(new SqlType(DbType.Object))
			{
			}

			public override string Name => "Object";
			public override System.Type ReturnedClass => typeof(object);
			public override void Set(DbCommand cmd, object value, int index, ISessionImplementor session)
			{
				cmd.Parameters[index].Value = value;
			}

			public override object Get(DbDataReader rs, int index, ISessionImplementor session)
			{
				return rs[index];
			}

			public override object Get(DbDataReader rs, string name, ISessionImplementor session)
			{
				return rs[name];
			}

			public override System.Type PrimitiveClass => typeof(object);

			public override object DefaultValue => null;

			public override string ObjectToSQLString(object value, Dialect.Dialect dialect)
			{
				return value?.ToString();
			}
		}
	}
}
