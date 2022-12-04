﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using taskt.UI.CustomControls;
using taskt.UI.Forms;
using taskt.Core.Automation.Attributes.PropertyAttributes;

namespace taskt.Core.Automation.Commands
{
    [Serializable]
    [Attributes.ClassAttributes.Group("Programs/Process Commands")]
    [Attributes.ClassAttributes.Description("This command allows you to run C# code from the input")]
    [Attributes.ClassAttributes.UsesDescription("Use this command when you want to run custom C# code commands.  The code in this command is compiled and run at runtime when this command is invoked.  This command only supports the standard framework classes.")]
    [Attributes.ClassAttributes.ImplementationDescription("This command implements 'Process.Start' and waits for the script/program to exit before proceeding.")]
    [Attributes.ClassAttributes.EnableAutomateRender(true)]
    public class RunCustomCodeCommand : ScriptCommand
    {
        [XmlAttribute]
        [PropertyDescription("Paste the C# code to execute")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        //[Attributes.PropertyAttributes.PropertyUIHelper(Attributes.PropertyAttributes.PropertyUIHelper.UIAdditionalHelperType.ShowCodeBuilder)]
        [PropertyCustomUIHelper("Show Code Builder", nameof(ShowCodeBuilderLink_Clicked))]
        [InputSpecification("Enter the code to be executed or use the builder to create your custom C# code.  The builder contains a Hello World template that you can use to build from.")]
        [SampleUsage("n/a")]
        [Remarks("")]
        public string v_Code { get; set; }

        [XmlAttribute]
        [PropertyDescription("Optional - Supply Arguments")]
        [PropertyUIHelper(PropertyUIHelper.UIAdditionalHelperType.ShowVariableHelper)]
        [InputSpecification("Enter arguments that the custom code will receive during execution")]
        [SampleUsage("n/a")]
        [Remarks("")]
        public string v_Args { get; set; }

        [XmlAttribute]
        [PropertyDescription("Optional - Select the variable to receive the output")]
        [InputSpecification("Select or provide a variable from the variable list")]
        [SampleUsage("**vSomeVariable**")]
        [Remarks("If you have enabled the setting **Create Missing Variables at Runtime** then you are not required to pre-define your variables, however, it is highly recommended.")]
        public string v_applyToVariableName { get; set; }

        public RunCustomCodeCommand()
        {
            this.CommandName = "RunCustomCodeCommand";
            this.SelectionName = "Run Custom Code";
            this.CommandEnabled = true;
            this.CustomRendering = true;
        }

        public override void RunCommand(object sender)
        {
            //create compiler service
            var compilerSvc = new CompilerServices();
            var customCode = v_Code.ConvertToUserVariable(sender);

            //compile custom code
            var result = compilerSvc.CompileInput(customCode);

            //check for errors
            if (result.Errors.HasErrors)
            {
                //throw exception
                var errors = string.Join(", ", result.Errors);
                throw new Exception("Errors Occured: " + errors);
            }
            else
            {

                var arguments = v_Args.ConvertToUserVariable(sender);
            
                //run code, taskt will wait for the app to exit before resuming
                System.Diagnostics.Process scriptProc = new System.Diagnostics.Process();
                scriptProc.StartInfo.FileName = result.PathToAssembly;
                scriptProc.StartInfo.Arguments = arguments;

                if (!String.IsNullOrEmpty(v_applyToVariableName))
                {
                    //redirect output
                    scriptProc.StartInfo.RedirectStandardOutput = true;
                    scriptProc.StartInfo.UseShellExecute = false;
                }
             

                scriptProc.Start();

                scriptProc.WaitForExit();

                if (!String.IsNullOrEmpty(v_applyToVariableName))
                {
                    var output = scriptProc.StandardOutput.ReadToEnd();
                    output.StoreInUserVariable(sender, v_applyToVariableName);
                }

                scriptProc.Close();
            }


        }

        private void ShowCodeBuilderLink_Clicked(object sender, EventArgs e)
        {
            var targetTextbox = (TextBox)((CommandItemControl)sender).Tag;
            using (UI.Forms.Supplemental.frmCodeBuilder codeBuilder = new UI.Forms.Supplemental.frmCodeBuilder(targetTextbox.Text))
            {
                if (codeBuilder.ShowDialog() == DialogResult.OK)
                {
                    targetTextbox.Text = codeBuilder.rtbCode.Text;
                }
            }
        }

        //public override List<Control> Render(frmCommandEditor editor)
        //{
        //    base.Render(editor);

        //    RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_Code", this, editor));

        //    RenderedControls.AddRange(CommandControls.CreateDefaultInputGroupFor("v_Args", this, editor));

        //    RenderedControls.Add(CommandControls.CreateDefaultLabelFor("v_applyToVariableName", this));
        //    var VariableNameControl = CommandControls.CreateStandardComboboxFor("v_applyToVariableName", this).AddVariableNames(editor);
        //    RenderedControls.AddRange(CommandControls.CreateDefaultUIHelpersFor("v_applyToVariableName", this, VariableNameControl, editor));
        //    RenderedControls.Add(VariableNameControl);

        //    return RenderedControls;
        //}
        public override string GetDisplayValue()
        {
            return base.GetDisplayValue();
        }

        public override bool IsValidate(frmCommandEditor editor)
        {
            base.IsValidate(editor);

            if (String.IsNullOrEmpty(this.v_Code))
            {
                this.validationResult += "C# code is empty.\n";
                this.IsValid = false;
            }

            return this.IsValid;
        }
        public override void convertToIntermediate(EngineSettings settings, List<Script.ScriptVariable> variables)
        {
            var cnv = new Dictionary<string, string>();
            cnv.Add("v_Code", "convertToIntermediateVariableParser");
            convertToIntermediate(settings, cnv, variables);
        }
    }
}