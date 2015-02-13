using System.Linq;
using NHibernate.DdlGen.Model;
using NHibernate.DdlGen.Operations;
using NHibernate.Dialect;
using NUnit.Framework;

namespace NHibernate.Test.DdlGen.Operations
{
    [TestFixture]
    public class CreateSequenceDdlOperationFixture
    {
        private class FixtureDialect : GenericDialect
        {
            public override string GetCreateSequenceString(string sequenceName)
            {
                return "create sequence " + sequenceName;
            }

            protected override string GetCreateSequenceString(string sequenceName, int initialValue, int incrementSize)
            {
                return string.Format("create sequence {0} as int start with {1} increment by {2}", sequenceName, initialValue, incrementSize);
            }
        }
        [Test]
        public void CanCreateSequenceViaParams()
        {
            var model = new CreateSequenceModel
            {
                Name = new DbName("Bob"),
                Parameters = "things and stuff"
            };
            var operation = new CreateSequenceDdlOperation(model);
            var expected = "create sequence Bob things and stuff";
            var actual = operation.GetStatements(new FixtureDialect()).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CanCreateSequenceViaSettings()
        {
            var model = new CreateSequenceModel
            {
                Name = new DbName("Bob"),
                IncrementSize = 2,
                InitialValue = 2
            };
            var operation = new CreateSequenceDdlOperation(model);
            var expected = "create sequence Bob as int start with 2 increment by 2";
            var actual = operation.GetStatements(new FixtureDialect()).Single();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}