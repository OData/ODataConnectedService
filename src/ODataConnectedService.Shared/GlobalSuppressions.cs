//---------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.ConnectedService.Views")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.OData.ConnectedService.Converters")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.OData.ConnectedService.ViewModels.ConfigODataEndpointViewModel.#OnPageLeavingAsync(Microsoft.VisualStudio.ConnectedServices.WizardLeavingArgs)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Microsoft.OData.ConnectedService.Common.UserSettingsPersistenceHelper.#ExecuteNoncriticalOperation(System.Action,Microsoft.VisualStudio.ConnectedServices.ConnectedServiceLogger,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "VsHierarchy", Scope = "member", Target = "Microsoft.OData.ConnectedService.Common.ProjectHelper.#GetProjectFromHierarchy(Microsoft.VisualStudio.Shell.Interop.IVsHierarchy)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.OData.ConnectedService.ViewModels.ConfigODataEndpointViewModel.#UserSettings")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Scope = "type", Target = "Microsoft.OData.ConnectedService.ODataConnectedServiceHandler")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.OData.ConnectedService.ODataConnectedServiceProvider.#CreateConfiguratorAsync(Microsoft.VisualStudio.ConnectedServices.ConnectedServiceProviderContext)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly", Scope = "member", Target = "Microsoft.OData.ConnectedService.ViewModels.ConfigODataEndpointViewModel.#GetMetadata(System.Version&)")]