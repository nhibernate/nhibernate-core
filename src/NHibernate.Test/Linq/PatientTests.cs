using System.Linq;
using NHibernate.DomainModel.Northwind.Entities;
using NUnit.Framework;

namespace NHibernate.Test.Linq
{
    [TestFixture]
    public class PatientTests : LinqTestCase
    {
        [Test]
        public void CanQueryOnPropertyOfComponent()
        {
            var query = (from pr in db.PatientRecords
                         where pr.Name.LastName == "Doe"
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void CanQueryOnManyToOneOfComponent()
        {
            var florida = db.States.FirstOrDefault(x => x.Abbreviation == "FL");

            var query = (from pr in db.PatientRecords
                         where pr.Address.State == florida
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void CanQueryOnPropertyOfManyToOneOfComponent()
        {
            var query = (from pr in db.PatientRecords
                         where pr.Address.State.Abbreviation == "FL"
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void CanQueryOnPropertyOfOneToMany()
        {
            var query = (from p in db.Patients
                         where p.PatientRecords.Any(x => x.Gender == Gender.Unknown)
                         select p).ToList();

            Assert.AreEqual(1, query.Count);
        }

        [Test]
        public void CanQueryOnPropertyOfManyToOne()
        {
            var query = (from pr in db.PatientRecords
                         where pr.Patient.Active == true
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void CanQueryOnManyToOneOfManyToOne()
        {
            var drWatson = db.Physicians.FirstOrDefault(x => x.Name == "Dr Watson");

            var query = (from pr in db.PatientRecords
                         where pr.Patient.Physician == drWatson
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }

        [Test]
        public void CanQueryOnPropertyOfManyToOneOfManyToOne()
        {
            var query = (from pr in db.PatientRecords
                         where pr.Patient.Physician.Name == "Dr Watson"
                         select pr).ToList();

            Assert.AreEqual(2, query.Count);
        }
    }
}