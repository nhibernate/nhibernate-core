using System.Xml;

namespace NHibernate.Test.NHSpecificTest.NH3518
{
	public class ClassWithXmlMember
    {
        private int _id;

        public ClassWithXmlMember(string name, XmlDocument xml)
        {
            Xml = xml;
            this.Name = name;
        }

        private ClassWithXmlMember()
        {
        }

        public string Name { get; private set; }

        public XmlDocument Xml { get; private set; }
    }
}
