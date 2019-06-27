using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Employee
    {
        private readonly ISet<Order> _orders;
        private readonly ISet<Employee> _subordinates;
        private readonly IList<Territory> _territories;

        public Employee()
        {
            _subordinates = new HashSet<Employee>();
            _orders = new HashSet<Order>();
            _territories = new List<Territory>();
        }

        public virtual int EmployeeId { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

        public virtual string Title { get; set; }

        public virtual string TitleOfCourtesy { get; set; }

        public virtual DateTime? BirthDate { get; set; }

        public virtual DateTime? HireDate { get; set; }

        public virtual Address Address { get; set; }

        public virtual string Extension { get; set; }

        public virtual string Notes { get; set; }

        public virtual Employee Superior { get; set; }

        public virtual ISet<Employee> Subordinates => _subordinates;

        public virtual ReadOnlyCollection<Territory> Territories => new ReadOnlyCollection<Territory>(_territories);

        public virtual ISet<Order> Orders => _orders;

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