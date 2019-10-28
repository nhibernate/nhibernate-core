using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.DomainModel;
using NHibernate.DomainModel.NHSpecific;
using NHibernate.DomainModel.Northwind.Entities;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Linq;
using NHibernate.Linq.Visitors;
using NHibernate.Persister.Entity;
using NHibernate.Type;
using NHibernate.Util;
using NUnit.Framework;
using IQueryable = System.Linq.IQueryable;

namespace NHibernate.Test.Linq
{
	/// <summary>
	/// Tests form ExpressionsHelper.TryGetMappedType and ExpressionsHelper.TryGetMappedNullability
	/// </summary>
	public class TryGetMappedTests : LinqTestCase
	{
		private static readonly TryGetMappedType _tryGetMappedType;
		private static readonly TryGetMappedNullability _tryGetMappedNullability;

		delegate bool TryGetMappedType(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out IType mappedType,
			out IEntityPersister entityPersister,
			out IAbstractComponentType component,
			out string memberPath);

		delegate bool TryGetMappedNullability(
			ISessionFactoryImplementor sessionFactory,
			Expression expression,
			out bool nullability);

		static TryGetMappedTests()
		{
			var method = typeof(ExpressionsHelper).GetMethod(
				nameof(TryGetMappedType),
				BindingFlags.NonPublic | BindingFlags.Static);
			var sessionFactoryParam = Expression.Parameter(typeof(ISessionFactoryImplementor), "sessionFactory");
			var expressionParam = Expression.Parameter(typeof(Expression), "expression");
			var mappedTypeParam = Expression.Parameter(typeof(IType).MakeByRefType(), "mappedType");
			var entityPersisterParam = Expression.Parameter(typeof(IEntityPersister).MakeByRefType(), "entityPersister");
			var componentParam = Expression.Parameter(typeof(IAbstractComponentType).MakeByRefType(), "component");
			var memberPathParam = Expression.Parameter(typeof(string).MakeByRefType(), "memberPath");
			var methodCall = Expression.Call(
				method,
				sessionFactoryParam,
				expressionParam,
				mappedTypeParam,
				entityPersisterParam,
				componentParam,
				memberPathParam);
			_tryGetMappedType = Expression.Lambda<TryGetMappedType>(
				methodCall,
				sessionFactoryParam,
				expressionParam,
				mappedTypeParam,
				entityPersisterParam,
				componentParam,
				memberPathParam).Compile();

			method = typeof(ExpressionsHelper).GetMethod(
				nameof(TryGetMappedNullability),
				BindingFlags.NonPublic | BindingFlags.Static);
			var nullabilityParam = Expression.Parameter(typeof(bool).MakeByRefType(), "nullability");
			methodCall = Expression.Call(
				method,
				sessionFactoryParam,
				expressionParam,
				nullabilityParam);
			_tryGetMappedNullability = Expression.Lambda<TryGetMappedNullability>(
				methodCall,
				sessionFactoryParam,
				expressionParam,
				nullabilityParam).Compile();
		}

		protected override string[] Mappings
		{
			get
			{
				return 
					new[]
					{
						"ABC.hbm.xml",
						"Baz.hbm.xml",
						"FooBar.hbm.xml",
						"Glarch.hbm.xml",
						"Fee.hbm.xml",
						"Qux.hbm.xml",
						"Fum.hbm.xml",
						"Holder.hbm.xml",
						"One.hbm.xml",
						"Many.hbm.xml"
					}.Concat(base.Mappings).ToArray();
			}
		}

		[Test]
		public void SelfTest()
		{
			var query = db.OrderLines.Select(o => o);
			AssertTrueNotNull(
				query,
				typeof(OrderLine).FullName,
				null,
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(OrderLine));
		}

		[Test]
		public void SelfCastNotMappedTest()
		{
			var query = session.Query<A>().Select(o => (object) o);
			AssertTrueNotNull(
				query,
				false,
				typeof(A).FullName,
				null,
				o => o is SerializableType serializableType && serializableType.ReturnedClass == typeof(object));
		}

