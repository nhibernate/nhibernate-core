using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Territory : Entity<Territory>
    {
        private readonly ISet<Employee> _employees;
        private string _description;
        private Region _region;

        public Territory() : this(null)
        {
        }

        public Territory(string description)
        {
            _description = description;

            _employees = new HashSet<Employee>();
        }

        public virtual string Description
        {
            get { return _description.Trim(); }
            set { _description = value; }
        }

        public virtual Region Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public virtual ReadOnlyCollection<Employee> Employees
        {
            get { return new ReadOnlyCollection<Employee>(new List<Employee>(_employees).AsReadOnly()); }
        }

        public virtual void AddEmployee(Employee employee)
        {
            if (!_employees.Contains(employee))
            {
                _employees.Add(employee);
            }
        }

        public virtual void RemoveEmployee(Employee employee)
        {
            if (_employees.Contains(employee))
            {
                _employees.Remove(employee);
            }
        }
    }
}