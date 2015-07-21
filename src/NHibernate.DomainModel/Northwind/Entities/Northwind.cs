using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Linq;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Northwind
    {
        private readonly ISession _session;

        public Northwind(ISession session)
        {
            _session = session;
        }

        public IQueryable<Customer> Customers
        {
            get { return _session.Query<Customer>(); }
        }

        public IQueryable<Product> Products
        {
            get { return _session.Query<Product>(); }
        }

        public IQueryable<Shipper> Shippers
        {
            get { return _session.Query<Shipper>(); }
        }

        public IQueryable<Order> Orders
        {
            get { return _session.Query<Order>(); }
        }
		
        public IQueryable<OrderLine> OrderLines
        {
            get { return _session.Query<OrderLine>(); }
        }

        public IQueryable<Employee> Employees
        {
            get { return _session.Query<Employee>(); }
        }

        public IQueryable<ProductCategory> Categories
        {
            get { return _session.Query<ProductCategory>(); }
        }

        public IQueryable<Timesheet> Timesheets
        {
            get { return _session.Query<Timesheet>(); }
        }

    	public IQueryable<Animal> Animals
    	{
			get { return _session.Query<Animal>(); }
    	}

        public IQueryable<Mammal> Mammals
        {
            get { return _session.Query<Mammal>(); }
        }

        public IQueryable<User> Users
        {
            get { return _session.Query<User>(); }
        }

        public IQueryable<PatientRecord> PatientRecords
        {
            get { return _session.Query<PatientRecord>(); }
        }

        public IQueryable<State> States
        {
            get { return _session.Query<State>(); }
        }

        public IQueryable<Patient> Patients
        {
            get { return _session.Query<Patient>(); }
        }

        public IQueryable<Physician> Physicians
        {
            get { return _session.Query<Physician>(); }
        }

        public IQueryable<Role> Role
        {
            get { return _session.Query<Role>(); }
        }

        public IEnumerable<IUser> IUsers
        {
            get { return _session.Query<IUser>(); }
        }
    }
}