		[Test]
		public void PropertyTest()
		{
			var query = db.OrderLines.Select(o => o.Quantity);
			AssertTrueNotNull(query, typeof(OrderLine).FullName, "Quantity", o => o is Int32Type);
		}

		[Test]
		public void NotMappedPropertyTest()
		{
			var query = db.Users.Select(o => o.NotMapped);
			AssertFalse(query, typeof(User).FullName, "NotMapped", o => o is null);
		}

		[Test]
		public void NestedNotMappedPropertyTest()
		{
			var query = db.Users.Select(o => o.Name.Length);
			AssertFalse(query, false, null, null, o => o is null);
		}

		[Test]
		public void PropertyCastTest()
		{
			var query = db.OrderLines.Select(o => (long) o.Quantity);
			AssertTrueNotNull(query, typeof(OrderLine).FullName, "Quantity", o => o is Int64Type);
		}

		[Test]
		public void PropertyIndexer()
		{
			var query = db.Products.Select(o => o.Name[0]);
			AssertFalse(query, null, null, o => o == null);
		}

		[Test]
		public void EnumInt32Test()
		{
			var query = db.Users.Select(o => o.Enum2);
			AssertTrueNotNull(
				query,
				typeof(User).FullName,
				"Enum2",
				o => o.GetType().GetGenericArguments().FirstOrDefault() == typeof(EnumStoredAsInt32));
		}

		[Test]
		public void EnumInt32CastTest()
		{
			var query = db.Users.Select(o => (int) o.Enum2);
			AssertTrueNotNull(query, typeof(User).FullName, "Enum2", o => o is Int32Type);
		}

		[Test]
		public void EnumAsStringTest()
		{
			var query = db.Users.Select(o => o.Enum1);
			AssertTrue(query, typeof(User).FullName, "Enum1", o => o is EnumStoredAsStringType);
		}

		[Test]
		public void IdentifierTest()
		{
			var query = db.OrderLines.Select(o => o.Id);
			AssertTrueNotNull(query, typeof(OrderLine).FullName, "Id", o => o is Int64Type);
		}

		[Test]
		public void CompositeIdentifierTest()
		{
			var query = session.Query<Fum>().Select(o => o.Id.Date);
			AssertTrueNotNull(
				query,
				typeof(Fum).FullName,
				"Id.Date",
				o => o is DateTimeType,
				o => o?.Name == "component[String,Short,Date]");
		}

		[Test]
		public void ComponentTest()
		{
			var query = db.Customers.Select(o => o.Address);
			AssertTrue(
				query,
				typeof(Customer).FullName,
				"Address",
				o => o is ComponentType && o.Name == "component[Street,City,Region,PostalCode,Country,PhoneNumber,Fax]");
		}

		[Test]
		public void ComponentPropertyTest()
		{
			var query = db.Customers.Select(o => o.Address.City);
			AssertTrue(
				query,
				typeof(Customer).FullName,
				"Address.City",
				o => o is StringType,
				o => o?.Name == "component[Street,City,Region,PostalCode,Country,PhoneNumber,Fax]");
		}

		[Test]
		public void ComponentNotMappedPropertyTest()
		{
			var query = db.Customers.Select(o => o.Address.NotMapped);
			AssertFalse(
				query,
				typeof(Customer).FullName,
				"Address.NotMapped",
				o => o == null,
				o => o?.Name == "component[Street,City,Region,PostalCode,Country,PhoneNumber,Fax]");
		}

		[Test]
		public void ComponentNestedNotMappedPropertyTest()
		{
			var query = db.Customers.Select(o => o.Address.City.Length);
			AssertFalse(query, false, null, null, o => o == null);
		}

		[Test]
		public void NestedComponentPropertyTest()
		{
			var query = db.Users.Select(o => o.Component.OtherComponent.OtherProperty1);
			AssertTrue(
				query,
				typeof(User).FullName,
				"Component.OtherComponent.OtherProperty1",
				o => o is AnsiStringType,
				o => o?.Name == "component[OtherProperty1]");
		}

