

using System;
using System.Collections.Generic;
namespace NHibernate.Test.NHSpecificTest.NH3253
{
    public class ParentCategoryEnt
    {
        private Int32? id;
        public virtual Int32? Id
        {
            get { return id; }
            set { id = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }
    }

    #region Teste T1
    public class T1ParentCpId
    {
        public T1ParentCpId()
        {
        }

        public T1ParentCpId(Int32? idA, Int32? idB)
        {
            this.idA = idA;
            this.idB = idB;
        }

        private Int32? idA;
        public virtual Int32? IdA
        {
            get { return idA; }
            set { idA = value; }
        }

        private Int32? idB;
        public virtual Int32? IdB
        {
            get { return idB; }
            set { idB = value; }
        }

        public override int GetHashCode()
        {
            return this.IdA.GetHashCode() + this.IdB.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            T1ParentCpId cpIdObj = (T1ParentCpId)obj;
            return this.IdA.Equals(cpIdObj.IdA) && this.IdB.Equals(cpIdObj.IdB);
        }

        public override string ToString()
        {
            return this.IdA + ", " + this.IdB;
        }
    }

    public class T1ParentEnt
    {
        public T1ParentEnt()
        {
        }

        private T1ParentCpId cpId;
        /// <summary>
        /// Entity key
        /// </summary>
        public virtual T1ParentCpId CpId
        {
            get { return cpId; }
            set { cpId = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }

        private ICollection<T1ChildEnt> childList;
        public virtual ICollection<T1ChildEnt> ChildList
        {
            get { return childList; }
            set { childList = value; }
        }

        private ParentCategoryEnt parentCategory;
        public virtual ParentCategoryEnt ParentCategory
        {
            get { return parentCategory; }
            set { parentCategory = value; }
        }
    }

    public class T1ChildCpId
    {
        public T1ChildCpId()
        {
        }

        public T1ChildCpId(T1ParentEnt parent, Int32? subId)
        {
            this.parent = parent;
            this.subId = subId;
        }

        private T1ParentEnt parent;
        public virtual T1ParentEnt Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private Int32? subId;
        public virtual Int32? SubId
        {
            get { return subId; }
            set { subId = value; }
        }

        public override int GetHashCode()
        {
            return this.Parent.CpId.GetHashCode() + this.SubId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            T1ChildCpId objCpId = (T1ChildCpId)obj;
            return this.Parent.CpId.Equals(objCpId.Parent.CpId) && this.SubId.Equals(objCpId.SubId);
        }

        public override string ToString()
        {
            return "[T1ParentEnt], " + this.SubId.ToString();
        }
    }

    public class T1ChildEnt
    {
        private T1ChildCpId cpId;
        public virtual T1ChildCpId CpId
        {
            get { return cpId; }
            set { cpId = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }
    } 
    #endregion

    #region Teste T2
    public class T2ParentCpId
    {
        public T2ParentCpId()
        {
        }

        public T2ParentCpId(Int32? idA, Int32? idB)
        {
            this.idA = idA;
            this.idB = idB;
        }

        private Int32? idA;
        public virtual Int32? IdA
        {
            get { return idA; }
            set { idA = value; }
        }

        private Int32? idB;
        public virtual Int32? IdB
        {
            get { return idB; }
            set { idB = value; }
        }

        public override int GetHashCode()
        {
            return this.IdA.GetHashCode() + this.IdB.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            T2ParentCpId cpIdObj = (T2ParentCpId)obj;
            return this.IdA.Equals(cpIdObj.IdA) && this.IdB.Equals(cpIdObj.IdB);
        }

        public override string ToString()
        {
            return this.IdA + ", " + this.IdB;
        }
    }

    public class T2ParentEnt
    {
        public T2ParentEnt()
        {
        }

        private T2ParentCpId cpId;
        /// <summary>
        /// Entity key
        /// </summary>
        public virtual T2ParentCpId CpId
        {
            get { return cpId; }
            set { cpId = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }

        private ICollection<T2ChildEnt> childList;
        public virtual ICollection<T2ChildEnt> ChildList
        {
            get { return childList; }
            set { childList = value; }
        }

        private ParentCategoryEnt parentCategory;
        public virtual ParentCategoryEnt ParentCategory
        {
            get { return parentCategory; }
            set { parentCategory = value; }
        }
    }

    public class T2ChildEnt
    {
        private Int32? id;
        public virtual Int32? Id
        {
            get { return id; }
            set { id = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }

        private T2ParentEnt parent;
        public virtual T2ParentEnt Parent
        {
            get { return parent; }
            set { parent = value; }
        }
    }
    #endregion

    #region Teste T3
    public class T3ParentCpId
    {
        public T3ParentCpId()
        {
        }

        public T3ParentCpId(Int32? idA, Int32? idB)
        {
            this.idA = idA;
            this.idB = idB;
        }

        private Int32? idA;
        public virtual Int32? IdA
        {
            get { return idA; }
            set { idA = value; }
        }

        private Int32? idB;
        public virtual Int32? IdB
        {
            get { return idB; }
            set { idB = value; }
        }

        public override int GetHashCode()
        {
            return this.IdA.GetHashCode() + this.IdB.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            T3ParentCpId cpIdObj = (T3ParentCpId)obj;
            return this.IdA.Equals(cpIdObj.IdA) && this.IdB.Equals(cpIdObj.IdB);
        }

        public override string ToString()
        {
            return this.IdA + ", " + this.IdB;
        }
    }

    public class T3ParentEnt
    {
        public T3ParentEnt()
        {
        }

        private T3ParentCpId cpId;
        /// <summary>
        /// Entity key
        /// </summary>
        public virtual T3ParentCpId CpId
        {
            get { return cpId; }
            set { cpId = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }

        private ICollection<T3ChildEnt> childList;
        public virtual ICollection<T3ChildEnt> ChildList
        {
            get { return childList; }
            set { childList = value; }
        }

        private ParentCategoryEnt parentCategory;
        public virtual ParentCategoryEnt ParentCategory
        {
            get { return parentCategory; }
            set { parentCategory = value; }
        }
    }

    public class T3ChildCpId
    {
        public T3ChildCpId()
        {
        }

        public T3ChildCpId(T3ParentEnt parent, Int32? subId)
        {
            this.parent = parent;
            this.subId = subId;
        }

        private T3ParentEnt parent;
        public virtual T3ParentEnt Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        private Int32? subId;
        public virtual Int32? SubId
        {
            get { return subId; }
            set { subId = value; }
        }

        public override int GetHashCode()
        {
            return this.Parent.CpId.GetHashCode() + this.SubId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            T3ChildCpId objCpId = (T3ChildCpId)obj;
            return this.Parent.CpId.Equals(objCpId.Parent.CpId) && this.SubId.Equals(objCpId.SubId);
        }

        public override string ToString()
        {
            return "[T3ParentEnt], " + this.SubId.ToString();
        }
    }

    public class T3ChildEnt
    {
        private T3ChildCpId cpId;
        public virtual T3ChildCpId CpId
        {
            get { return cpId; }
            set { cpId = value; }
        }

        private String description;
        public virtual String Description
        {
            get { return description; }
            set { description = value; }
        }
    }
    #endregion
}