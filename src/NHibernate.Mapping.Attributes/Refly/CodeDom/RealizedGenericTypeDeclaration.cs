#if WHIDBEY
using System;
using System.CodeDom;
using Refly.CodeDom.Collections;
using System.IO;

namespace Refly.CodeDom
{
    public sealed class RealizedGenericTypeDeclaration : ITypeDeclaration
    {
        private ITypeDeclaration typeDeclaration;
        private TypeDeclarationCollection typeArguments = new TypeDeclarationCollection();

        public RealizedGenericTypeDeclaration(ITypeDeclaration typeDeclaration)
        {
            this.typeDeclaration = typeDeclaration;
        }

        public TypeDeclarationCollection TypeArguments
        {
            get { return this.typeArguments; }
        }

        public System.CodeDom.CodeTypeReference TypeReference
        {
            get
            {
                CodeTypeReference typeRef = this.typeDeclaration.TypeReference;
                foreach (ITypeDeclaration typeArgument in this.TypeArguments)
                    typeRef.TypeArguments.Add(typeArgument.TypeReference);
                return typeRef;
            }
        }

        public ITypeDeclaration TypeDeclaration
        {
            get { return this.typeDeclaration; }
        }

        public string FullName
        {
            get
            {
                return String.Format("{0}{1}",
                    this.TypeDeclaration.FullName,
                    this.ArgumentString()
                    );
            }
        }

        public string Name
        {
            get
            {
                return String.Format("{0}{1}",
                    this.TypeDeclaration.Name,
                    this.ArgumentString()
                    );
            }
        }

        private string ArgumentString()
        {
            StringWriter sw = new StringWriter();
            foreach (ITypeDeclaration typeArgument in this.TypeArguments)
                sw.Write("{0},", typeArgument.FullName);
            return String.Format("<{0}>", sw.ToString().TrimEnd(','));
        }
    }
}
#endif