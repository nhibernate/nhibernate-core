using System;
using System.Collections.Generic;
using NHibernate.Persister.Collection;
using NHibernate.Persister.Entity;
using NHibernate.Type;

namespace NHibernate.Loader.Criteria
{
    public class ComponentCollectionCriteriaInfoProvider : ICriteriaInfoProvider
    {
        private readonly IQueryableCollection persister;
        private readonly IDictionary<String, IType> subTypes = new Dictionary<string, IType>();

        public ComponentCollectionCriteriaInfoProvider(IQueryableCollection persister)
        {
            this.persister = persister;
            if (!persister.ElementType.IsComponentType)
            {
                throw new ArgumentException("persister for role " + persister.Role + " is not a collection-of-component");
            }

            var componentType = (ComponentType)persister.ElementType;
            var names = componentType.PropertyNames;
            var types = componentType.Subtypes;

            for (var i = 0; i < names.Length; i++)
            {
                subTypes.Add(names[i], types[i]);
            }

        }

        public String Name
        {
            get
            {
                return persister.Role;
            }
        }

        public string[] Spaces
        {
            get
            {
                return persister.CollectionSpaces;
            }
        }

        public IPropertyMapping PropertyMapping
        {
            get
            {
                return persister;
            }
        }

        public IType GetType(String relativePath)
        {
            // TODO: can a component have a nested component? then we may need to do something more here...
            if (relativePath.IndexOf('.') >= 0)
                throw new ArgumentException("dotted paths not handled (yet?!) for collection-of-component");

            IType type;

            if (subTypes.TryGetValue(relativePath, out type) == false)
                throw new ArgumentException("property " + relativePath + " not found in component of collection " + Name);

            return type;
        }
    }
}