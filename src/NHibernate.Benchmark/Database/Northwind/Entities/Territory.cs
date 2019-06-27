using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Territory : Entity<Territory>
    {
        private readonly ISet<Employee> _employees;
        private string _description;

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
            get => _description.Trim();
            set => _description = value;
        }

        public virtual Region Region { get; set; }

        public virtual ReadOnlyCollection<Employee> Employees => new ReadOnlyCollection<Employee>(new List<Employee>(_employees).AsReadOnly());

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