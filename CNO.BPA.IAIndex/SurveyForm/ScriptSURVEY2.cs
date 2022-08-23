using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptSURVEY2 : UimScriptDocument
    {

        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        //private string ddrunning;


        private int _docNodeId;

        private IUimFieldDataContext _valField;

        private DocumentNode docNode;

        private string rejectReasonCode;

        private bool AutoValidationRan = false;
        private bool extraction = false;
        


        public ScriptSURVEY2() : base()
        {
            this.rejectReasonCode = "";
        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;

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
                dataContext.FindFieldDataContext("MACHINE_NAME").SetValue(ModCommon.getMachineName());


            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void DocumentUnload(IUimDataContext dataContext)
        {

            try
            {
                extraction = false;


                string rejected = dataContext.FindFieldDataContext("Rejected").Text;
                if (rejected == "1")
                {

                    CommonParameters CP = new CommonParameters();
                    string BatchNo = docNode.getValues()["BATCH_NO"];
                    CP.BatchNo = BatchNo.Trim();
                    CP.CurrentNodeID = docNode.getValues()["NODE_ID"];
                    DataAccess dataAcces = new DataAccess();
                    dataAcces.createBatchItemReject(ref CP);

                }

                //TODO: Default(set to 0) Replicate value on the last document no matter what..\


                //dataContext.FindFieldDataContext("Rejected")



                //             //Extraction methods does not work in Identification Step [21.4 upgrade 20th July 2022]
                /*
                dataContext.FindFieldDataContext("D_VALIDATION_AUDIT_START").SetValue(docNode.getValues()["D_VALIDATION_AUDIT_START"]);
                dataContext.FindFieldDataContext("D_BATCH_ITEM_ID").SetValue(docNode.getValues()["D_BATCH_ITEM_ID"]);
                dataContext.FindFieldDataContext("D_DD_ITEM_SEQ").SetValue(docNode.getValues()["D_DD_ITEM_SEQ"]);
                *
                */

            }
            catch (Exception ex)
            {

            }

        }

        public void DocumentExtracted(IUimDataContext dataContext)
        {

            extraction = true;
           

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

            extraction = false;
        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {



        }

        //public void SelectionChangeRejected(IUimFormControlContext controlContext)
        //{
        //    string originalStatus = controlContext.Text;
        //    ValidationRejectForm rejectForm = new ValidationRejectForm();
        //    //IBatchInformationListParameters ibparms = info.Application.CreateBatchInformationListParameters();

        //    //foreach (IBatchInformation ibinfo in info.Application.ProcessList(ibparms))
        //    //{
        //    //   rejectForm.addProcess(ibinfo.Name);
        //    //}
        //    DataAccess dataAcces = new DataAccess();
        //    List<string> IAProcessList = dataAcces.getIAProcessNames();
        //    foreach (string processName in IAProcessList)
        //    {
        //        ////rejectForm.addProcess(processName);
        //    }

        //    DialogResult dRes = rejectForm.ShowDialog();

        //    if (dRes == DialogResult.OK)
        //    {
        //        string rejectResult = rejectForm.RejectResult;
        //        controlContext.FindFieldData("Validation").SetValue(rejectResult);
        //        controlContext.FindFieldData("Validated").SetValue(rejectResult);   

        //    }
        //    else
        //    {
        //        if (originalStatus == "1")
        //        {
        //            controlContext.SetText("0");
        //        }
        //        else
        //        {
        //            controlContext.SetText("1");
        //        }
        //    }

        //}

        // Method taken from source code shared by RUNTEAM [21.4 upgrade 26th july 22]
        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {
            try
            {
                controlContext.FindFieldData("VALIDATED").SetValue("");
                if (controlContext.ChoiceValue == "1")
                {
                    ValidationRejectForm validationRejectForm = new ValidationRejectForm();
                    DataAccess dataAccess = new DataAccess();
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    DataTable dataTable = dataAccess.getRejectOptions("AUTOMATE_INDEX").Tables[0];
                    foreach (DataRow dataRow in dataTable.Rows)
                    {
                        dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
                        validationRejectForm.addWrongDeptOption(dataRow["REJECT_OPTION"].ToString().Trim());
                    }
                    DataTable dataTable2 = dataAccess.getRejectOptions("WRONG_COMPANY").Tables[0];
                    foreach (DataRow dataRow in dataTable2.Rows)
                    {
                        dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
                        validationRejectForm.addWrongCompanyOption(dataRow["REJECT_OPTION"].ToString().Trim());
                    }
                    DataTable dataTable3 = dataAccess.getRejectOptions("VALIDATION_ISSUES").Tables[0];
                    foreach (DataRow dataRow in dataTable3.Rows)
                    {
                        dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
                        validationRejectForm.addIssueOption(dataRow["REJECT_OPTION"].ToString().Trim());
                    }
                    DataTable dataTable4 = dataAccess.getRejectOptions("TRASH").Tables[0];
                    foreach (DataRow dataRow in dataTable4.Rows)
                    {
                        dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
                    }
                    DataTable dataTable5 = dataAccess.getRejectOptions("INTER_OFFICE").Tables[0];
                    foreach (DataRow dataRow in dataTable5.Rows)
                    {
                        dictionary.Add(dataRow["REJECT_OPTION"].ToString().Trim(), dataRow["REASON_CODE"].ToString().Trim());
                    }
                    DialogResult dialogResult = validationRejectForm.ShowDialog();
                    if (dialogResult == DialogResult.OK)
                    {
                        string rejectResult = validationRejectForm.RejectResult;
                        this.rejectReasonCode = dictionary[rejectResult];
                        controlContext.FindFieldData("RejectOption").SetValue(rejectResult);
                        controlContext.FindFieldData("Rejected").SetValidationError(null);
                        IUimFieldDataContext[] fieldDataContextArray = controlContext.FieldDataContext.UimDataContext.GetFieldDataContextArray();
                        for (int i = 0; i < fieldDataContextArray.Length; i++)
                        {
                            IUimFieldDataContext uimFieldDataContext = fieldDataContextArray[i];
                            if (uimFieldDataContext.ValidationError != null)
                            {
                                uimFieldDataContext.SetValidationError(null);
                            }
                        }
                    }
                    else
                    {
                        controlContext.SetText("0");
                    }
                }
                else
                {
                    this.rejectReasonCode = "";
                }
            }
            catch (Exception var_12_462)
            {
                controlContext.SetText("0");
                this.rejectReasonCode = "";
                controlContext.FindFieldData("Validated").SetValue("");
            }
        }
        private void AutoValidation(IUimDataContext dataContext)
        {
            int intCount = 0;
            int intValue = 0;
            int[] intValues = new int[5];
            int intFieldTotal = 0;

            // ERROR: Not supported in C#: OnErrorStatement

            try
            {
                //for each question we need to verify that only 1 field is populated.
                for (intCount = 1; intCount <= 2; intCount++)
                {
                    intValues[0] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_1").Text);
                    intValues[1] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_2").Text);
                    intValues[2] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_3").Text);
                    intValues[3] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_4").Text);
                    intValues[4] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_5").Text);

                    intFieldTotal = intValues[0] + intValues[1] + intValues[2] + intValues[3] + intValues[4];
                    if (intFieldTotal > 1)
                    {
                        for (intValue = 0; intValue <= 4; intValue++)
                        {
                            if (intValues[intValue] == 1)
                            {
                                string ruleFailMessage = "Only one field must be marked";
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                Exception ex = new Exception(ruleFailMessage);
                                dataContext.FindFieldDataContext("Question" + intCount + "_" + (intValue + 1)).SetValidationError(ex);
                            }
                        }
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("FAILED");
                    }
                    else if (intFieldTotal == 0)
                    {
                        for (intValue = 0; intValue <= 4; intValue++)
                        {
                            string ruleFailMessage = "One field must be marked";
                            dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                            Exception ex = new Exception(ruleFailMessage);
                            dataContext.FindFieldDataContext("Question" + intCount + "_" + (intValue + 1)).SetValidationError(ex);
                        }
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("FAILED");
                    }
                }

                if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("VALIDATED").Text.Trim()))
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValidationError(null);
                }
                else
                {
                    string ruleFailMessage = "Validation Failed";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    dataContext.FindFieldDataContext("VALIDATED").SetValidationError(ex);
                }

            }
            catch (Exception ex)
            {
                int lngErrNum = 0;
                string strErrDesc = null;

                //lngErrNum = ex.Number;
                strErrDesc = ex.Message;

                //write out error
                WriteError(lngErrNum, strErrDesc, "AutoValidation", "");
                //dataContext.FindFieldDataContext("VALIDATED").OutputMessage = strErrDesc;

                dataContext.SetValidationRuleFailMessage(_validationRule, strErrDesc);
                Exception exception = new Exception(strErrDesc);
                dataContext.FindFieldDataContext("VALIDATED").SetValidationError(exception);
            }
        }

        public void ExitControl(IUimFormControlContext formControl)
        {

            try
            {
                int intCount = 0;
                int intValue = 0;
                int[] intValues = new int[5];
                int intFieldTotal = 0;
                IUimDataContext dataContext = formControl.FieldDataContext.UimDataContext;
                //reset the validation value
                dataContext.FindFieldDataContext("VALIDATED").SetValue("");

                //Extraction functionality does not work in Identification Step [21.4 upgrade 20th July 2022]




                //Field value must  be either 1 or 0.
                if (formControl.FieldDataContext.Name.ToUpper().Contains("QUESTION") &&
                                formControl.FieldDataContext.Text.Trim() != "1" &&
                                formControl.FieldDataContext.Text.Trim() != "0")
                {
                    string ruleFailMessage = "Value must be either 1 or 0.";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    dataContext.FindFieldDataContext(formControl.FieldDataContext.Name).SetValidationError(ex);
                }


                //if manual validation happens, then for each question we need to verify that only 1 field is populated.
                for (intCount = 1; intCount <= 2; intCount++)
                {
                    intValues[0] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_1").Text);
                    intValues[1] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_2").Text);
                    intValues[2] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_3").Text);
                    intValues[3] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_4").Text);
                    intValues[4] = convertToInteger(dataContext.FindFieldDataContext("Question" + intCount + "_5").Text);

                    intFieldTotal = intValues[0] + intValues[1] + intValues[2] + intValues[3] + intValues[4];
                    if (intFieldTotal == 1)
                    {
                        foreach (IUimFieldDataContext field in dataContext.GetFieldDataContextArray())
                        {
                            if (field.Name.ToUpper().Contains("QUESTION" + intCount))
                            {
                                field.SetValidationError(null);
                            }
                        }
                    }
                    else if (intFieldTotal > 1)
                    {
                        for (intValue = 0; intValue <= 4; intValue++)
                        {
                            if (intValues[intValue] == 1)
                            {
                                string ruleFailMessage = "Only 1 field must be populated";
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                Exception ex = new Exception(ruleFailMessage);
                                dataContext.FindFieldDataContext("Question" + intCount + "_" + (intValue + 1)).SetValidationError(ex);

                            }
                        }
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("FAILED");
                    }
                }

                if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("VALIDATED").Text))
                {
                    //Validate all fields to validate the document
                    foreach (IUimFieldDataContext field in dataContext.GetFieldDataContextArray())
                    {
                        field.SetValidationError(null);
                    }
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetDataConfirmed(false);
                    string ruleFailMessage = "Validation Failed";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    dataContext.FindFieldDataContext("VALIDATED").SetValidationError(ex);
                }


            }
			
            catch (NullReferenceException err)
			{

			}
        }

        private int convertToInteger(string input)
        {

            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception ex)
            {
                return 0;
            }

        }

        private void WriteError(int errorNum, string errorDesc, string errorLocation, string batchName)
        {
            string strData;

            try
            {

                strData = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " | " + errorNum + " | " +
                    errorDesc + " | " + errorLocation + " | " + batchName;
                IADebugOutVB_PHEOMB(0, strData);

            }
            catch (Exception ex)
            {

            }

        }


    }
}
