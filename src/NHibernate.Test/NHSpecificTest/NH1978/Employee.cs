namespace NHibernate.Test.NHSpecificTest.NH1978
{
    /// <summary> 
    /// Category object for NHibernate mapped table 'Categories'.
    /// </summary>
    public class Employee
    {
        public virtual int ID { get; set; }
        public virtual string EmpName { get; set; }
        public virtual _401k PlanParent { get; set; }
    }
}