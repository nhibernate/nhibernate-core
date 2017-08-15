using System.Xml;

namespace NHibernate.Test.NHSpecificTest.NH3518
{
	public class ClassWithXmlMember
    {
        // Used by reflection
#pragma warning disable CS0169 // The field is never used
        private int _id;
#pragma warning restore CS0169 // The field is never used

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
