'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On


'Generation date: 18.03.2020 14:15:36
Namespace Foo
    '''<summary>
    '''There are no comments for EntityContainer in the schema.
    '''</summary>
    Partial Public Class EntityContainer
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        '''<summary>
        '''Initialize a new EntityContainer object.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            MyBase.New(serviceRoot, Global.Microsoft.OData.Client.ODataProtocolVersion.V4)
            Me.ResolveName = AddressOf Me.ResolveNameFromType
            Me.ResolveType = AddressOf Me.ResolveTypeFromName
            Me.OnContextCreated
            Me.Format.LoadServiceModel = AddressOf GeneratedEdmModel.GetInstance
            Me.Format.UseJson()
        End Sub
        Partial Private Sub OnContextCreated()
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private Shared ROOTNAMESPACE As String = GetType(EntityContainer).Namespace.Remove(GetType(EntityContainer).Namespace.LastIndexOf("Foo"))
        '''<summary>
        '''Since the namespace configured for this service reference
        '''in Visual Studio is different from the one indicated in the
        '''server schema, use type-mappers to map between the two.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Function ResolveTypeFromName(ByVal typeName As String) As Global.System.Type
            Dim resolvedType As Global.System.Type = Me.DefaultResolveType(typeName, "NamespacePrefixWithInheritence", String.Concat(ROOTNAMESPACE, "Foo"))
            If (Not (resolvedType) Is Nothing) Then
                Return resolvedType
            End If
            Return Nothing
        End Function
        '''<summary>
        '''Since the namespace configured for this service reference
        '''in Visual Studio is different from the one indicated in the
        '''server schema, use type-mappers to map between the two.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Function ResolveNameFromType(ByVal clientType As Global.System.Type) As String
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "Foo"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                Return String.Concat("NamespacePrefixWithInheritence.", clientType.Name)
            End If
            Return clientType.FullName.Substring(ROOTNAMESPACE.Length)
        End Function
        '''<summary>
        '''There are no comments for Set1 in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public ReadOnly Property Set1() As Global.Microsoft.OData.Client.DataServiceQuery(Of EntityType)
            Get
                If (Me._Set1 Is Nothing) Then
                    Me._Set1 = MyBase.CreateQuery(Of EntityType)("Set1")
                End If
                Return Me._Set1
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Set1 As Global.Microsoft.OData.Client.DataServiceQuery(Of EntityType)
        '''<summary>
        '''There are no comments for Set1 in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Sub AddToSet1(ByVal entityType As EntityType)
            MyBase.AddObject("Set1", entityType)
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""NamespacePrefixWithInheritence"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""EntityBase"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""IdKey"" />" & _
 "        </Key>" & _
 "        <Property Name=""IdKey"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""EntityType"" BaseType=""NamespacePrefixWithInheritence.EntityBase"">" & _
 "        <Property Name=""ID"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityContainer Name=""EntityContainer"">" & _
 "        <EntitySet Name=""Set1"" EntityType=""NamespacePrefixWithInheritence.EntityType"" />" & _
 "      </EntityContainer>" & _
 "    </Schema>" & _
 "  </edmx:DataServices>" & _
 "</edmx:Edmx>"
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Public Shared Function GetInstance() As Global.Microsoft.OData.Edm.IEdmModel
                Return ParsedModel
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared Function LoadModelFromString() As Global.Microsoft.OData.Edm.IEdmModel
                Dim reader As Global.System.Xml.XmlReader = CreateXmlReader(Edmx)
                Try
                    Return Global.Microsoft.OData.Edm.Csdl.CsdlReader.Parse(reader)
                Finally
                    CType(reader,Global.System.IDisposable).Dispose
                End Try
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared Function CreateXmlReader(ByVal edmxToParse As String) As Global.System.Xml.XmlReader
                Return Global.System.Xml.XmlReader.Create(New Global.System.IO.StringReader(edmxToParse))
            End Function
        End Class
    End Class
    '''<summary>
    '''There are no comments for EntityBaseSingle in the schema.
    '''</summary>
    Partial Public Class EntityBaseSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of EntityBase)
        ''' <summary>
        ''' Initialize a new EntityBaseSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new EntityBaseSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new EntityBaseSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of EntityBase))
            MyBase.New(query)
        End Sub
    End Class
    '''<summary>
    '''There are no comments for EntityBase in the schema.
    '''</summary>
    '''<KeyProperties>
    '''IdKey
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("IdKey")>  _
    Partial Public Class EntityBase
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        '''<summary>
        '''Create a new EntityBase object.
        '''</summary>
        '''<param name="idKey">Initial value of IdKey.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateEntityBase(ByVal idKey As Integer) As EntityBase
            Dim entityBase As EntityBase = New EntityBase()
            entityBase.IdKey = idKey
            Return entityBase
        End Function
        '''<summary>
        '''There are no comments for Property IdKey in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Property IdKey() As Integer
            Get
                Return Me._IdKey
            End Get
            Set
                Me.OnIdKeyChanging(value)
                Me._IdKey = value
                Me.OnIdKeyChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _IdKey As Integer
        Partial Private Sub OnIdKeyChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnIdKeyChanged()
        End Sub
    End Class
    '''<summary>
    '''There are no comments for EntityTypeSingle in the schema.
    '''</summary>
    Partial Public Class EntityTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of EntityType)
        ''' <summary>
        ''' Initialize a new EntityTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new EntityTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new EntityTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of EntityType))
            MyBase.New(query)
        End Sub
    End Class
    '''<summary>
    '''There are no comments for EntityType in the schema.
    '''</summary>
    '''<KeyProperties>
    '''IdKey
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("IdKey")>  _
    Partial Public Class EntityType
        Inherits EntityBase
        '''<summary>
        '''Create a new EntityType object.
        '''</summary>
        '''<param name="idKey">Initial value of IdKey.</param>
        '''<param name="ID">Initial value of ID.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateEntityType(ByVal idKey As Integer, ByVal ID As Integer) As EntityType
            Dim entityType As EntityType = New EntityType()
            entityType.IdKey = idKey
            entityType.ID = ID
            Return entityType
        End Function
        '''<summary>
        '''There are no comments for Property ID in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Property ID() As Integer
            Get
                Return Me._ID
            End Get
            Set
                Me.OnIDChanging(value)
                Me._ID = value
                Me.OnIDChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ID As Integer
        Partial Private Sub OnIDChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnIDChanged()
        End Sub
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type Foo.EntityBase as Foo.EntityBaseSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of Foo.EntityBase), ByVal keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As Foo.EntityBaseSingle
            Return New Foo.EntityBaseSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Foo.EntityBase as Foo.EntityBaseSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="idKey">The value of idKey</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of Foo.EntityBase),
            idKey As Integer) As Foo.EntityBaseSingle
            Dim keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "IdKey", idKey }
            }
            Return New Foo.EntityBaseSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Foo.EntityType as Foo.EntityTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of Foo.EntityType), ByVal keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As Foo.EntityTypeSingle
            Return New Foo.EntityTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Foo.EntityType as Foo.EntityTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="idKey">The value of idKey</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of Foo.EntityType),
            idKey As Integer) As Foo.EntityTypeSingle
            Dim keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "IdKey", idKey }
            }
            Return New Foo.EntityTypeSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Cast an entity of type Foo.EntityBase to its derived type Foo.EntityType
        ''' </summary>
        ''' <param name="source">source entity</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function CastToEntityType(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Foo.EntityBase)) As Foo.EntityTypeSingle
            Dim query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of Foo.EntityType) = source.CastTo(Of Foo.EntityType)()
            Return New Foo.EntityTypeSingle(source.Context, query.GetPath(Nothing))
        End Function
    End Module
End Namespace
