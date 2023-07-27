using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace NHibernate.DomainModel.Northwind.Entities
{
	public class NorthwindQueryOver
	{
		private readonly ISession _session;

		public NorthwindQueryOver(ISession session)
		{
			_session = session;
		}

		public IQueryOver<Customer, Customer> Customers
		{
			get { return _session.QueryOver<Customer>(); }
		}

		public IQueryOver<Product, Product> Products
		{
			get { return _session.QueryOver<Product>(); }
		}

		public IQueryOver<Shipper, Shipper> Shippers
		{
			get { return _session.QueryOver<Shipper>(); }
		}

		public IQueryOver<Order, Order> Orders
		{
			get { return _session.QueryOver<Order>(); }
		}

		public IQueryOver<OrderLine, OrderLine> OrderLines
		{
			get { return _session.QueryOver<OrderLine>(); }
		}

		public IQueryOver<Employee, Employee> Employees
		{
			get { return _session.QueryOver<Employee>(); }
		}

		public IQueryOver<ProductCategory, ProductCategory> Categories
		{
			get { return _session.QueryOver<ProductCategory>(); }
		}

		public IQueryOver<Timesheet, Timesheet> Timesheets
		{
			get { return _session.QueryOver<Timesheet>(); }
		}

		public IQueryOver<Animal, Animal> Animals
		{
			get { return _session.QueryOver<Animal>(); }
		}

		public IQueryOver<Mammal, Mammal> Mammals
		{
			get { return _session.QueryOver<Mammal>(); }
		}

		public IQueryOver<User, User> Users
		{
			get { return _session.QueryOver<User>(); }
		}

		public IQueryOver<PatientRecord, PatientRecord> PatientRecords
		{
			get { return _session.QueryOver<PatientRecord>(); }
		}

		public IQueryOver<State, State> States
		{
			get { return _session.QueryOver<State>(); }
		}

		public IQueryOver<Patient, Patient> Patients
		{
			get { return _session.QueryOver<Patient>(); }
		}

		public IQueryOver<Physician, Physician> Physicians
		{
			get { return _session.QueryOver<Physician>(); }
		}

		public IQueryOver<Role, Role> Role
		{
			get { return _session.QueryOver<Role>(); }
		}

		public IQueryOver<IUser, IUser> IUsers
		{
			get { return _session.QueryOver<IUser>(); }
		}
	}
}
