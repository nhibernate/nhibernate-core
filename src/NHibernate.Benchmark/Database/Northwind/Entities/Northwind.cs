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

        public IQueryable<Customer> Customers => _session.Query<Customer>();

        public IQueryable<Product> Products => _session.Query<Product>();

        public IQueryable<Shipper> Shippers => _session.Query<Shipper>();

        public IQueryable<Order> Orders => _session.Query<Order>();

        public IQueryable<OrderLine> OrderLines => _session.Query<OrderLine>();

        public IQueryable<Employee> Employees => _session.Query<Employee>();

        public IQueryable<ProductCategory> Categories => _session.Query<ProductCategory>();

        public IQueryable<Timesheet> Timesheets => _session.Query<Timesheet>();

        public IQueryable<Animal> Animals => _session.Query<Animal>();

        public IQueryable<Mammal> Mammals => _session.Query<Mammal>();

        public IQueryable<User> Users => _session.Query<User>();

        public IQueryable<PatientRecord> PatientRecords => _session.Query<PatientRecord>();

        public IQueryable<State> States => _session.Query<State>();

        public IQueryable<Patient> Patients => _session.Query<Patient>();

        public IQueryable<Physician> Physicians => _session.Query<Physician>();

        public IQueryable<Role> Role => _session.Query<Role>();

        public IEnumerable<IUser> IUsers => _session.Query<IUser>();
    }
}