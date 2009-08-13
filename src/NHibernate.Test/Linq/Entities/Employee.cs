using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Iesi.Collections.Generic;

namespace NHibernate.Test.Linq.Entities
{
    public class Employee : Entity<Employee>
    {
        private readonly ISet<Order> _orders;
        private readonly ISet<Employee> _subordinates;
        private readonly IList<Territory> _territories;
        private Address _address;
        private DateTime? _birthDate;
        private string _extension;
        private string _firstName;
        private DateTime? _hireDate;
        private string _lastName;
        private string _notes;
        private Employee _superior;
        private string _title;
        private string _titleOfCourtesy;

        public Employee()
            : this(null, null)
        {
        }

        public Employee(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;

            _subordinates = new HashedSet<Employee>();
            _orders = new HashedSet<Order>();
            _territories = new List<Territory>();
        }

        public virtual string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        public virtual string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        public virtual string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public virtual string TitleOfCourtesy
        {
            get { return _titleOfCourtesy; }
            set { _titleOfCourtesy = value; }
        }

        public virtual DateTime? BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }

        public virtual DateTime? HireDate
        {
            get { return _hireDate; }
            set { _hireDate = value; }
        }

        public virtual Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public virtual string Extension
        {
            get { return _extension; }
            set { _extension = value; }
        }

        public virtual string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public virtual Employee Superior
        {
            get { return _superior; }
            set { _superior = value; }
        }

        public virtual ReadOnlyCollection<Employee> Subordinates
        {
            get { return new ReadOnlyCollection<Employee>(new List<Employee>(_subordinates)); }
        }

        public virtual ReadOnlyCollection<Territory> Territories
        {
            get { return new ReadOnlyCollection<Territory>(_territories); }
        }

        public virtual ReadOnlyCollection<Order> Orders
        {
            get { return new ReadOnlyCollection<Order>(new List<Order>(_orders)); }
        }

        public virtual void AddSubordinate(Employee subordinate)
        {
            if (!_subordinates.Contains(subordinate))
            {
                subordinate.Superior = this;
                _subordinates.Add(subordinate);
            }
        }

        public virtual void RemoveSubordinate(Employee subordinate)
        {
            if (_subordinates.Contains(subordinate))
            {
                subordinate.Superior = null;
                _subordinates.Remove(subordinate);
            }
        }

        public virtual void AddOrder(Order order)
        {
            if (!_orders.Contains(order))
            {
                order.Employee = this;
                _orders.Add(order);
            }
        }

        public virtual void RemoveOrder(Order order)
        {
            if (_orders.Contains(order))
            {
                order.Employee = null;
                _orders.Remove(order);
            }
        }
    }
}