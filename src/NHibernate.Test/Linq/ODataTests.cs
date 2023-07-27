﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.AspNetCore.Http;
using Microsoft.OData.Edm;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
	[TestFixture]
	public class ODataTests : LinqTestCase
	{
		private IEdmModel _edmModel;

		protected override void OnSetUp()
		{
			base.OnSetUp();

			_edmModel = CreatEdmModel();
		}

		[TestCase("$expand=Customer", 830, "Customer")]
		[TestCase("$expand=OrderLines", 830, "OrderLines")]
		public void Expand(string queryString, int expectedRows, string expandedProperty)
		{
			var query = ApplyFilter(session.Query<Order>(), queryString);
			Assert.That(query, Is.AssignableTo<IQueryable<ISelectExpandWrapper>>());

			var results = ((IQueryable<ISelectExpandWrapper>) query).ToList();
			Assert.That(results, Has.Count.EqualTo(expectedRows));

			var dict = results[0].ToDictionary();
			Assert.That(dict.TryGetValue(expandedProperty, out var value), Is.True);
			Assert.That(value, Is.Not.Null);
		}

		[TestCase("$apply=groupby((Customer/CustomerId))", 89)]
		[TestCase("$apply=groupby((Customer/CustomerId))&$orderby=Customer/CustomerId", 89)]
		[TestCase("$apply=groupby((Customer/CustomerId, ShippingAddress/PostalCode), aggregate(OrderId with average as Average, Employee/EmployeeId with max as Max))", 89)]
		[TestCase("$apply=groupby((Customer/CustomerId), aggregate(OrderId with sum as Total))&$skip=2", 87)]
		public void OrderGroupBy(string queryString, int expectedRows)
		{
			var query = ApplyFilter(session.Query<Order>(), queryString);
			Assert.That(query, Is.AssignableTo<IQueryable<DynamicTypeWrapper>>());

			var results = ((IQueryable<DynamicTypeWrapper>) query).ToList();
			Assert.That(results, Has.Count.EqualTo(expectedRows));
		}

		private class CustomerVm : BaseCustomerVm
		{
		}

		private class BaseCustomerVm
		{
			public string Id { get; set; }

			public string Name { get; set; }
		}

		[TestCase("$filter=Name eq 'Maria Anders'", 1)]
		public void BasePropertyFilter(string queryString, int expectedRows)
		{
			var query = ApplyFilter(
				session.Query<Customer>().Select(o => new CustomerVm {Name = o.ContactName, Id = o.CustomerId}),
				queryString);

			var results = ((IQueryable<CustomerVm>) query).ToList();
			Assert.That(results, Has.Count.EqualTo(expectedRows));
		}

		//GH-2362
		[TestCase("$filter=CustomerId le 'ANATR'", 2)]
		[TestCase("$filter=startswith(CustomerId, 'ANATR')", 1)]
		[TestCase("$filter=endswith(CustomerId, 'ANATR')", 1)]
		[TestCase("$filter=indexof(CustomerId, 'ANATR') eq 0", 1)]
		public void StringFilter(string queryString, int expectedCount)
		{
			Assert.That(
				ApplyFilter(session.Query<Customer>(), queryString).Cast<Customer>().ToList(),
				Has.Count.EqualTo(expectedCount));
		}

		private IQueryable ApplyFilter<T>(IQueryable<T> query, string queryString)
		{
			var context = new ODataQueryContext(CreatEdmModel(), typeof(T), null) { };
			var dataQuerySettings = new ODataQuerySettings {HandleNullPropagation = HandleNullPropagationOption.False};
			var serviceProvider = new ODataServiceProvider(
				new Dictionary<System.Type, object>()
				{
					{typeof(DefaultQuerySettings), new DefaultQuerySettings()},
					{typeof(ODataOptions), new ODataOptions()},
					{typeof(IEdmModel), _edmModel},
					{typeof(ODataQuerySettings), dataQuerySettings},
				});

			HttpContext httpContext = new DefaultHttpContext();
			httpContext.ODataFeature().RequestContainer = serviceProvider;
			httpContext.RequestServices = serviceProvider;
			var request = httpContext.Request;
			Uri requestUri = new Uri($"http://localhost/?{queryString}");
			request.Method = HttpMethods.Get;
			request.Scheme = requestUri.Scheme;
			request.Host = new HostString(requestUri.Host);
			request.QueryString = new QueryString(requestUri.Query);
			request.Path = new PathString(requestUri.AbsolutePath);
			var options = new ODataQueryOptions(context, request);

			return options.ApplyTo(query, dataQuerySettings);
		}

		private static IEdmModel CreatEdmModel()
		{
			var builder = new ODataConventionModelBuilder();

			var addressModel = builder.ComplexType<Address>();
			addressModel.Property(o => o.City);
			addressModel.Property(o => o.Country);
			addressModel.Property(o => o.Fax);
			addressModel.Property(o => o.PhoneNumber);
			addressModel.Property(o => o.PostalCode);
			addressModel.Property(o => o.Region);
			addressModel.Property(o => o.Street);

			var customerModel = builder.EntitySet<Customer>(nameof(Customer));
			customerModel.EntityType.HasKey(o => o.CustomerId);
			customerModel.EntityType.Property(o => o.CompanyName);
			customerModel.EntityType.Property(o => o.ContactTitle);
			customerModel.EntityType.ComplexProperty(o => o.Address);
			customerModel.EntityType.HasMany(o => o.Orders);

			var orderLineModel = builder.EntitySet<OrderLine>(nameof(OrderLine));
			orderLineModel.EntityType.HasKey(o => o.Id);
			orderLineModel.EntityType.Property(o => o.Discount);
			orderLineModel.EntityType.Property(o => o.Quantity);
			orderLineModel.EntityType.Property(o => o.UnitPrice);
			orderLineModel.EntityType.HasRequired(o => o.Order);

			var orderModel = builder.EntitySet<Order>(nameof(Order));
			orderModel.EntityType.HasKey(o => o.OrderId);
			orderModel.EntityType.Property(o => o.Freight);
			orderModel.EntityType.Property(o => o.OrderDate);
			orderModel.EntityType.Property(o => o.RequiredDate);
			orderModel.EntityType.Property(o => o.ShippedTo);
			orderModel.EntityType.Property(o => o.ShippingDate);
			orderModel.EntityType.ComplexProperty(o => o.ShippingAddress);
			orderModel.EntityType.HasRequired(o => o.Customer);
			orderModel.EntityType.HasOptional(o => o.Employee);
			orderModel.EntityType.HasMany(o => o.OrderLines);

			var employeeModel = builder.EntitySet<Employee>(nameof(Employee));
			employeeModel.EntityType.HasKey(o => o.EmployeeId);
			employeeModel.EntityType.Property(o => o.BirthDate);
			employeeModel.EntityType.Property(o => o.Extension);
			employeeModel.EntityType.Property(o => o.FirstName);
			employeeModel.EntityType.Property(o => o.HireDate);
			employeeModel.EntityType.Property(o => o.LastName);
			employeeModel.EntityType.Property(o => o.Notes);
			employeeModel.EntityType.Property(o => o.Title);
			employeeModel.EntityType.HasMany(o => o.Orders);

			builder.EntitySet<CustomerVm>(nameof(CustomerVm));

			return builder.GetEdmModel();
		}

		private class ODataServiceProvider : IServiceProvider
		{
			private readonly Dictionary<System.Type, object> _singletonObjects = new Dictionary<System.Type, object>();

			public ODataServiceProvider(Dictionary<System.Type, object> singletonObjects)
			{
				_singletonObjects = singletonObjects;
			}

			public object GetService(System.Type serviceType)
			{
				if (_singletonObjects.TryGetValue(serviceType, out var service))
				{
					return service;
				}

				var ctor = serviceType.GetConstructor(new System.Type[0]);
				if (ctor != null)
				{
					return ctor.Invoke(new object[0]);
				}

				ctor = serviceType.GetConstructor(new[] { typeof(DefaultQuerySettings) });
				if (ctor != null)
				{
					return ctor.Invoke(new object[] { GetService(typeof(DefaultQuerySettings)) });
				}

				ctor = serviceType.GetConstructor(new[] { typeof(IServiceProvider) });
				if (ctor != null)
				{
					return ctor.Invoke(new object[] { this });
				}

				return null;
			}
		}
	}
}