		[Test]
		public void NestedComponentPropertyCastTest()
		{
			var query = db.Users.Select(o => (object) o.Component.OtherComponent.OtherProperty1);
			AssertTrue(
				query,
				typeof(User).FullName,
				"Component.OtherComponent.OtherProperty1",
				o => o is SerializableType serializableType && serializableType.ReturnedClass == typeof(object),
				o => o?.Name == "component[OtherProperty1]");
		}

		[Test]
		public void ManyToOneTest()
		{
			var query = db.OrderLines.Select(o => o.Order);
			AssertTrueNotNull(query, typeof(OrderLine).FullName, "Order",
						o => o is ManyToOneType manyToOne && manyToOne.PropertyName == "Order");
		}

		[Test]
		public void ManyToOnePropertyTest()
		{
			var query = db.OrderLines.Select(o => o.Order.Freight);
			AssertTrue(query, typeof(Order).FullName, "Freight", o => o is DecimalType);
		}

		[Test]
		public void ManyToOneNotMappedPropertyTest()
		{
			var query = db.OrderLines.Select(o => o.Product.NotMapped);
			AssertFalse(query, typeof(Product).FullName, "NotMapped", o => o == null);
		}

		[Test]
		public void NotMappedManyToOnePropertyTest()
		{
			var query = db.Users.Select(o => o.NotMappedRole.Name);
			AssertFalse(query, false, null, null, o => o is null);
		}

		[Test]
		public void NestedManyToOneTest()
		{
			var query = db.OrderLines.Select(o => o.Order.Employee);
			AssertTrue(query, false, typeof(Order).FullName, "Employee",
						o => o is ManyToOneType manyToOne && manyToOne.PropertyName == "Employee");
		}

		[Test]
		public void NestedManyToOnePropertyTest()
		{
			var query = db.OrderLines.Select(o => o.Order.Employee.BirthDate);
			AssertTrue(query, typeof(Employee).FullName, "BirthDate", o => o is DateTimeType);
		}

		[Test]
		public void OneToManyTest()
		{
			var query = db.Customers.SelectMany(o => o.Orders);
			AssertTrue(
				query,
				typeof(Customer).FullName,
				"Orders",
				o => o is CollectionType collectionType && collectionType.Role == $"{typeof(Customer).FullName}.Orders");
		}

		[Test]
		public void OneToManyElementIndexerTest()
		{
			var query = session.Query<Baz>().Select(o => o.StringList[0]);
			AssertTrue(query, false, typeof(Baz).FullName, "StringList", o => o is StringType);
		}

