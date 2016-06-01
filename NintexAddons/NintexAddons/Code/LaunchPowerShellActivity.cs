using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using Nintex.Workflow;
using Nintex.Workflow.Activities;
using System;
using System.Management;
using System.Collections.ObjectModel;
using System.Workflow.ComponentModel;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using Microsoft.SharePoint;

namespace NintexAddons.Code
{
    public class LaunchPowerShellActivity : ProgressTrackingActivity
    {

        #region Dependencies

        public static DependencyProperty __ListItemProperty = DependencyProperty.Register("__ListItem", typeof (SPItemKey), typeof (LaunchPowerShellActivity));
        public static DependencyProperty __ContextProperty = DependencyProperty.Register("__Context", typeof (WorkflowContext), typeof (LaunchPowerShellActivity));
        public static DependencyProperty __ListIdProperty = DependencyProperty.Register("__ListId", typeof (string),typeof (LaunchPowerShellActivity));
        public static DependencyProperty WebProperty = DependencyProperty.Register("Web", typeof(string), typeof(LaunchPowerShellActivity));
        public static DependencyProperty PropertyProperty = DependencyProperty.Register("Property", typeof(string), typeof(LaunchPowerShellActivity));
        public static DependencyProperty ScriptPropertyProperty = DependencyProperty.Register("ScriptProperty", typeof(string), typeof(LaunchPowerShellActivity));
        
        #endregion

        #region Properties

        public WorkflowContext __Context
        {
            get { return (WorkflowContext) base.GetValue(__ContextProperty); }
            set { base.SetValue(__ContextProperty, value); }
        }

        public SPItemKey __ListItem
        {
            get { return (SPItemKey) base.GetValue(__ListItemProperty); }
            set { base.SetValue(__ListItemProperty, value); }
        }

        public string __ListId
        {
            get { return (string) base.GetValue(__ListIdProperty); }
            set { base.SetValue(__ListIdProperty, value);}
        }

        public string Web
        {
            get { return (string) base.GetValue(WebProperty); }
            set { base.SetValue(WebProperty, value);}
        }

        public string Property
        {
            get { return (string) base.GetValue(PropertyProperty); }
            set { base.SetValue(PropertyProperty, value);}
        }

        public string PropertyScript
        {
            get { return (string)base.GetValue(ScriptPropertyProperty); }
            set { base.SetValue(ScriptPropertyProperty, value); }
        }

        #endregion

        #region Constructor

        public LaunchPowerShellActivity()
        {

        }

        #endregion

        #region Override

        protected override ActivityExecutionStatus Execute(ActivityExecutionContext executionContext)
        {            
            ActivityActivationReference.IsAllowed(this, __Context.Web);
            NWWorkflowContext ctx = NWWorkflowContext.GetContext(this.__Context, new Guid(this.__ListId), this.__ListItem.Id,  this.WorkflowInstanceId, this);
            base.LogProgressStart(ctx);

            string resolvedProperty = ctx.AddContextDataToString(this.Property);
            string resolvedPropertyScript = ctx.AddContextDataToString(this.PropertyScript);
            
            Guid idSite = ctx.Web.Site.ID;
            Guid idWeb = ctx.Web.ID;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite elevatedSite = new SPSite(idSite))
                {
                    using (SPWeb elevatedWeb = elevatedSite.OpenWeb(idWeb))
                    {
                        elevatedWeb.AllowUnsafeUpdates = true;
                        try
                        {
                            // Ejecución del PowerShell
                            ExecutePowerShell(resolvedProperty, resolvedPropertyScript);
                        }
                        finally
                        {
                            elevatedWeb.AllowUnsafeUpdates = false;
                        }
                    }
                }                
            });

            base.LogProgressEnd(ctx, executionContext);
            return ActivityExecutionStatus.Closed;
        }

        protected override ActivityExecutionStatus HandleFault(ActivityExecutionContext executionContext, Exception exception)
        {
            Nintex.Workflow.Diagnostics.ActivityErrorHandler.HandleFault(executionContext, exception, this.WorkflowInstanceId, "Error ejecutando PowerShell", __ListItem.Id, __ListId, __Context);
            return base.HandleFault(executionContext, exception);
        }

        #endregion

        #region Private

        private void ExecutePowerShell(string script, string pathToPs1)
        {
            if (!string.IsNullOrEmpty(script))
            {
                RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

                using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
                {
                    runspace.Open();
                    Pipeline pipeline = runspace.CreatePipeline(script);
                    Collection<PSObject> psObjects;
                    psObjects = pipeline.Invoke();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(pathToPs1))
                {
                    RunspaceConfiguration runspaceConfiguration = RunspaceConfiguration.Create();

                    using (Runspace runspace = RunspaceFactory.CreateRunspace(runspaceConfiguration))
                    {
                        runspace.Open();
                        Pipeline pipeline = runspace.CreatePipeline();
                        Command scriptCommand = new Command(pathToPs1);
                        pipeline.Commands.Add(scriptCommand);
                        Collection<PSObject> psObjects;
                        psObjects = pipeline.Invoke();
                    }
                }
            }
        }

        #endregion

    }
}
