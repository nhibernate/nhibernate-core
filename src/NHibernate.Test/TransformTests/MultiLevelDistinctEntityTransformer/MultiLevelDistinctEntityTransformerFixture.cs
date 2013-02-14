using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Test.TransformTests.MultiLevelDistinctEntityTransformer
{
    [TestFixture]
    public class when_dealing_with_the_multi_level_distinct_entity_transformer_class
    {
        protected Dictionary<System.Type, List<string>> FetchedProperties;
        protected Transform.MultiLevelDistinctEntityTransformer Transformer;

        public virtual void SetupTest()
        {
            FetchedProperties = new Dictionary<System.Type, List<string>>();
            Transformer = new Transform.MultiLevelDistinctEntityTransformer(FetchedProperties);
        }
    }

    public class and_calling_transform_list_method : when_dealing_with_the_multi_level_distinct_entity_transformer_class
    {
        protected IList Actual;
        protected IList Data;
        protected IList Expected;
    }

    public class and_passing_an_empty_list : and_calling_transform_list_method
    {
        public override void SetupTest()
        {
            base.SetupTest();
            IList data = new ArrayList();
            Actual = Transformer.TransformList(data);
        }

        [Test]
        public void then_an_empty_list_should_be_returned()
        {
            SetupTest();
            Assert.AreEqual(0, Actual.Count);
        }
    }

    public class and_passing_a_list_that_have_duplication_on_first_level : and_calling_transform_list_method
    {
        public override void SetupTest()
        {
            base.SetupTest();
            Data = new ArrayList();
            var employee = new Employee { Id = 1, FirstName = "hello", Nationality = new Nationality { Id = 1, Name = "Nationality1" } };
            var employee2 = new Employee { Id = 2, FirstName = "Job2", Nationality = new Nationality { Id = 1, Name = "Nationality1" } };
            Data.Add(employee);
            Data.Add(employee);
            Data.Add(employee2);
            Expected = new ArrayList { employee, employee2 };
            Actual = Transformer.TransformList(Data);
        }

        [Test]
        public void then_duplicated_values_should_be_removed_from_the_list()
        {
            SetupTest();
            Assert.AreEqual(Expected.Count, Actual.Count);
            Assert.AreEqual(Expected[0], Actual[0]);
            Assert.AreEqual(Expected[1], Actual[1]);
        }
    }

    public class and_passing_a_list_the_have_duplication_on_second_level : and_calling_transform_list_method
    {
        public override void SetupTest()
        {
            base.SetupTest();
            Data = new ArrayList();
            var employee = new Employee { Id = 1, FirstName = "hello", Nationality = new Nationality { Id = 1, Name = "Nationality1" } };
            var child1 = new Child
            {
                Id = 1,
                FirstName = "Auth1",
                Employee = employee,
            };
            var child2 = new Child
            {
                Id = 2,
                FirstName = "Auth2",
                Employee = employee,
            };
            employee.Children.Add(child1);
            employee.Children.Add(child1);
            employee.Children.Add(child2);
            var employee2 = new Employee { Id = 2, FirstName = "Job2", Nationality = new Nationality { Id = 1, Name = "Nationality1" } };
            Data.Add(employee);
            Data.Add(employee);
            Data.Add(employee2);
        }
    }

    public class and_the_second_level_is_fetched : and_passing_a_list_the_have_duplication_on_second_level
    {
        public override void SetupTest()
        {
            base.SetupTest();
            FetchedProperties.Add(typeof(Employee), new List<string> { "Children" });
            Actual = Transformer.TransformList(Data);
        }

        [Test]
        public void then_the_duplication_from_the_first_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(2, Actual.Count);
        }

        [Test]
        public void then_the_duplication_from_the_second_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(2, ((Employee)Actual[0]).Children.Count);
        }
    }

    public class and_passing_a_list_that_have_duplication_on_fetched_third_level : and_calling_transform_list_method
    {
        private IList<Child> _expectedChildren;
        private IList<Passport> _expectedPassports;

        public override void SetupTest()
        {
            base.SetupTest();
            FetchedProperties.Add(typeof(Employee), new List<string> { "Children" });
            FetchedProperties.Add(typeof(Child), new List<string> { "Passports" });
            Data = new ArrayList();
            var employee = new Employee { Id = 1, FirstName = "hello", Nationality = new Nationality { Id = 1, Name = "Nationality1" } };
            var child1 = new Child
            {
                Id = 1,
                Employee = employee,
                FirstName = "FirstName1",
            };
            var passport1 = new Passport
            {
                Id = 1,
                Description = "passport1",
                Child = child1
            };
            var passport2 = new Passport
            {
                Id = 2,
                Description = "passport2",
                Child = child1
            };
            child1.Passports.Add(passport1);
            child1.Passports.Add(passport1);
            child1.Passports.Add(passport2);
            var child2 = new Child
            {
                Id = 2,
                Employee = employee,
                FirstName = "FirstName2",
            };
            employee.Children.Add(child1);
            employee.Children.Add(child1);
            employee.Children.Add(child2);
            var employee2 = new Employee { Id = 2, FirstName = "Employee1", Nationality = new Nationality { Id = 1, Name = "Nationality1" }};
            Data.Add(employee);
            Data.Add(employee);
            Data.Add(employee2);
            _expectedPassports= new List<Passport> { passport1, passport2 };
            _expectedChildren= new List<Child> { child1, child2 };
            Expected = new ArrayList { employee, employee2 };
            Actual = Transformer.TransformList(Data);
        }

        [Test]
        public void then_the_duplication_from_the_first_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(Expected.Count, Actual.Count);
            Assert.AreEqual(Expected[0], Actual[0]);
            Assert.AreEqual(Expected[1], Actual[1]);
        }

        [Test]
        public void then_the_duplication_from_the_second_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(_expectedChildren.Count,
                         ((Employee)Actual[0]).Children.Count);
            Assert.True(
                _expectedChildren.SequenceEqual(((Employee)Actual[0]).Children),
                "Children are not Equal");
        }

        [Test]
        public void then_the_duplication_from_the_third_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(_expectedPassports.Count,
                         ((Employee)Actual[0]).Children[0].Passports.Count);
            Assert.True(
                _expectedPassports.SequenceEqual(((Employee)Actual[0]).Children[0].Passports),
                "Passports are not equal");
        }
    }

    public class and_the_second_level_is_not_fetched : and_passing_a_list_the_have_duplication_on_second_level
    {
        public override void SetupTest()
        {
            base.SetupTest();
            Actual = Transformer.TransformList(Data);
        }

        [Test]
        public void then_the_duplication_from_the_first_level_should_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(2, Actual.Count);
        }

        [Test]
        public void then_the_duplication_from_the_second_level_should_not_be_removed_from_list()
        {
            SetupTest();
            Assert.AreEqual(3,
                         ((Employee)Actual[0]).Children.Count);
        }
    }
}