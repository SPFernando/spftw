using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using System.Xml;
using System.IO;
using Microsoft.SharePoint.Administration;
using Nintex.Workflow.Common;
using System.Collections.ObjectModel;
using Nintex.Workflow;
using Nintex.Workflow.Administration;
using System.Reflection;

namespace NintexAddons.Features.NintexAddonsFeature
{
    /// <summary>
    /// Esta clase controla los eventos generados durante la activación, desactivación, instalación, desinstalación y actualización de características.
    /// </summary>
    /// <remarks>
    /// El GUID asociado a esta clase se puede usar durante el empaquetado y no se debe modificar.
    /// </remarks>

    [Guid("c1d44865-1576-467d-bb7a-41173d8b18f8")]
    public class NintexAddonsFeatureEventReceiver : SPFeatureReceiver
    {

        #region Override

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parent = (SPWebApplication)properties.Feature.Parent;
            AddCustomAction(parent, properties, @"CustomNWA\LaunchPowerShell.nwa");
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication parent = (SPWebApplication)properties.Feature.Parent;
            RemoveCustomAction(parent, properties, "NintexAddons.Code.LaunchPowerShellAdapter", Assembly.GetExecutingAssembly().FullName);
        }

        #endregion

        #region Protected

        protected void AddCustomAction(SPWebApplication parent, SPFeatureReceiverProperties properties, string pathToNWAFile)
        {
            XmlDocument nwaXml = GetNWADefinition(properties, pathToNWAFile);
            ActivityReference newActivityReference = ActivityReference.ReadFromNWA(nwaXml);
            ActivityReference action = ActivityReferenceCollection.FindByAdapter(newActivityReference.AdapterType, newActivityReference.AdapterAssembly);

            if (action != null)
            {
                ActivityReferenceCollection.UpdateActivity(action.ActivityId, newActivityReference.Name,
                                                            newActivityReference.Description, newActivityReference.Category,
                                                            newActivityReference.ActivityAssembly, newActivityReference.ActivityType,
                                                            newActivityReference.AdapterAssembly, newActivityReference.AdapterType,
                                                            newActivityReference.HandlerUrl, newActivityReference.ConfigPage,
                                                            newActivityReference.RenderBehaviour, newActivityReference.Icon, newActivityReference.ToolboxIcon,
                                                            newActivityReference.WarningIcon, newActivityReference.QuickAccess,
                                                            newActivityReference.ListTypeFilter);
            }
            else
            {
                ActivityReferenceCollection.AddActivity(newActivityReference.Name, newActivityReference.Description,
                                                        newActivityReference.Category, newActivityReference.ActivityAssembly,
                                                        newActivityReference.ActivityType, newActivityReference.AdapterAssembly,
                                                        newActivityReference.AdapterType, newActivityReference.HandlerUrl, newActivityReference.ConfigPage,
                                                        newActivityReference.RenderBehaviour, newActivityReference.Icon, newActivityReference.ToolboxIcon,
                                                        newActivityReference.WarningIcon, newActivityReference.QuickAccess,
                                                        newActivityReference.ListTypeFilter);
                action = ActivityReferenceCollection.FindByAdapter(newActivityReference.AdapterType,
                                                                    newActivityReference.AdapterAssembly);
            }

            // Escribir en web.config en el apartado AuthorizedTypes.
            string activityTypeName = string.Empty;
            string activityNamespace = string.Empty;
            Utility.ExtractNamespaceAndClassName(action.ActivityType, out activityTypeName, out activityNamespace);
            AuthorisedTypes.InstallAuthorizedWorkflowTypes(parent, action.ActivityAssembly, activityNamespace, activityTypeName);

            ActivityActivationReference reference = new ActivityActivationReference(action.ActivityId, Guid.Empty, Guid.Empty);
            reference.AddOrUpdateActivationReference();

            // Escribir en los SafeControls para el Namespace del Activity y Adapter
            SPWebConfigModification mod = new SPWebConfigModification(@"SafeControl[@Assembly='NintexAddons, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7f6b0f58b76e9e85'][@Namespace='NintexAddons.Code'][@TypeName='*'][@Safe='True'][@AllowRemoteDesigner='True']", "/configuration/SharePoint/SafeControls");
            parent.WebConfigModifications.Add(mod);
            parent.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
        }
        
        protected void RemoveCustomAction(SPWebApplication parent, SPFeatureReceiverProperties properties, string adapterType, string adapterAssembly)
        {
            ActivityReference action = ActivityReferenceCollection.FindByAdapter(adapterType, adapterAssembly);
            
            if (action != null)
            {                
                if (!IsFeatureActivatedInAnyWebApp(parent, properties.Definition.Id))
                {
                    ActivityReferenceCollection.RemoveAction(action.ActivityId);
                }

                string activityTypeName = string.Empty;
                string activityNamespace = string.Empty;
                Utility.ExtractNamespaceAndClassName(action.ActivityType, out activityTypeName, out activityNamespace);
                
                Collection<SPWebConfigModification> modifications = parent.WebConfigModifications;
                foreach (SPWebConfigModification modification in modifications)
                {
                    if (modification.Owner == AuthorisedTypes.OWNER_TOKEN)
                    {
                        if (IsAuthorizedTypeMatch(modification.Value, action.ActivityAssembly, activityTypeName, activityNamespace))
                        {
                            modifications.Remove(modification);
                            parent.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                            break;
                        }
                    }
                }

            }
        }

        private bool IsAuthorizedTypeMatch(string modification, string activityAssembly, string activityType, string activityNamespace)
        {
            bool result = false;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(modification);
            
            if (doc.FirstChild.Name == "authorizedType")
            {
                result = (doc.SelectSingleNode("//@TypeName").Value == activityType
                        && doc.SelectSingleNode("//@Namespace").Value == activityNamespace
                        && doc.SelectSingleNode("//@Assembly").Value == activityAssembly);

            }

            return result;
        }

        private bool IsFeatureActivatedInAnyWebApp(SPWebApplication thisWebApplication, Guid thisFeatureId)
        {
            bool result = false;
            
            SPWebService webService = SPWebService.ContentService;
            if (webService == null)
            {
                throw new ApplicationException("No se pudo acceder al ContentService.");
            }

            SPWebApplicationCollection webApps = webService.WebApplications;
            foreach (SPWebApplication webApp in webApps)
            {
                if (webApp != thisWebApplication)
                {
                    if (webApp.Features[thisFeatureId] != null)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        private XmlDocument GetNWADefinition(SPFeatureReceiverProperties properties, string pathToNWAFile)
        {
            using (Stream stream = properties.Definition.GetFile(pathToNWAFile))
            {
                XmlDocument nwaXml = new XmlDocument();
                nwaXml.Load(stream);
                return nwaXml;
            }
        }

        #endregion
    }
}
