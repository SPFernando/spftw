<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LaunchPowerShellDialog.aspx.cs" Inherits="NintexAddons.Layouts.NintexWorkflow.NintexAddons.LaunchPowerShellDialog.LaunchPowerShellDialog, $SharePoint.Project.AssemblyFullName$" DynamicMasterPageFile="~masterurl/default.master" %>

<%@ Register TagPrefix="Nintex" Namespace="Nintex.Workflow.ServerControls" Assembly="Nintex.Workflow.ServerControls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=913f6bae0ca5ae12" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationPropertySection" src="~/_layouts/NintexWorkflow/ConfigurationPropertySection.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="ConfigurationProperty" src="~/_layouts/NintexWorkflow/ConfigurationProperty.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogLoad" Src="~/_layouts/NintexWorkflow/DialogLoad.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="DialogBody" Src="~/_layouts/NintexWorkflow/DialogBody.ascx" %>

<%@ Register TagPrefix="Nintex" TagName="SingleLineInput" Src="~/_layouts/NintexWorkflow/SingleLineInput.ascx" %>
<%@ Register TagPrefix="Nintex" TagName="PlainTextWebControl" Src="~/_layouts/NintexWorkflow/PlainTextWebControl.ascx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <Nintex:DialogLoad runat="server" />

    <script type="text/javascript" language="javascript">
        function TPARetrieveConfig() {
            setRTEValue('<%=sliProperty.ClientID%>', configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='ScriptProperty']/PrimitiveValue/@Value").text);            
            setPlainTextEditorText('<%=ptwbProperty.ClientID%>', configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='Property']/PrimitiveValue/@Value").text);            
        }

        function TPAWriteConfig() {
            configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='ScriptProperty']/PrimitiveValue/@Value").text = getRTEValue('<%=sliProperty.ClientID%>');
            configXml.selectSingleNode("/NWActionConfig/Parameters/Parameter[@Name='Property']/PrimitiveValue/@Value").text = getStringFromPlainTextEditor('<%=ptwbProperty.ClientID%>');
            return true;
        }

        onLoadFunctions[onLoadFunctions.length] = function () {
            dialogSectionsArray["<%= MainControls1.ClientID %>"] = true;
        };
    </script>
</asp:Content>

<asp:Content ID="ContentBody" ContentPlaceHolderID="PlaceHolderMain" runat="Server">

  <Nintex:ConfigurationPropertySection runat="server" Id="MainControls1">
              <TemplateRowsArea>

               <Nintex:ConfigurationProperty runat="server" FieldTitle="Script PowerShell" RequiredField="False">
                   <TemplateControlArea>                       
                        <Nintex:SingleLineInput clearFieldOnInsert="true" filter="number" runat="server" id="sliProperty"></Nintex:SingleLineInput>
                  </TemplateControlArea>
                </Nintex:ConfigurationProperty>
                  
               <Nintex:ConfigurationProperty runat="server" FieldTitle="Código PowerShell" RequiredField="False">
                   <TemplateControlArea>                       
                        <Nintex:PlainTextWebControl clearFieldOnInsert="true" filter="number" runat="server" id="ptwbProperty" width="100%"></Nintex:PlainTextWebControl>
                  </TemplateControlArea>
                </Nintex:ConfigurationProperty>
              </TemplateRowsArea>
            </Nintex:ConfigurationPropertySection>

  <Nintex:DialogBody runat="server" id="DialogBody">
  </Nintex:DialogBody>
</asp:Content>
