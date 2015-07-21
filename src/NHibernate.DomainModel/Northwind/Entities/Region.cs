using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NHibernate.DomainModel.Northwind.Entities
{
    public class Region : Entity<Region>
    {
        private readonly ISet<Territory> _territories;
        private string _description;

        public Region() : this(null)
        {
        }

        public Region(string description)
        {
            _description = description;
            _territories = new HashSet<Territory>();
        }

        public virtual string Description
        {
            get { return _description.Trim(); }
            set { _description = value; }
        }

        public virtual ReadOnlyCollection<Territory> Territories
        {
            get { return new List<Territory>(_territories).AsReadOnly(); }
        }

        public virtual void AddTerritory(Territory territory)
        {
            if (!_territories.Contains(territory))
            {
                territory.Region = this;
                _territories.Add(territory);
            }
        }

        public virtual void RemoveTerritory(Territory territory)
        {
            if (_territories.Contains(territory))
            {
                territory.Region = null;
                _territories.Remove(territory);
            }
        }
    }
}