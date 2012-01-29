namespace NHibernate.Test.NHSpecificTest.NH3030
{
    public class EntityWithoutDefaultCtor
    {
        private string name;
        private ComponentWithoutDefaultCtor money;

        public EntityWithoutDefaultCtor(string name, ComponentWithoutDefaultCtor money = null)
        {
            this.name = name;
            this.money = money ?? new ComponentWithoutDefaultCtor("AUD", 9.99m);
        }

        public string Name
        {
            get { return name; }
        }

        public ComponentWithoutDefaultCtor Money
        {
            get { return money; }
        }
    }
}