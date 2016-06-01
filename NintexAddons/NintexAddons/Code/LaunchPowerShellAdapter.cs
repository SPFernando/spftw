using Microsoft.SharePoint;
using Nintex.Workflow;
using Nintex.Workflow.Activities.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.ComponentModel;

namespace NintexAddons.Code
{
    public class LaunchPowerShellAdapter : GenericRenderingAction
    {

        #region Constants

        private const string PROP_NAME = "Property";
        private const string PROP_SCRIPT_NAME = "ScriptProperty";

        #endregion

        #region Override

        public override NWActionConfig GetDefaultConfig(GetDefaultConfigContext context)
        {
            NWActionConfig config = new NWActionConfig(this);

            // Creación de los 2 parámetros que se van a utilizar. El de el Path, y el de insertar el Script directamente.
            config.Parameters = new ActivityParameter[2];

            config.Parameters[0] = new ActivityParameter();
            config.Parameters[0].Name = PROP_NAME;
            config.Parameters[0].PrimitiveValue = new PrimitiveValue();
            config.Parameters[0].PrimitiveValue.Value = string.Empty;
            config.Parameters[0].PrimitiveValue.ValueType = SPFieldType.Text.ToString();

            config.Parameters[1] = new ActivityParameter();
            config.Parameters[1].Name = PROP_SCRIPT_NAME;
            config.Parameters[1].PrimitiveValue = new PrimitiveValue();
            config.Parameters[1].PrimitiveValue.Value = string.Empty;
            config.Parameters[1].PrimitiveValue.ValueType = SPFieldType.Note.ToString();

            config.TLabel = ActivityReferenceCollection.FindByAdapter(this).Name;
            return config;

        }

        public override bool ValidateConfig(ActivityContext context)
        {
            bool isValid = true;
            Dictionary<string, ActivityParameterHelper> parameters = context.Configuration.GetParameterHelpers();

            // Uno de los dos campos (Text o Note) debe ir relleno.
            if (!parameters[PROP_NAME].Validate(typeof(string), context) && !parameters[PROP_SCRIPT_NAME].Validate(typeof(string), context))
            {
                isValid &= false;
                validationSummary.AddError("Script o Código PowerShell", ValidationSummaryErrorType.CannotBeBlank);
            }
            return isValid;
        }

        public override CompositeActivity AddActivityToWorkflow(PublishContext context)
        {
            Dictionary<string, ActivityParameterHelper> parameters = context.Config.GetParameterHelpers();

            LaunchPowerShellActivity activity = new LaunchPowerShellActivity();
            parameters[PROP_NAME].AssignTo(activity, LaunchPowerShellActivity.PropertyProperty, context);
            parameters[PROP_SCRIPT_NAME].AssignTo(activity, LaunchPowerShellActivity.ScriptPropertyProperty, context);

            activity.SetBinding(LaunchPowerShellActivity.__ContextProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__context));
            activity.SetBinding(LaunchPowerShellActivity.__ListItemProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__item));
            activity.SetBinding(LaunchPowerShellActivity.__ListIdProperty, new ActivityBind(context.ParentWorkflow.Name, StandardWorkflowDataItems.__list));

            ActivityFlags activityFlags = new ActivityFlags();
            activityFlags.AddLabelsFromConfig(context);
            activityFlags.AssignTo(activity);

            context.ParentActivity.Activities.Add(activity);
            return null;
        }

        public override NWActionConfig GetConfig(RetrieveConfigContext context)
        {
            NWActionConfig config = this.GetDefaultConfig(context);

            Dictionary<string, ActivityParameterHelper> parameters = config.GetParameterHelpers();
            parameters[PROP_NAME].RetrieveValue(context.Activity, LaunchPowerShellActivity.PropertyProperty, context);
            parameters[PROP_SCRIPT_NAME].RetrieveValue(context.Activity, LaunchPowerShellActivity.ScriptPropertyProperty, context);
            return config;
        }

        public override ActionSummary BuildSummary(ActivityContext context)
        {
            ActionSummary result = null;
            Dictionary<string, ActivityParameterHelper> parameters = context.Configuration.GetParameterHelpers();
            
            // ActionSummary cuando se vea el Detalle del Histórico del WF.
            if (!string.IsNullOrEmpty(parameters[PROP_SCRIPT_NAME].Value))
            {
                result = new ActionSummary("Lanzado Script PowerShell: {0}", parameters[PROP_SCRIPT_NAME].Value);
            }
            if (!string.IsNullOrEmpty(parameters[PROP_NAME].Value))
            {
                result = new ActionSummary("Lanzado Código PowerShell: {0}", parameters[PROP_NAME].Value);
            }
            return result;
        }

        #endregion

    }
}
