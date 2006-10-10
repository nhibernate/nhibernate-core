.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 1:0:5000:0
}
.assembly HashCodeProvider
{
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 05 31 2E 30 2E 30 00 00 )                   // ...1.0.0..
  .custom instance void [mscorlib]System.Reflection.AssemblyInformationalVersionAttribute::.ctor(string) = ( 01 00 03 31 2E 30 00 00 )                         // ...1.0..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 22 43 6F 70 79 72 69 67 68 74 20 28 63 29   // .."Copyright (c)
                                                                                                  20 32 30 30 32 20 4D 61 74 74 69 61 73 20 53 6A   //  2002 Mattias Sj
                                                                                                  6F 67 72 65 6E 00 00 )                            // ogren..
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 10 48 61 73 68 43 6F 64 65 50 72 6F 76 69   // ...HashCodeProvi
                                                                                                64 65 72 00 00 )                                  // der..
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 33 68 74 74 70 3A 2F 2F 77 77 77 2E 6D 73   // ..3http://www.ms
                                                                                                6A 6F 67 72 65 6E 2E 6E 65 74 2F 64 6F 74 6E 65   // jogren.net/dotne
                                                                                                74 2F 65 6E 67 2F 73 61 6D 70 6C 65 73 2F 6D 69   // t/eng/samples/mi
                                                                                                73 63 2E 61 73 70 00 00 )                         // sc.asp..
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 30 55 74 69 6C 69 74 79 20 74 6F 20 63 61   // ..0Utility to ca
                                                                                                    6C 6C 20 4F 62 6A 65 63 74 2E 47 65 74 48 61 73   // ll Object.GetHas
                                                                                                    68 43 6F 64 65 20 6E 6F 6E 76 69 72 74 75 61 6C   // hCode nonvirtual
                                                                                                    6C 79 2E 00 00 )                                  // ly...
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 10 48 61 73 68 43 6F 64 65 50 72 6F 76 69   // ...HashCodeProvi
                                                                                              64 65 72 00 00 )                                  // der..
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module HashCodeProvider.dll
// MVID: {52F1D165-1EC9-4B77-8031-3AAFBBC27452}
.imagebase 0x11000000
.subsystem 0x00000003
.file alignment 4096
.corflags 0x00000001
// Image base: 0x07b50000
.namespace NHibernate 
{
  .class public serializable IdentityHashCodeProvider
       implements [mscorlib]System.Collections.IHashCodeProvider
  {
    .method private hidebysig newslot final virtual 
          instance int32 System.Collections.IHashCodeProvider.GetHashCode(object obj)
    {
      .override [mscorlib]System.Collections.IHashCodeProvider::GetHashCode
      ldarg.1
      call      instance int32 [mscorlib]System.Object::GetHashCode()
      ret
    }

    .method public hidebysig specialname rtspecialname instance void .ctor()
    {
      ldarg.0
      call      instance void [mscorlib]System.Object::.ctor()
      ret
    }
  }
}