		[Test]
		public void OneToManyElementIndexerNotMappedPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => o.StringList[0].Length);
			AssertFalse(query, false, null, null, o => o == null);
		}

		[Test]
		public void OneToManyCustomElementIndexerTest()
		{
			var query = session.Query<Baz>().Select(o => o.Customs[0]);
			AssertTrue(
				query,
				false,
				typeof(Baz).FullName,
				"Customs",
				o => o is CompositeCustomType customType && customType.UserType is DoubleStringType);
		}

		[Test]
		public void OneToManyIndexerCastTest()
		{
			var query = session.Query<Baz>().Select(o => (long) o.IntArray[0]);
			AssertTrue(query, false, typeof(Baz).FullName, "IntArray", o => o is Int64Type);
		}

		[Test]
		public void OneToManyIndexerPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => o.Fees[0].Count);
			AssertTrue(query, false, typeof(Fee).FullName, "Count", o => o is Int32Type);
		}

		[Test]
		public void OneToManyElementAtTest()
		{
			var query = session.Query<Baz>().Select(o => o.StringList.ElementAt(0));
			AssertTrue(query, false, typeof(Baz).FullName, "StringList", o => o is StringType);
		}

		[Test]
		public void NestedOneToManyManyToOneComponentPropertyTest()
		{
			var query = session.Query<Baz>().SelectMany(o => o.Fees).Select(o => o.TheFee.Compon.Name);
			AssertTrue(
				query,
				typeof(Fee).FullName,
				"Compon.Name",
				o => o is StringType,
				o => o?.Name == "component[Name,NullString]");
		}

		[Test]
		public void OneToManyCompositeElementPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].Count);
			AssertTrue(
				query,
				false,
				null,
				"Count",
				o => o is Int32Type,
				o => o?.Name == "component[Name,Count,Subcomponent]");
		}

		[Test]
		public void OneToManyCompositeElementPropertyIndexerTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].Name[0]);
			AssertFalse(query, false, null, null, o => o == null);
		}

		[Test]
		public void OneToManyCompositeElementNotMappedPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].NotMapped);
			AssertFalse(
				query,
				false,
				null,
				"NotMapped",
				o => o == null,
				o => o?.Name == "component[Name,Count,Subcomponent]");
		}

		[Test]
		public void OneToManyCompositeElementCastPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => (long) o.Components[0].Count);
			AssertTrue(
				query,
				false,
				null,
				"Count",
				o => o is Int64Type,
				o => o?.Name == "component[Name,Count,Subcomponent]");
		}

		[Test]
		public void OneToManyCompositeElementCollectionNotMappedPropertyTest()
		{
			var query = session.Query<Baz>().SelectMany(o => o.Components[0].ImportantDates);
			AssertFalse(
				query,
				false,
				null,
				"ImportantDates",
				o => o == null,
				o => o?.Name == "component[Name,Count,Subcomponent]");
		}

		[Test]
		public void NestedOneToManyCompositeElementTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].Subcomponent);
			AssertTrue(
				query,
				false,
				null,
				"Subcomponent",
				o => o is IAbstractComponentType componentType && componentType.ReturnedClass == typeof(FooComponent),
				o => o?.Name == "component[Name,Count,Subcomponent]");
		}

		[Test]
		public void NestedOneToManyCompositeElementPropertyTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].Subcomponent.Name);
			AssertTrue(query, false, null, "Name", o => o is StringType, o => o?.Name == "component[Name,Count]");
		}

		[Test]
		public void NestedOneToManyCompositeElementPropertyIndexerTest()
		{
			var query = session.Query<Baz>().Select(o => o.Components[0].Subcomponent.Name[0]);
			AssertFalse(query, false, null, null, o => o == null);
		}

		[Test]
		public void ManyToManyTest()
		{
			var query = session.Query<Baz>().Select(o => o.FooArray);
			AssertTrue(
				query,
				false,
				typeof(Baz).FullName,
				"FooArray",
				o => o is ArrayType arrayType && arrayType.Role == $"{typeof(Baz).FullName}.FooArray");
		}

		[Test]
		public void ManyToManyIndexerTest()
		{
			var query = session.Query<Baz>().Select(o => o.FooArray[0].Null);
			AssertTrue(query, false, typeof(Foo).FullName, "Null", o => o is NullableInt32Type);
		}

		[Test]
		public void SubclassCastTest()
		{
			var query = session.Query<A>().Select(o => (B) o);
			AssertTrueNotNull(
				query,
				typeof(A).FullName,
				null,
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(B));
		}

		[Test]
		public void NestedSubclassCastTest()
		{
			var query = session.Query<A>().Select(o => (C1) ((B) o));
			AssertTrueNotNull(
				query,
				false,
				typeof(A).FullName,
				null,
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(C1));
		}

		[Test]
		public void SubclassPropertyTest()
		{
			var query = session.Query<A>().Select(o => ((C1) o).Count);
			AssertTrue(query, typeof(C1).FullName, "Count", o => o is Int32Type);
		}

		[Test]
		public void NestedSubclassCastPropertyTest()
		{
			var query = session.Query<A>().Select(o => ((C1) ((B) o)).Id);
			AssertTrueNotNull(query, typeof(C1).FullName, "Id", o => o is Int64Type);
		}

		[Test]
		public void AnyTest()
		{
			var query = session.Query<Bar>().Select(o => o.Object);
			AssertTrue(query, typeof(Bar).FullName, "Object", o => o.IsAnyType);
		}

		[Test]
		public void CastAnyTest()
		{
			var query = session.Query<Bar>().Select(o => (Foo) o.Object);
			AssertTrue(
				query,
				typeof(Bar).FullName,
				"Object",
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(Foo));
		}

		[Test]
		public void NestedCastAnyTest()
		{
			var query = session.Query<Bar>().Select(o => (Foo) ((Bar) o.Object).Object);
			AssertTrue(
				query,
				false,
				typeof(Bar).FullName,
				"Object",
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(Foo));
		}

		[Test]
		public void CastAnyManyToOneTest()
		{
			var query = session.Query<Bar>().Select(o => ((Foo) o.Object).Dependent);
			AssertTrueNotNull(
				query,
				typeof(Foo).FullName,
				"Dependent",
				o => o is EntityType entityType && entityType.ReturnedClass == typeof(Fee));
		}

		[Test]
		public void CastAnyPropertyTest()
		{
			var query = session.Query<Bar>().Select(o => ((Foo) o.Object).String);
			AssertTrue(query, false, typeof(Foo).FullName, "String", o => o is StringType);
		}

		[Test]
		public void QueryUnmppedEntityTest()
		{
			var query = session.Query<IEntity<int>>().Select(o => o.Id);
			AssertTrueNotNull(query, typeof(User).FullName, "Id", o => o is Int32Type);
		}

		[Test]
		public void ConditionalExpressionTest()
		{
			var query = db.Users.Select(o => (o.Name == "Test" ? o.RegisteredAt : o.LastLoginDate));
			AssertTrue(query, false, typeof(User).FullName, "RegisteredAt", o => o is DateTimeType);
		}

		[Test]
		public void ConditionalIfFalseExpressionTest()
		{
			var query = db.Users.Select(o => (o.Name == "Test" ? DateTime.Today : o.LastLoginDate));
			AssertTrue(query, false, typeof(User).FullName, "LastLoginDate", o => o is DateTimeType);
		}

		[Test]
		public void ConditionalMemberExpressionTest()
		{
			var query = db.Users.Select(o => (o.Name == "Test" ? o.NotMappedRole : o.Role).IsActive);
			AssertTrue(query, false, typeof(Role).FullName, "IsActive", o => o is BooleanType);
		}

		[Test]
		public void ConditionalNestedExpressionTest()
		{
			var query = db.Users.Select(o => (o.Name == "Test" ? o.Component.OtherComponent.OtherProperty1 : o.Component.Property1));
			AssertTrue(
				query,
				false,
				typeof(User).FullName,
				"Component.OtherComponent.OtherProperty1",
				o => o is AnsiStringType,
				o => o?.Name == "component[OtherProperty1]");
		}

		[Test]
		public void CoalesceExpressionTest()
		{
			var query = db.Users.Select(o => o.LastLoginDate ?? o.RegisteredAt);
			AssertTrue(query, false, typeof(User).FullName, "LastLoginDate", o => o is DateTimeType);
		}

		[Test]
		public void CoalesceRightExpressionTest()
		{
			var query = db.Users.Select(o => ((DateTime?) DateTime.Now) ?? o.RegisteredAt);
			AssertTrue(query, false, typeof(User).FullName, "RegisteredAt", o => o is DateTimeType);
		}

		[Test]
		public void CoalesceMemberExpressionTest()
		{
			var query = db.Users.Select(o => (o.NotMappedRole ?? o.Role).IsActive);
			AssertTrue(query, false, typeof(Role).FullName, "IsActive", o => o is BooleanType);
		}

		[Test]
		public void CoalesceNestedExpressionTest()
		{
			var query = db.Users.Select(o => o.Component.OtherComponent.OtherProperty1 ?? o.Component.Property1);
			AssertTrue(
				query,
				false,
				typeof(User).FullName,
				"Component.OtherComponent.OtherProperty1",
				o => o is AnsiStringType,
				o => o?.Name == "component[OtherProperty1]");
		}

		[Test]
		public void CoalesceConditionalMemberExpressionTest()
		{
			var query = db.Users.Select(o => (o.Name == "Test" ? o.NotMappedRole : (o.NotMappedRole ?? new Role() ?? o.Role)).IsActive);
			AssertTrue(query, false, typeof(Role).FullName, "IsActive", o => o is BooleanType);
		}

		[Test]
		public void JoinTest()
		{
			var query = from o in db.Orders
						from p in db.Products
						join d in db.OrderLines
							on new {o.OrderId, p.ProductId} equals new {d.Order.OrderId, d.Product.ProductId}
							into details
						from d in details
						select d.UnitPrice;
			AssertTrueNotNull(query, typeof(OrderLine).FullName, "UnitPrice", o => o is DecimalType);
		}

		[Test]
		public void NotNullComponentPropertyTest()
		{
			var query = session.Query<Patient>().SelectMany(o => o.PatientRecords.Select(r => r.Name.FirstName));
			AssertTrueNotNull(
				query,
				typeof(PatientRecord).FullName,
				"Name.FirstName",
				o => o is StringType,
				o => o?.Name == "component[FirstName,LastName]");
		}

		[Test]
		public void NotRelatedTypeTest()
		{
			var query = session.Query<Expression>().Select(o => o.CanReduce);
			AssertFalse(query, null, null, o => o == null);
		}

		[Test]
		public void NotNhQueryableTest()
		{
			var query = new List<User>().AsQueryable().Select(o => o.Name);
			AssertFalse(query, false, null, null, o => o == null);
		}

		private void AssertFalse(
			IQueryable query,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, true, false, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType);
		}

		private void AssertFalse(
			IQueryable query,
			bool rewriteQuery,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, rewriteQuery, false, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType);
		}

		private void AssertTrue(
			IQueryable query,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, true, true, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType);
		}

		private void AssertTrue(
			IQueryable query,
			bool rewriteQuery,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, rewriteQuery, true, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType);
		}

		private void AssertTrueNotNull(
			IQueryable query,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, true, true, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType, false);
		}

		private void AssertTrueNotNull(
			IQueryable query,
			bool rewriteQuery,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null)
		{
			AssertResult(query, rewriteQuery, true, expectedEntityName, expectedMemberPath, expectedMemberType, expectedComponentType, false);
		}

		private void AssertResult(
			IQueryable query,
			bool rewriteQuery,
			bool result,
			string expectedEntityName,
			string expectedMemberPath,
			Predicate<IType> expectedMemberType,
			Predicate<IAbstractComponentType> expectedComponentType = null,
			bool nullability = true)
		{
			expectedComponentType = expectedComponentType ?? (o => o == null);

			var expression = query.Expression;
			NhRelinqQueryParser.PreTransform(expression);
			var constantToParameterMap = ExpressionParameterVisitor.Visit(expression, Sfi);
			var queryModel = NhRelinqQueryParser.Parse(expression);
			var requiredHqlParameters = new List<NamedParameterDescriptor>();
			var visitorParameters = new VisitorParameters(
				Sfi,
				constantToParameterMap,
				requiredHqlParameters,
				new QuerySourceNamer(),
				expression.Type,
				QueryMode.Select);
			if (rewriteQuery)
			{
				QueryModelVisitor.GenerateHqlQuery(
					queryModel,
					visitorParameters,
					true,
					NhLinqExpressionReturnType.Scalar);
			}

			var found = _tryGetMappedType(
				Sfi,
				queryModel.SelectClause.Selector,
				out var memberType,
				out var entityPersister,
				out var componentType,
				out var memberPath);
			Assert.That(found, Is.EqualTo(result), "Expression should be supported");
			Assert.That(entityPersister?.EntityName, Is.EqualTo(expectedEntityName), "Invalid enity name");
			Assert.That(memberPath, Is.EqualTo(expectedMemberPath), "Invalid member path");
			Assert.That(() => expectedMemberType(memberType), $"Invalid member type: {memberType?.Name ?? "null"}");
			Assert.That(() => expectedComponentType(componentType), $"Invalid component type: {componentType?.Name ?? "null"}");

			if (found)
			{
				Assert.That(_tryGetMappedNullability(Sfi, queryModel.SelectClause.Selector, out var isNullable), Is.True, "Expression should be supported");
				Assert.That(nullability, Is.EqualTo(isNullable), "Nullability is not correct");
			}
		}
	}
}
