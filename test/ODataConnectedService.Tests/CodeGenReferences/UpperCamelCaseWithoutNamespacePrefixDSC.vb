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


'Generation date: 10.03.2021 19:39:13
Namespace [Namespace].Foo.DSC
    ''' <summary>
    ''' There are no comments for BaseTypeSingle in the schema.
    ''' </summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("baseTypeSingle")>  _
    Partial Public Class BaseTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of BaseType)
        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new BaseTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of BaseType))
            MyBase.New(query)
        End Sub
    End Class
    ''' <summary>
    ''' There are no comments for BaseType in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("baseType")>  _
    Partial Public Class BaseType
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        Implements Global.System.ComponentModel.INotifyPropertyChanged
        ''' <summary>
        ''' Create a new BaseType object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateBaseType(ByVal keyProp As Integer) As BaseType
            Dim baseType As BaseType = New BaseType()
            baseType.KeyProp = keyProp
            Return baseType
        End Function
        ''' <summary>
        ''' There are no comments for Property KeyProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("keyProp")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
                Me.OnPropertyChanged("keyProp")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        ''' <summary>
        ''' This event is raised when the value of the property is changed
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements Global.System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        ''' <summary>
        ''' The value of the property is changed
        ''' </summary>
        ''' <param name="property">property name</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub
    End Class
    ''' <summary>
    ''' There are no comments for TestTypeSingle in the schema.
    ''' </summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("testTypeSingle")>  _
    Partial Public Class TestTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType)
        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new TestTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of TestType))
            MyBase.New(query)
        End Sub
        ''' <summary>
        ''' There are no comments for SingleType in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
        Public Overridable ReadOnly Property SingleType() As [Namespace].Foo.DSC.SingleTypeSingle
            Get
                If Not Me.IsComposable Then
                    Throw New Global.System.NotSupportedException("The previous function is not composable.")
                End If
                If (Me._SingleType Is Nothing) Then
                    Me._SingleType = New [Namespace].Foo.DSC.SingleTypeSingle(Me.Context, GetPath("singleType"))
                End If
                Return Me._SingleType
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _SingleType As [Namespace].Foo.DSC.SingleTypeSingle
    End Class
    ''' <summary>
    ''' There are no comments for TestType in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("testType")>  _
    Partial Public Class TestType
        Inherits BaseType
        ''' <summary>
        ''' Create a new TestType object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateTestType(ByVal keyProp As Integer) As TestType
            Dim testType As TestType = New TestType()
            testType.KeyProp = keyProp
            Return testType
        End Function
        ''' <summary>
        ''' There are no comments for Property SingleType in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
        Public Overridable Property SingleType() As [Namespace].Foo.DSC.SingleType
            Get
                Return Me._SingleType
            End Get
            Set
                Me.OnSingleTypeChanging(value)
                Me._SingleType = value
                Me.OnSingleTypeChanged
                Me.OnPropertyChanged("singleType")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _SingleType As [Namespace].Foo.DSC.SingleType
        Partial Private Sub OnSingleTypeChanging(ByVal value As [Namespace].Foo.DSC.SingleType)
        End Sub
        Partial Private Sub OnSingleTypeChanged()
        End Sub
    End Class
    ''' <summary>
    ''' There are no comments for SingleTypeSingle in the schema.
    ''' </summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singleTypeSingle")>  _
    Partial Public Class SingleTypeSingle
        Inherits Global.Microsoft.OData.Client.DataServiceQuerySingle(Of SingleType)
        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String)
            MyBase.New(context, path)
        End Sub

        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal context As Global.Microsoft.OData.Client.DataServiceContext, ByVal path As String, ByVal isComposable As Boolean)
            MyBase.New(context, path, isComposable)
        End Sub

        ''' <summary>
        ''' Initialize a new SingleTypeSingle object.
        ''' </summary>
        Public Sub New(ByVal query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of SingleType))
            MyBase.New(query)
        End Sub
        ''' <summary>
        ''' There are no comments for BaseSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseSet")>  _
        Public Overridable ReadOnly Property BaseSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType)
            Get
                If Not Me.IsComposable Then
                    Throw New Global.System.NotSupportedException("The previous function is not composable.")
                End If
                If (Me._BaseSet Is Nothing) Then
                    Me._BaseSet = Context.CreateQuery(Of [Namespace].Foo.DSC.TestType)(GetPath("baseSet"))
                End If
                Return Me._BaseSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _BaseSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType)
    End Class
    ''' <summary>
    ''' There are no comments for SingleType in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("keyProp")>  _
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singleType")>  _
    Partial Public Class SingleType
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        Implements Global.System.ComponentModel.INotifyPropertyChanged
        ''' <summary>
        ''' Create a new SingleType object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        ''' <param name="colorProp">Initial value of ColorProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Shared Function CreateSingleType(ByVal keyProp As Integer, ByVal colorProp As [Namespace].Foo.DSC.Color) As SingleType
            Dim singleType As SingleType = New SingleType()
            singleType.KeyProp = keyProp
            singleType.ColorProp = colorProp
            Return singleType
        End Function
        ''' <summary>
        ''' There are no comments for Property KeyProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("keyProp")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
                Me.OnPropertyChanged("keyProp")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property ColorProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("colorProp")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property ColorProp() As [Namespace].Foo.DSC.Color
            Get
                Return Me._ColorProp
            End Get
            Set
                Me.OnColorPropChanging(value)
                Me._ColorProp = value
                Me.OnColorPropChanged
                Me.OnPropertyChanged("colorProp")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _ColorProp As [Namespace].Foo.DSC.Color
        Partial Private Sub OnColorPropChanging(ByVal value As [Namespace].Foo.DSC.Color)
        End Sub
        Partial Private Sub OnColorPropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property BaseSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseSet")>  _
        Public Overridable Property BaseSet() As Global.Microsoft.OData.Client.DataServiceCollection(Of [Namespace].Foo.DSC.TestType)
            Get
                Return Me._BaseSet
            End Get
            Set
                Me.OnBaseSetChanging(value)
                Me._BaseSet = value
                Me.OnBaseSetChanged
                Me.OnPropertyChanged("baseSet")
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _BaseSet As Global.Microsoft.OData.Client.DataServiceCollection(Of [Namespace].Foo.DSC.TestType) = New Global.Microsoft.OData.Client.DataServiceCollection(Of [Namespace].Foo.DSC.TestType)(Nothing, Global.Microsoft.OData.Client.TrackingMode.None)
        Partial Private Sub OnBaseSetChanging(ByVal value As Global.Microsoft.OData.Client.DataServiceCollection(Of [Namespace].Foo.DSC.TestType))
        End Sub
        Partial Private Sub OnBaseSetChanged()
        End Sub
        ''' <summary>
        ''' This event is raised when the value of the property is changed
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Event PropertyChanged As Global.System.ComponentModel.PropertyChangedEventHandler Implements Global.System.ComponentModel.INotifyPropertyChanged.PropertyChanged
        ''' <summary>
        ''' The value of the property is changed
        ''' </summary>
        ''' <param name="property">property name</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Overridable Sub OnPropertyChanged(ByVal [property] As String)
            If (Not (Me.PropertyChangedEvent) Is Nothing) Then
                RaiseEvent PropertyChanged(Me, New Global.System.ComponentModel.PropertyChangedEventArgs([property]))
            End If
        End Sub
        ''' <summary>
        ''' There are no comments for Foo7 in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo7")>  _
        Public Overridable Function Foo7() As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))
            Dim resource As Global.Microsoft.OData.Client.EntityDescriptor = Context.EntityTracker.TryGetEntityDescriptor(Me)
            If resource Is Nothing Then
                Throw New Global.System.Exception("cannot find entity")
            End If

            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))(Me.Context, resource.EditLink.OriginalString.Trim("/"C) + "/namespace.foo.DSC.foo7")
        End Function
    End Class
        ''' <summary>
        ''' There are no comments for Color in the schema.
        ''' </summary>
    <Global.System.Flags()>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("color")>  _
    Public Enum Color
        <Global.Microsoft.OData.Client.OriginalNameAttribute("red")>  _
        Red = 0
        <Global.Microsoft.OData.Client.OriginalNameAttribute("white")>  _
        White = 1
        <Global.Microsoft.OData.Client.OriginalNameAttribute("blue")>  _
        Blue = 2
    End Enum
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.BaseType as [Namespace].Foo.DSC.BaseTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.BaseType), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As [Namespace].Foo.DSC.BaseTypeSingle
            Return New [Namespace].Foo.DSC.BaseTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.BaseType as [Namespace].Foo.DSC.BaseTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.BaseType),
            keyProp As Integer) As [Namespace].Foo.DSC.BaseTypeSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.DSC.BaseTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.TestType as [Namespace].Foo.DSC.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As [Namespace].Foo.DSC.TestTypeSingle
            Return New [Namespace].Foo.DSC.TestTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.TestType as [Namespace].Foo.DSC.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType),
            keyProp As Integer) As [Namespace].Foo.DSC.TestTypeSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.DSC.TestTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Cast an entity of type [Namespace].Foo.DSC.BaseType to its derived type [Namespace].Foo.DSC.TestType
        ''' </summary>
        ''' <param name="_source">source entity</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function CastToTestType(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.DSC.BaseType)) As [Namespace].Foo.DSC.TestTypeSingle
            Dim query As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.DSC.TestType) = _source.CastTo(Of [Namespace].Foo.DSC.TestType)()
            Return New [Namespace].Foo.DSC.TestTypeSingle(_source.Context, query.GetPath(Nothing))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.SingleType as [Namespace].Foo.DSC.SingleTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.SingleType), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As [Namespace].Foo.DSC.SingleTypeSingle
            Return New [Namespace].Foo.DSC.SingleTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type [Namespace].Foo.DSC.SingleType as [Namespace].Foo.DSC.SingleTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.SingleType),
            keyProp As Integer) As [Namespace].Foo.DSC.SingleTypeSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "keyProp", keyProp }
            }
            Return New [Namespace].Foo.DSC.SingleTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' There are no comments for Foo7 in the schema.
        ''' </summary>
        <Global.System.Runtime.CompilerServices.Extension()>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo7")>  _
        Public Function Foo7(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of [Namespace].Foo.DSC.SingleType)) As Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))
            If Not _source.IsComposable Then
                Throw New Global.System.NotSupportedException("The previous function is not composable.")
            End If
            Return New Global.Microsoft.OData.Client.DataServiceActionQuerySingle(Of Global.System.Nullable(Of Integer))(_source.Context, _source.AppendRequestUri("namespace.foo.DSC.foo7"))
        End Function
    End Module
End Namespace
Namespace [Namespace].Bar.DSC
    ''' <summary>
    ''' There are no comments for SingletonContainer in the schema.
    ''' </summary>
    <Global.Microsoft.OData.Client.OriginalNameAttribute("singletonContainer")>  _
    Partial Public Class SingletonContainer
        Inherits Global.Microsoft.OData.Client.DataServiceContext
        ''' <summary>
        ''' Initialize a new SingletonContainer object.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Sub New(ByVal serviceRoot As Global.System.Uri)
            Me.New(serviceRoot, Global.Microsoft.OData.Client.ODataProtocolVersion.V4)
        End Sub
        ''' <summary>
        ''' Initialize a new SingletonContainer object.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Sub New(ByVal serviceRoot As Global.System.Uri, ByVal protocolVersion As Global.Microsoft.OData.Client.ODataProtocolVersion)
            MyBase.New(serviceRoot, protocolVersion)
            Me.ResolveName = AddressOf Me.ResolveNameFromType
            Me.ResolveType = AddressOf Me.ResolveTypeFromName
            Me.OnContextCreated
            Me.Format.LoadServiceModel = AddressOf GeneratedEdmModel.GetInstance
            Me.Format.UseJson()
        End Sub
        Partial Private Sub OnContextCreated()
        End Sub
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private Shared ROOTNAMESPACE As String = GetType(SingletonContainer).Namespace.Remove(GetType(SingletonContainer).Namespace.LastIndexOf("Namespace.Bar.DSC"))
        ''' <summary>
        ''' Since the namespace configured for this service reference
        ''' in Visual Studio is different from the one indicated in the
        ''' server schema, use type-mappers to map between the two.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Function ResolveTypeFromName(ByVal typeName As String) As Global.System.Type
            Dim resolvedType As Global.System.Type = Me.DefaultResolveType(typeName, "namespace.bar.DSC", String.Concat(ROOTNAMESPACE, "Namespace.Bar.DSC"))
            If (Not (resolvedType) Is Nothing) Then
                Return resolvedType
            End If
            resolvedType = Me.DefaultResolveType(typeName, "namespace.foo.DSC", String.Concat(ROOTNAMESPACE, "Namespace.Foo.DSC"))
            If (Not (resolvedType) Is Nothing) Then
                Return resolvedType
            End If
            Return Nothing
        End Function
        ''' <summary>
        ''' Since the namespace configured for this service reference
        ''' in Visual Studio is different from the one indicated in the
        ''' server schema, use type-mappers to map between the two.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Protected Function ResolveNameFromType(ByVal clientType As Global.System.Type) As String
            Dim originalNameAttribute As Global.Microsoft.OData.Client.OriginalNameAttribute =
                CType(Global.System.Linq.Enumerable.SingleOrDefault(Global.Microsoft.OData.Client.Utility.GetCustomAttributes(clientType, GetType(Global.Microsoft.OData.Client.OriginalNameAttribute), true)), Global.Microsoft.OData.Client.OriginalNameAttribute)
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "Namespace.Bar.DSC"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                If (Not (originalNameAttribute) Is Nothing) Then
                    Return String.Concat("namespace.bar.DSC.", originalNameAttribute.OriginalName)
                End If
                Return String.Concat("namespace.bar.DSC.", clientType.Name)
            End If
            If clientType.Namespace.Equals(String.Concat(ROOTNAMESPACE, "Namespace.Foo.DSC"), Global.System.StringComparison.OrdinalIgnoreCase) Then
                If (Not (originalNameAttribute) Is Nothing) Then
                    Return String.Concat("namespace.foo.DSC.", originalNameAttribute.OriginalName)
                End If
                Return String.Concat("namespace.foo.DSC.", clientType.Name)
            End If
            If (Not (originalNameAttribute) Is Nothing) Then
                Dim fullName As String = clientType.FullName.Substring(ROOTNAMESPACE.Length)
                Return fullName.Remove(fullName.LastIndexOf(clientType.Name)) + originalNameAttribute.OriginalName
            End If
            Return clientType.FullName.Substring(ROOTNAMESPACE.Length)
        End Function
        ''' <summary>
        ''' There are no comments for TestTypeSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("testTypeSet")>  _
        Public Overridable ReadOnly Property TestTypeSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType)
            Get
                If (Me._TestTypeSet Is Nothing) Then
                    Me._TestTypeSet = MyBase.CreateQuery(Of [Namespace].Foo.DSC.TestType)("testTypeSet")
                End If
                Return Me._TestTypeSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _TestTypeSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.TestType)
        ''' <summary>
        ''' There are no comments for BaseTypeSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("baseTypeSet")>  _
        Public Overridable ReadOnly Property BaseTypeSet() As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.BaseType)
            Get
                If (Me._BaseTypeSet Is Nothing) Then
                    Me._BaseTypeSet = MyBase.CreateQuery(Of [Namespace].Foo.DSC.BaseType)("baseTypeSet")
                End If
                Return Me._BaseTypeSet
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _BaseTypeSet As Global.Microsoft.OData.Client.DataServiceQuery(Of [Namespace].Foo.DSC.BaseType)
        ''' <summary>
        ''' There are no comments for TestTypeSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddToTestTypeSet(ByVal testType As [Namespace].Foo.DSC.TestType)
            MyBase.AddObject("testTypeSet", testType)
        End Sub
        ''' <summary>
        ''' There are no comments for BaseTypeSet in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Public Overridable Sub AddToBaseTypeSet(ByVal baseType As [Namespace].Foo.DSC.BaseType)
            MyBase.AddObject("baseTypeSet", baseType)
        End Sub
        ''' <summary>
        ''' There are no comments for SuperType in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("superType")>  _
        Public Overridable ReadOnly Property SuperType() As [Namespace].Foo.DSC.TestTypeSingle
            Get
                If (Me._SuperType Is Nothing) Then
                    Me._SuperType = New [Namespace].Foo.DSC.TestTypeSingle(Me, "superType")
                End If
                Return Me._SuperType
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _SuperType As [Namespace].Foo.DSC.TestTypeSingle
        ''' <summary>
        ''' There are no comments for Single in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        <Global.Microsoft.OData.Client.OriginalNameAttribute("single")>  _
        Public Overridable ReadOnly Property [Single]() As [Namespace].Foo.DSC.SingleTypeSingle
            Get
                If (Me._Single Is Nothing) Then
                    Me._Single = New [Namespace].Foo.DSC.SingleTypeSingle(Me, "single")
                End If
                Return Me._Single
            End Get
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private _Single As [Namespace].Foo.DSC.SingleTypeSingle
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
        Private MustInherit Class GeneratedEdmModel
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared ParsedModel As Global.Microsoft.OData.Edm.IEdmModel = LoadModelFromString
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Const Edmx As String = "<edmx:Edmx Version=""4.0"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">" & _
 "  <edmx:DataServices>" & _
 "    <Schema Namespace=""namespace.foo.DSC"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityType Name=""baseType"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""keyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""keyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""testType"" BaseType=""namespace.foo.DSC.baseType"">" & _
 "        <NavigationProperty Name=""singleType"" Type=""namespace.foo.DSC.singleType"" />" & _
 "      </EntityType>" & _
 "      <EntityType Name=""singleType"">" & _
 "        <Key>" & _
 "          <PropertyRef Name=""keyProp"" />" & _
 "        </Key>" & _
 "        <Property Name=""keyProp"" Type=""Edm.Int32"" Nullable=""false"" />" & _
 "        <Property Name=""colorProp"" Type=""namespace.foo.DSC.color"" Nullable=""false"" />" & _
 "        <NavigationProperty Name=""baseSet"" Type=""Collection(namespace.foo.DSC.testType)"" />" & _
 "      </EntityType>" & _
 "      <EnumType Name=""color"" UnderlyingType=""Edm.Int32"" IsFlags=""true"">" & _
 "        <Member Name=""red"" />" & _
 "        <Member Name=""white"" />" & _
 "        <Member Name=""blue"" />" & _
 "      </EnumType>" & _
 "      <Function Name=""foo6"">" & _
 "        <Parameter Name=""p1"" Type=""Collection(namespace.foo.DSC.testType)"" />" & _
 "        <ReturnType Type=""Edm.String"" />" & _
 "      </Function>" & _
 "      <Action Name=""foo7"" IsBound=""True"">" & _
 "        <Parameter Name=""p1"" Type=""namespace.foo.DSC.singleType"" />" & _
 "        <ReturnType Type=""Edm.Int32"" />" & _
 "      </Action>" & _
 "    </Schema>" & _
 "    <Schema Namespace=""namespace.bar.DSC"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">" & _
 "      <EntityContainer Name=""singletonContainer"">" & _
 "        <EntitySet Name=""testTypeSet"" EntityType=""namespace.foo.DSC.testType"" />" & _
 "        <EntitySet Name=""baseTypeSet"" EntityType=""namespace.foo.DSC.baseType"" />" & _
 "        <Singleton Name=""superType"" Type=""namespace.foo.DSC.testType"" />" & _
 "        <Singleton Name=""single"" Type=""namespace.foo.DSC.singleType"" />" & _
 "        <FunctionImport Name=""foo6"" Function=""namespace.foo.DSC.foo6"" />" & _
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
                    Dim errors As Global.System.Collections.Generic.IEnumerable(Of Global.Microsoft.OData.Edm.Validation.EdmError) = Nothing
                    Dim edmModel As Global.Microsoft.OData.Edm.IEdmModel = Nothing
                    If Not Global.Microsoft.OData.Edm.Csdl.CsdlReader.TryParse(reader, False, edmModel, errors) Then
                        Dim errorMessages As Global.System.Text.StringBuilder = New Global.System.Text.StringBuilder()
                        For Each err As Global.Microsoft.OData.Edm.Validation.EdmError In errors
                            errorMessages.Append(err.ErrorMessage)
                            errorMessages.Append("; ")
                        Next
                        Throw New Global.System.InvalidOperationException(errorMessages.ToString())
                    End If

                    Return edmModel
                Finally
                    CType(reader, Global.System.IDisposable).Dispose()
                End Try
            End Function
            <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "2.4.0")>  _
            Private Shared Function CreateXmlReader(ByVal edmxToParse As String) As Global.System.Xml.XmlReader
                Return Global.System.Xml.XmlReader.Create(New Global.System.IO.StringReader(edmxToParse))
            End Function
        End Class
        ''' <summary>
        ''' There are no comments for Foo6 in the schema.
        ''' </summary>
        <Global.Microsoft.OData.Client.OriginalNameAttribute("foo6")>  _
        Public Overridable Function Foo6(p1 As Global.System.Collections.Generic.ICollection(Of [Namespace].Foo.DSC.TestType), Optional ByVal useEntityReference As Boolean = False) As Global.Microsoft.OData.Client.DataServiceQuerySingle(Of String)
            Return Me.CreateFunctionQuerySingle(Of String)("", "/foo6", False, New Global.Microsoft.OData.Client.UriEntityOperationParameter("p1", p1, useEntityReference))
        End Function
    End Class
End Namespace
