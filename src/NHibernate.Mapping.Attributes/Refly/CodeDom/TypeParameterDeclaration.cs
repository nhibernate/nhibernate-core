#if WHIDBEY
using System;
using System.CodeDom;
using Refly.CodeDom.Collections;

namespace Refly.CodeDom
{
    public sealed class TypeParameterDeclaration : Declaration
    {
        private bool hasConstructorConstraint = false;
        private TypeDeclarationCollection constraints = new TypeDeclarationCollection();

        public TypeParameterDeclaration(string name, NameConformer conformer)
            : base(name, conformer)
        { }

        public TypeDeclarationCollection Constraints
        {
            get { return this.constraints; }
        }

        public bool HasConstructorConstraint
        {
            get { return this.hasConstructorConstraint; }
            set { this.hasConstructorConstraint = value; }
        }

        public CodeTypeParameter ToCodeDom()
        {
            CodeTypeParameter typeParameter = new CodeTypeParameter(this.Name);
            typeParameter.CustomAttributes.AddRange(this.CustomAttributes.ToCodeDom());
            foreach (ITypeDeclaration constraint in this.Constraints)
                typeParameter.Constraints.Add(constraint.TypeReference);
            return typeParameter;
        }
    }
}
#endif