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


'Generation date: 25.05.2021 14:23:22
Namespace Simple
    ''' <summary>
    ''' There are no comments for TestTypeSingle in the schema.
    ''' </summary>
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
    End Class
    ''' <summary>
    ''' There are no comments for TestType in the schema.
    ''' </summary>
    ''' <KeyProperties>
    ''' KeyProp
    ''' </KeyProperties>
    <Global.Microsoft.OData.Client.Key("KeyProp")>  _
    Partial Public Class TestType
        Inherits Global.Microsoft.OData.Client.BaseEntityType
        ''' <summary>
        ''' Create a new TestType object.
        ''' </summary>
        ''' <param name="keyProp">Initial value of KeyProp.</param>
        ''' <param name="valueProp">Initial value of ValueProp.</param>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")>  _
        Public Shared Function CreateTestType(ByVal keyProp As Integer, ByVal valueProp As String) As TestType
            Dim testType As TestType = New TestType()
            testType.KeyProp = keyProp
            testType.ValueProp = valueProp
            Return testType
        End Function
        ''' <summary>
        ''' There are no comments for Property KeyProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property KeyProp() As Integer
            Get
                Return Me._KeyProp
            End Get
            Set
                Me.OnKeyPropChanging(value)
                Me._KeyProp = value
                Me.OnKeyPropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")>  _
        Private _KeyProp As Integer
        Partial Private Sub OnKeyPropChanging(ByVal value As Integer)
        End Sub
        Partial Private Sub OnKeyPropChanged()
        End Sub
        ''' <summary>
        ''' There are no comments for Property ValueProp in the schema.
        ''' </summary>
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")>  _
        <Global.System.ComponentModel.DataAnnotations.StringLengthAttribute(25)>  _
        <Global.System.ComponentModel.DataAnnotations.RequiredAttribute()>  _
        Public Overridable Property ValueProp() As String
            Get
                Return Me._ValueProp
            End Get
            Set
                Me.OnValuePropChanging(value)
                Me._ValueProp = value
                Me.OnValuePropChanged
            End Set
        End Property
        <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.OData.Client.Design.T4", "#VersionNumber#")>  _
        Private _ValueProp As String
        Partial Private Sub OnValuePropChanging(ByVal value As String)
        End Sub
        Partial Private Sub OnValuePropChanged()
        End Sub
    End Class
    ''' <summary>
    ''' Class containing all extension methods
    ''' </summary>
    Public Module ExtensionMethods
        ''' <summary>
        ''' Get an entity of type Simple.TestType as Simple.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="_keys">dictionary with the names and values of keys</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Simple.TestType), ByVal _keys As Global.System.Collections.Generic.IDictionary(Of String, Object)) As Simple.TestTypeSingle
            Return New Simple.TestTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
        ''' <summary>
        ''' Get an entity of type Simple.TestType as Simple.TestTypeSingle specified by key from an entity set
        ''' </summary>
        ''' <param name="_source">source entity set</param>
        ''' <param name="keyProp">The value of keyProp</param>
        <Global.System.Runtime.CompilerServices.Extension()>
        Public Function ByKey(ByVal _source As Global.Microsoft.OData.Client.DataServiceQuery(Of Simple.TestType),
            keyProp As Integer) As Simple.TestTypeSingle
            Dim _keys As Global.System.Collections.Generic.IDictionary(Of String, Object) = New Global.System.Collections.Generic.Dictionary(Of String, Object)() From
            {
                { "KeyProp", keyProp }
            }
            Return New Simple.TestTypeSingle(_source.Context, _source.GetKeyPath(Global.Microsoft.OData.Client.Serializer.GetKeyString(_source.Context, _keys)))
        End Function
    End Module
End Namespace
