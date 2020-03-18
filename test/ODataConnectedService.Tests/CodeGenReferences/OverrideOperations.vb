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


'Generation date: 18.03.2020 15:14:36
Namespace OverrideOperations
    '''<summary>
    '''There are no comments for OverrideOperationsContainer in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("OverrideOperationsContainer")>  _
    Partial Public Class OverrideOperationsContainer
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        '''<summary>
        '''Initialize a new OverrideOperationsContainer object.
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
        Private Shared ROOTNAMESPACE As String = GetType(OverrideOperationsContainer).Namespace.Remove(GetType(OverrideOperationsContainer).Namespace.LastIndexOf("OverrideOperations"))
        '''<summary>
        '''Since the namespace configured for this service reference
        '''in Visual Studio is different from the one indicated in the
        '''server schema, use type-mappers to map between the two.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Function ResolveTypeFromName(ByVal typeName As String) As Global.System.Type
            Dim resolvedType As Global.System.Type = Me.DefaultResolveType(typeName, "OverrideOperations", String.Concat(ROOTNAMESPACE, "OverrideOperations"))
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
            Dim originalNameAttribute As Global.Microsoft.OData.Client.OriginalNameAttribute =
                CType(Global.System.Linq.Enumerable.SingleOrDefault(Global.Microsoft.OData.Client.Utility.GetCustomAttributes(clientType, GetType(Global.Microsoft.OData.Client.OriginalNameAttribute), true)), Global.Microsoft.OData.Client.OriginalNameAttribute)
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "OverrideOperations"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                If (Not (originalNameAttribute) Is Nothing) Then
                    Return String.Concat("OverrideOperations.", originalNameAttribute.OriginalName)
                End If
                Return String.Concat("OverrideOperations.", clientType.Name)
            End If
            If (Not (originalNameAttribute) Is Nothing) Then
                Dim fullName As String = clientType.FullName.Substring(ROOTNAMESPACE.Length)
                Return fullName.Remove(fullName.LastIndexOf(clientType.Name)) + originalNameAttribute.OriginalName
            End If
            Return clientType.FullName.Substring(ROOTNAMESPACE.Length)
        End Function
        '''<summary>
        '''There are no comments for ETSets in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ETSets")>  _
        Public Overridable ReadOnly Property ETSets() As Global.Microsoft.OData.Client.DataServiceQuery(Of ET)
            Get
                If (Me._ETSets Is Nothing) Then
                    Me._ETSets = MyBase.CreateQuery(Of ET)("ETSets")
                End If
                Return Me._ETSets
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ETSets As Global.Microsoft.OData.Client.DataServiceQuery(Of ET)
        '''<summary>
        '''There are no comments for ETSets in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddToETSets(ByVal eT As ET)
            MyBase.AddObject("ETSets", eT)
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""OverrideOperations"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""ET"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""UserName"" />" & _
 "        </Key>" & _
 "        <Property Name=""UserName"" Type=""Edm.String"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""DerivedET"" BaseType=""OverrideOperations.ET"">" & _
 "        <Property Name=""DerivedComplexP"" Type=""OverrideOperations.DerivedCT"" />" & _
 "      </EntityType>" & _
 "      <ComplexType Name=""CT"">" & _
 "        <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"" />" & _
 "      </ComplexType>" & _
 "      <ComplexType Name=""DerivedCT"" BaseType=""OverrideOperations.CT"">" & _
 "        <Property Name=""Description"" Type=""Edm.String"" Nullable=""false"" />" & _
 "      </ComplexType>" & _
 "      <Function Name=""FunctionWithoutParameter"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""OverrideOperations.ET"" Nullable=""false"" />" & _
 "        <ReturnType Type=""OverrideOperations.CT"" Nullable=""false"" />" & _
 "      </Function>" & _
 "      <Function Name=""FunctionWithoutParameter"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""OverrideOperations.DerivedET"" Nullable=""false"" />" & _
 "        <ReturnType Type=""OverrideOperations.CT"" Nullable=""false"" />" & _
 "      </Function>" & _
 "      <Action Name=""ActionWithParameter"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""OverrideOperations.ET"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""OverrideOperations.ET"" Nullable=""false"" />" & _
 "      </Action>" & _
 "      <Action Name=""ActionWithParameter"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""OverrideOperations.DerivedET"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""OverrideOperations.ET"" Nullable=""false"" />" & _
 "      </Action>" & _
 "      <Function Name=""FunctionBoundToCollectionOfEntity"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "      </Function>" & _
 "      <Function Name=""FunctionBoundToCollectionOfEntity"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""Collection(OverrideOperations.DerivedET)"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "      </Function>" & _
 "      <Action Name=""ActionBoundToCollectionOfEntity"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "      </Action>" & _
 "      <Action Name=""ActionBoundToCollectionOfEntity"" IsBound=""true"">" & _
 "        <Parameter Name=""entity"" Type=""Collection(OverrideOperations.DerivedET)"" Nullable=""false"" />" & _
 "        <Parameter Name=""p1"" Type=""Edm.String"" Nullable=""false"" />" & _
 "        <ReturnType Type=""Collection(OverrideOperations.ET)"" Nullable=""false"" />" & _
 "      </Action>" & _
 "      <EntityContainer Name=""OverrideOperationsContainer"">" & _
 "        <EntitySet Name=""ETSets"" EntityType=""OverrideOperations.ET"">" & _
 "          <NavigationPropertyBinding Path=""NavP"" Target=""ETSets"" />" & _
 "        </EntitySet>" & _
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
    '''There are no comments for ETSingle in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("ETSingle")>  _
    Partial Public Class ETSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of ET)
        ''' <summary>
        ''' Initialize a new ETSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new ETSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new ETSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of ET))
            MyBase.New(query)
        End Sub
    End Class
    '''<summary>
    '''There are no comments for ET in the schema.
    '''</summary>
    '''<KeyProperties>
    '''UserName
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("UserName")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("ET")>  _
    Partial Public Class ET
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        '''<summary>
        '''Create a new ET object.
        '''</summary>
        '''<param name="userName">Initial value of UserName.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateET(ByVal userName As String) As ET
            Dim eT As ET = New ET()
            eT.UserName = userName
            Return eT
        End Function
        '''<summary>
        '''There are no comments for Property UserName in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("UserName")>  _
        Public Overridable Property UserName() As String
            Get
                Return Me._UserName
            End Get
            Set
                Me.OnUserNameChanging(value)
                Me._UserName = value
                Me.OnUserNameChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _UserName As String
        Partial Private Sub OnUserNameChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnUserNameChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for FunctionWithoutParameter in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionWithoutParameter")>  _
        Public Overridable Function FunctionWithoutParameter() As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.CT)
            Dim requestUri As Global.System.Uri = Nothing
            Context.TryGetUri(Me, requestUri)
            Return Me.Context.CreateFunctionQuerySingle(Of OverrideOperations.CT)(String.Join("/", Global.System.Linq.Enumerable.Select(Global.System.Linq.Enumerable.Skip(requestUri.Segments, Me.Context.BaseUri.Segments.Length), Function(s) s.Trim("/"C))), "/OverrideOperations.FunctionWithoutParameter", False)
        End Function
        ''' <summary>
        ''' There are no comments for ActionWithParameter in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionWithParameter")>  _
        Public Overridable Function ActionWithParameter(p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)
            Dim resource As Global.Microsoft.OData.Client.EntityDescriptor = Context.EntityTracker.TryGetEntityDescriptor(Me)
            If resource Is Nothing Then
                Throw New Global.System.Exception("cannot find entity")
            End If

            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)(Me.Context, resource.EditLink.OriginalString.Trim("/"C) + "/OverrideOperations.ActionWithParameter", New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
    End Class
    '''<summary>
    '''There are no comments for DerivedETSingle in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("DerivedETSingle")>  _
    Partial Public Class DerivedETSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of DerivedET)
        ''' <summary>
        ''' Initialize a new DerivedETSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new DerivedETSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new DerivedETSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of DerivedET))
            MyBase.New(query)
        End Sub
    End Class
    '''<summary>
    '''There are no comments for DerivedET in the schema.
    '''</summary>
    '''<KeyProperties>
    '''UserName
    '''</KeyProperties>
    <Global.Microsoft.OData.Client.Key("UserName")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("DerivedET")>  _
    Partial Public Class DerivedET
        Inherits ET
        '''<summary>
        '''Create a new DerivedET object.
        '''</summary>
        '''<param name="userName">Initial value of UserName.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateDerivedET(ByVal userName As String) As DerivedET
            Dim derivedET As DerivedET = New DerivedET()
            derivedET.UserName = userName
            Return derivedET
        End Function
        '''<summary>
        '''There are no comments for Property DerivedComplexP in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("DerivedComplexP")>  _
        Public Overridable Property DerivedComplexP() As OverrideOperations.DerivedCT
            Get
                Return Me._DerivedComplexP
            End Get
            Set
                Me.OnDerivedComplexPChanging(value)
                Me._DerivedComplexP = value
                Me.OnDerivedComplexPChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _DerivedComplexP As OverrideOperations.DerivedCT
        Partial Private Sub OnDerivedComplexPChanging(ByVal value As OverrideOperations.DerivedCT)
        End Sub
        Partial Private Sub OnDerivedComplexPChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for FunctionWithoutParameter in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionWithoutParameter")>  _
        Public Overridable Overloads Function FunctionWithoutParameter() As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.CT)
            Dim requestUri As Global.System.Uri = Nothing
            Context.TryGetUri(Me, requestUri)
            Return Me.Context.CreateFunctionQuerySingle(Of OverrideOperations.CT)(String.Join("/", Global.System.Linq.Enumerable.Select(Global.System.Linq.Enumerable.Skip(requestUri.Segments, Me.Context.BaseUri.Segments.Length), Function(s) s.Trim("/"C))), "/OverrideOperations.FunctionWithoutParameter", False)
        End Function
        ''' <summary>
        ''' There are no comments for ActionWithParameter in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionWithParameter")>  _
        Public Overridable Overloads Function ActionWithParameter(p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)
            Dim resource As Global.Microsoft.OData.Client.EntityDescriptor = Context.EntityTracker.TryGetEntityDescriptor(Me)
            If resource Is Nothing Then
                Throw New Global.System.Exception("cannot find entity")
            End If

            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)(Me.Context, resource.EditLink.OriginalString.Trim("/"C) + "/OverrideOperations.ActionWithParameter", New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
    End Class
    '''<summary>
    '''There are no comments for CT in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("CT")>  _
    Partial Public Class CT
        '''<summary>
        '''Create a new CT object.
        '''</summary>
        '''<param name="name">Initial value of Name.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateCT(ByVal name As String) As CT
            Dim cT As CT = New CT()
            cT.Name = name
            Return cT
        End Function
        '''<summary>
        '''There are no comments for Property Name in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("Name")>  _
        Public Overridable Property Name() As String
            Get
                Return Me._Name
            End Get
            Set
                Me.OnNameChanging(value)
                Me._Name = value
                Me.OnNameChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Name As String
        Partial Private Sub OnNameChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnNameChanged()
        End Sub
    End Class
    '''<summary>
    '''There are no comments for DerivedCT in the schema.
    '''</summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("DerivedCT")>  _
    Partial Public Class DerivedCT
        Inherits CT
        '''<summary>
        '''Create a new DerivedCT object.
        '''</summary>
        '''<param name="name">Initial value of Name.</param>
        '''<param name="description">Initial value of Description.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateDerivedCT(ByVal name As String, ByVal description As String) As DerivedCT
            Dim derivedCT As DerivedCT = New DerivedCT()
            derivedCT.Name = name
            derivedCT.Description = description
            Return derivedCT
        End Function
        '''<summary>
        '''There are no comments for Property Description in the schema.
        '''</summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("Description")>  _
        Public Overridable Property Description() As String
            Get
                Return Me._Description
            End Get
            Set
                Me.OnDescriptionChanging(value)
                Me._Description = value
                Me.OnDescriptionChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Description As String
        Partial Private Sub OnDescriptionChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnDescriptionChanged()
        End Sub
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type OverrideOperations.ET as OverrideOperations.ETSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET), ByVal keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As OverrideOperations.ETSingle
            Return New OverrideOperations.ETSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type OverrideOperations.ET as OverrideOperations.ETSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="userName">The value of userName</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET),
            userName As String) As OverrideOperations.ETSingle
            Dim keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "UserName", userName }
            }
            Return New OverrideOperations.ETSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type OverrideOperations.DerivedET as OverrideOperations.DerivedETSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.DerivedET), ByVal keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As OverrideOperations.DerivedETSingle
            Return New OverrideOperations.DerivedETSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type OverrideOperations.DerivedET as OverrideOperations.DerivedETSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="source">source entity set</param>
        ''' <param name="userName">The value of userName</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.DerivedET),
            userName As String) As OverrideOperations.DerivedETSingle
            Dim keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "UserName", userName }
            }
            Return New OverrideOperations.DerivedETSingle(source.Context, source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(source.Context, keys)))
        End Function
        ''' <summary>
        ''' Cast an entity of type OverrideOperations.ET to its derived type OverrideOperations.DerivedET
        ''' </summary>
        ''' <param name="source">source entity</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function CastToDerivedET(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.ET)) As OverrideOperations.DerivedETSingle
            Dim query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.DerivedET) = source.CastTo(Of OverrideOperations.DerivedET)()
            Return New OverrideOperations.DerivedETSingle(source.Context, query.GetPath(Nothing))
        End Function
        ''' <summary>
        ''' There are no comments for FunctionWithoutParameter in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionWithoutParameter")>  _
        Public Function FunctionWithoutParameter(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.ET)) As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.CT)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            
            Return source.CreateFunctionQuerySingle(Of OverrideOperations.CT)("OverrideOperations.FunctionWithoutParameter", False)
        End Function
        ''' <summary>
        ''' There are no comments for FunctionWithoutParameter in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionWithoutParameter")>  _
        Public Function FunctionWithoutParameter(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.DerivedET)) As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.CT)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            
            Return source.CreateFunctionQuerySingle(Of OverrideOperations.CT)("OverrideOperations.FunctionWithoutParameter", False)
        End Function
        ''' <summary>
        ''' There are no comments for FunctionBoundToCollectionOfEntity in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionBoundToCollectionOfEntity")>  _
        Public Function FunctionBoundToCollectionOfEntity(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET), p1 As String) As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            
            Return source.CreateFunctionQuery(Of OverrideOperations.ET)("OverrideOperations.FunctionBoundToCollectionOfEntity", False, New Global.Microsoft.OData.Client.UriOperationParameter("p1", p1))
        End Function
        ''' <summary>
        ''' There are no comments for FunctionBoundToCollectionOfEntity in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("FunctionBoundToCollectionOfEntity")>  _
        Public Function FunctionBoundToCollectionOfEntity(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.DerivedET), p1 As String) As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            
            Return source.CreateFunctionQuery(Of OverrideOperations.ET)("OverrideOperations.FunctionBoundToCollectionOfEntity", False, New Global.Microsoft.OData.Client.UriOperationParameter("p1", p1))
        End Function
        ''' <summary>
        ''' There are no comments for ActionWithParameter in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionWithParameter")>  _
        Public Function ActionWithParameter(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.ET), p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)(source.Context, source.AppendRequestUri("OverrideOperations.ActionWithParameter"), New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
        ''' <summary>
        ''' There are no comments for ActionWithParameter in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionWithParameter")>  _
        Public Function ActionWithParameter(ByVal source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of OverrideOperations.DerivedET), p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of OverrideOperations.ET)(source.Context, source.AppendRequestUri("OverrideOperations.ActionWithParameter"), New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
        ''' <summary>
        ''' There are no comments for ActionBoundToCollectionOfEntity in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionBoundToCollectionOfEntity")>  _
        Public Function ActionBoundToCollectionOfEntity(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.ET), p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuery(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuery(Of OverrideOperations.ET)(source.Context, source.AppendRequestUri("OverrideOperations.ActionBoundToCollectionOfEntity"), New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
        ''' <summary>
        ''' There are no comments for ActionBoundToCollectionOfEntity in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("ActionBoundToCollectionOfEntity")>  _
        Public Function ActionBoundToCollectionOfEntity(ByVal source As Global.Microsoft.OData.Client.DataServiceQuery(Of OverrideOperations.DerivedET), p1 As String) As Global.Microsoft.OData.Client.DataServiceActionQuery(Of OverrideOperations.ET)
            If Not source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuery(Of OverrideOperations.ET)(source.Context, source.AppendRequestUri("OverrideOperations.ActionBoundToCollectionOfEntity"), New Global.Microsoft.OData.Client.BodyOperationParameter("p1", p1))
        End Function
    End Module
End Namespace
