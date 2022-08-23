using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptARCHIVE_FORM : UimScriptDocument
    {
        private const string _validationRule = "One";

        string rejectReasonCode;

        int _docNodeId;
        //IUimFieldDataContext _valField;
        //IUimDataContext _dataContext;

        //private Dictionary<string, string> values;
        DocumentNode docNode;

        public ScriptARCHIVE_FORM()
            : base()
        {

        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;

            //MessageBox.Show("DocumentLoad event!!!");

            try
            {

                //Set OperatorID
                if (Share.getOperatorID() == "*")
                {
                    dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
                }
                else
                {
                    dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
                }

                //Set machine name
                if (ModCommon.gstrMachineName.Length == 0)
                {
                    dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void DocumentUnload(IUimDataContext dataContext)
        {
            
        }

        public void DocumentExtracted(IUimDataContext dataContext)
        {

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

            AutoValidation(dataContext);
        }
        
        public void FormLoad(IUimDataEntryFormContext form)
        {

        }

        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {

            try
            {
                if (controlContext.ChoiceValue == "1")
                {
                    ValidationRejectForm rejectForm = new ValidationRejectForm();
                    DataAccess dataAccess = new DataAccess();

                    Dictionary<string, string> rejectOptions = new Dictionary<string, string>();

                    DataTable automateToIndexOptionsTable = dataAccess.getRejectOptions("AUTOMATE_INDEX").Tables[0];
                    foreach (DataRow row in automateToIndexOptionsTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addWrongDeptOption(row["REJECT_OPTION"].ToString().Trim());
                    }


                    DataTable wrongCompanyOptionsTable = dataAccess.getRejectOptions("WRONG_COMPANY").Tables[0];
                    foreach (DataRow row in wrongCompanyOptionsTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addWrongCompanyOption(row["REJECT_OPTION"].ToString().Trim());
                    }

                    //DataTable issuesTable = dataAccess.getRejectOptions("ISSUES").Tables[0];
                    DataTable issuesTable = dataAccess.getRejectOptions("INDEX_ISSUES").Tables[0];
                    foreach (DataRow row in issuesTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                        rejectForm.addIssueOption(row["REJECT_OPTION"].ToString().Trim());
                    }

                    DataTable TrashTable = dataAccess.getRejectOptions("TRASH").Tables[0];
                    foreach (DataRow row in TrashTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                    }

                    DataTable InterofficeTable = dataAccess.getRejectOptions("INTER_OFFICE").Tables[0];
                    foreach (DataRow row in InterofficeTable.Rows)
                    {
                        rejectOptions.Add(row["REJECT_OPTION"].ToString().Trim(), row["REASON_CODE"].ToString().Trim());
                    }

                    DialogResult dRes = rejectForm.ShowDialog();

                    if (dRes == DialogResult.OK)
                    {
                        string rejectResult = rejectForm.RejectResult;
                        rejectReasonCode = rejectOptions[rejectResult];
                        controlContext.FindFieldData("Validation").SetValue(rejectResult);
                        controlContext.FindFieldData("Validated").SetValue(rejectResult);

                    }
                    else
                    {
                        controlContext.SetText("0");
                    }
                }
                else //Unmarking reject button
                {
                    rejectReasonCode = "";
                    controlContext.FindFieldData("Validation").SetValue("");
                    controlContext.FindFieldData("Validated").SetValue("");
                }
            }
            catch (Exception ex)
            {
                controlContext.SetText("0");
                rejectReasonCode = "";
                controlContext.FindFieldData("Validation").SetValue("");
                controlContext.FindFieldData("Validated").SetValue("");
            }

        }



        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            if (dataContext.FindFieldDataContext("VALIDATED").Text == "")
            {
                string ruleFailMessage = "Not Validated";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }
            /*
            If Trim(CurrentDocument.Fields("VALIDATED").Value) = "" Then
                CurrentDocument.Fields("VALIDATED").RequiresValidation = True
                CurrentDocument.Fields("VALIDATED").SetStatusError
            Else
                CurrentDocument.Fields("VALIDATED").RequiresValidation = False
                CurrentDocument.Fields("VALIDATED").SetStatusOK
            End If
             */


        }

        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                //Parse the barcode and assign values for respective fields
                ParseBarcode(dataContext.FindFieldDataContext("BARCODE").Text, dataContext);

                //Assign AutoTemplate Code
                //_dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").SetValue(_dataContext.\\Template.Code <<-this is written in old version
                //commented out like the as-is code

                if (dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").Text != ""
                    && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != ""
                    && dataContext.FindFieldDataContext("AUTO_SYSTEM_ID").Text != ""
                    && dataContext.FindFieldDataContext("AUTO_LINE_OF_BUSINESS").Text != ""
                    && dataContext.FindFieldDataContext("AUTO_DOCUMENT_TYPE").Text != "")
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                }

            }
            catch (Exception ex)
            {
                //WriteError(ex.Message.ToString(), "AutoValidation");
                //not sure how to set output message or setstatuserror
            }
        }

        private void ParseBarcode(string BARCODE, IUimDataContext dataContext)
        {
            //Extract fields values from BARCODE variable
            dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue(ModCommon.ParseParam(BARCODE, 7, Convert.ToChar(124).ToString()).Replace(Convert.ToChar(13).ToString(), ""));
            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(ModCommon.ParseParam(BARCODE, 8, Convert.ToChar(124).ToString()).Replace(Convert.ToChar(13).ToString(), ""));
            dataContext.FindFieldDataContext("AUTO_DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(BARCODE, 9, Convert.ToChar(124).ToString()).Replace(Convert.ToChar(13).ToString(), ""));
            dataContext.FindFieldDataContext("AUTO_SYSTEM_ID").SetValue(ModCommon.ParseParam(BARCODE, 10, Convert.ToChar(124).ToString()).Replace(Convert.ToChar(13).ToString(), ""));
            dataContext.FindFieldDataContext("AUTO_LINE_OF_BUSINESS").SetValue(ModCommon.ParseParam(BARCODE, 11, Convert.ToChar(124).ToString()).Replace(Convert.ToChar(13).ToString(), ""));
            
        }
    }
}
