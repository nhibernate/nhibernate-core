using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.NH1978
{
    /// <summary> 
    /// Product object for NHibernate mapped table 'Products'.
    /// </summary>
    public class _401k
    {
        public virtual int ID { get; set; }
        public virtual string PlanName { get; set; }
        public virtual IList<Employee> Employees { get; set; }
    }
}