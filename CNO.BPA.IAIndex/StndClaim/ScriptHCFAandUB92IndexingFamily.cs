using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;
//using CNO.BPA.DPValidationRejects;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptHCFAandUB92IndexingFamily : UimScriptDocument
    {

        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError=true)] 
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        string rejectReasonCode;
        CommonParameters CP;

        private bool AutoValidationRan = false;
        private bool extraction = false;

        /*
         *  ExternalValues: { SiteIDValue, Template_Code }
         */
        string[] externalValues;

        public ScriptHCFAandUB92IndexingFamily() : base() {
            rejectReasonCode = "";

        }

        public void DocumentLoad(IUimDataContext dataContext) {

            dataContext.TaskFinishOnErrorNotAllowed = true;
            //_dataContext = dataContext;

            //MessageBox.Show("DocumentLoad event!!!");

            try
            {
                CP = new CommonParameters();

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

                if (Share.getDD_Running() == true)
                {
                   //set ddrunning to f in case task was previously closed improperly
                    Share.setDD_Running(false); // nodeValueProvider.SetString(nodePath + "/$instance=Standard_MDF/E_DDRUNNING", "F");

                }

                //             //Extraction methods does not work in Identification Step [21.4 upgrade 20th July 2022]
                /*

                //Clear TOB validation

                // 
                dataContext.FindFieldDataContext("MANUAL_TYPE_OF_BILL").SetValidationError(null);
                dataContext.FindFieldDataContext("OCR_TYPE_OF_BILL").SetValidationError(null);
                

                //Check for auto validation
                string ruleFailMessage = "Validation field is empty";
                if (!dataContext.FindFieldDataContext("Validation").Text.ToUpper().Contains("VAL"))
                {
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    dataContext.FindFieldDataContext("Validation").SetValidationError(ex);

                    //Check for TOB for UB forms
                    MANUAL_TYPE_OF_BILL_ValidateValue(dataContext);
                }

                */



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        public void DocumentUnload(IUimDataContext dataContext) {

            //MessageBox.Show("DocumentUnload event");
            try
            {

                if (!extraction)
                {
                    string rejected = dataContext.FindFieldDataContext("Rejected").Text;
                    if (rejected == "1")
                    {

                        CNO.BPA.DataHandler.CommonParameters cp = new CNO.BPA.DataHandler.CommonParameters();
                        cp.BatchNo = dataContext.FindFieldDataContext("BatchName").Text.Trim();
                        cp.CurrentNodeID = dataContext.FindFieldDataContext("NodeID").Text.Trim();
                        cp.ProcessStep = "ManualValidation";
                        cp.ReasonCode = rejectReasonCode;
                        DataAccess dataAccess = new DataAccess();
                        dataAccess.createBatchItemReject(ref cp);

                    }
                    
                    //Extraction methods does not work in Identification Step [21.4 upgrade 20th July 2022]

                    //else
                    //{
                    //    DataAccess dataAccess = new DataAccess();
                    //    dataAccess.updateReasonCode(dataContext.FindFieldDataContext("BatchName").Text.Trim(), dataContext.FindFieldDataContext("NodeID").Text.Trim(), "Unrejected");
                    //}
                }

            }
            catch (Exception ex)
            {

            }

        }
        
        public void DocumentExtracted(IUimDataContext dataContext) {
            extraction = true;

            //External Values
            char[] delimiterChar = { ';' };
            externalValues = dataContext.StepCustomValue.Split(delimiterChar);

            //Clear validation error on Auto_TOB field
            dataContext.FindFieldDataContext("AUTO_TYPE_OF_BILL").SetValidationError(null);

            //Set OperatorID9
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

            //Set Site_ID
            if (!string.IsNullOrEmpty(externalValues[0]))
            {
                dataContext.FindFieldDataContext("SITE_ID").SetValue(externalValues[0]);
            }
            else
            {
                dataContext.FindFieldDataContext("SITE_ID").SetValue("CIC");
            }


            OCR_POLICY_NO1_AfterRecognition(dataContext);
            OCR_POLICY_NO2_AfterRecognition(dataContext);
            OCR_POLICY_NO3_AfterRecognition(dataContext);
            OCR_POLICY_NO4_AfterRecognition(dataContext);

            LOB_AfterRecognition(dataContext);
            OCR_PROVIDER_TAX_ID_AfterRecognition(dataContext);
            OCR_INSURED_BIRTHDATE_AfterRecognition(dataContext);
            OCR_STATE_AfterRecognition(dataContext);
            OCR_TYPE_OF_BILL_AfterRecognition(dataContext);
            OCR_ZIP_AfterRecognition(dataContext);
            OCR_INSURED_NAME_AfterRecognition(dataContext);
            OCR_PATIENT_BIRTHDATE_AfterRecognition(dataContext);
            OCR_PATIENT_NAME_AfterRecognition(dataContext);
            OCR_PHONE_AfterRecognition(dataContext);
            ROUTE_CODE_AfterRecognition(dataContext);


            AutoValidation(dataContext);

        }
        
        public void SelectionChangeRejected(IUimFormControlContext controlContext)
        {
            try
            {

                controlContext.FindFieldData("Validation").SetValue("");
                controlContext.FindFieldData("VALIDATED").SetValue("");

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
                        controlContext.FindFieldData("RejectOption").SetValue(rejectResult);

                        Validation_ValidateValue(controlContext.ParentForm.UimDataContext);

                    }
                    else
                    {
                        controlContext.SetText("0");
                    }
                }
                else //Unmarking reject button
                {
                    rejectReasonCode = "";
                    
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

        public void ButtonClickDataDirector(IUimFormControlContext controlContext)
        {
            try
            {
                
                if (Share.getDD_Running() == true)
                {
                    MessageBox.Show("Data Director is already running. Please complete current document before attempting to launch Data Director again.", "Data Director already running.");
                    return;
                }


                LaunchValidationSearch(controlContext.ParentForm.UimDataContext);

                
                //_dataDirector.LaunchDD(ref _cp);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            

            //If reject skip all validation rule and let it proceed
            /*if (dataContext.FindFieldDataContext("Rejected").Text == "1")
            {
                return;
            }*/


           
        }

        public void ExitControlPolicyNo(IUimFormControlContext controlContext)
        {
            CP.PolicyNo = controlContext.Text;
        }

        #region LOB
        private void LOB_AfterRecognition(IUimDataContext dataContext)
        {
            LineOfBusiness(dataContext);
        }

        public void ExitControlLOB(IUimFormControlContext controlContext)
        {
            LOB_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void LOB_ValidateValue(IUimDataContext dataContext)
        {
            LineOfBusiness(dataContext);
        }
        #endregion LOB

        #region MANUAL_PROVIDER_TAX_ID
        private void MANUAL_PROVIDER_TAX_ID_ValidateValue(IUimDataContext dataContext)
        {
            ProviderTaxID(dataContext);

            IUimFieldDataContext field = dataContext.FindFieldDataContext("MANUAL_PROVIDER_TAX_ID");
            if (!string.IsNullOrEmpty(field.Text))
            {
                int result;
                if (!int.TryParse(field.Text, out result) || field.Text.Length > 9)
                {
                    string ruleFailMessage = "Tax ID must be blank, or 9 digit";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
            }

            //Remove concatenated date
            if (field.Text.Length > 9)
            {
                field.SetValue(field.Text.Substring(0, 9));
            }
        }

        public void ExitControlMANUAL_PROVIDER_TAX_ID(IUimFormControlContext controlContext)
        {
            MANUAL_PROVIDER_TAX_ID_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        
        #endregion MANUAL_PROVIDER_TAX_ID

        #region MANUAL_TYPE_OF_BILL
        private void MANUAL_TYPE_OF_BILL_ValidateValue(IUimDataContext dataContext)
        {
            /*
            If Left(CurrentDocument.Template.Code,2) = "UB" Then
		        TOB()
	        End If*/
            if (dataContext.FindFieldDataContext("TEMPLATE_CODE").Text.ToUpper().Substring(0, 2) == "UB")
            {
                TOB(dataContext);
            }

            
        }

        public void ExitControlMANUAL_TYPE_OF_BILL(IUimFormControlContext controlContext)
        {
            MANUAL_TYPE_OF_BILL_ValidateValue(controlContext.ParentForm.UimDataContext);
        }        
        #endregion MANUAL_TYPE_OF_BILL

        #region OCR_INSURED_BIRTHDATE
        public void ExitControlOCR_INSURED_BIRTHDATE(IUimFormControlContext controlContext)
        {
            OCR_INSURED_BIRTHDATE_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void OCR_INSURED_BIRTHDATE_AfterRecognition(IUimDataContext dataContext)
        {
            InsuredBirthDate(dataContext);
        }

        private void OCR_INSURED_BIRTHDATE_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            string strTemp1 = null;
            string strTemp2 = null;
            string strTemp3 = null;

            //Field cleanup
            //InsuredBirthDate(field); //This is called within ExecuteValidationRuleOne()
            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_INSURED_BIRTHDATE");

            //Format checking
            if (string.IsNullOrEmpty(field.Text.Trim()))
            {
                field.SetDataConfirmed(true);
                return;
            }

            if (field.Text.Length != 10)
            {

                string ruleFailMessage = "Must be in the format of MM/DD/YYYY";
                //field.UimDataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                //throw new Exception(ruleFailMessage);
            }

            strTemp1 = field.Text.Substring(0, 2);
            strTemp2 = field.Text.Substring(2, 2);
            strTemp3 = field.Text.Substring(4, 4);

            DateTime result;
            if (!DateTime.TryParse(strTemp1 + "/" + strTemp2 + "/" + strTemp3, out result))
            {

                string ruleFailMessage = "Invalid Date Format";
                //field.UimDataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                //throw new Exception(ruleFailMessage);
            }

        }
        #endregion OCR_INSURED_BIRTHDATE

        #region OCR_INSURED_NAME
        public void ExitControlOCR_INSURED_NAME(IUimFormControlContext controlContext) {
            InsuredName(controlContext.ParentForm.UimDataContext);
        }
        private void OCR_INSURED_NAME_AfterRecognition(IUimDataContext dataContext)
        {
            InsuredName(dataContext);
        }
        #endregion OCR_INSURED_NAME

        #region OCR_PATIENT_BIRTHDATE
        private void OCR_PATIENT_BIRTHDATE_AfterRecognition(IUimDataContext dataContext)
        {
                PatientBirthDate(dataContext);
        }
    

        private void OCR_PATIENT_BIRTHDATE_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            string strTemp1 = null;
            string strTemp2 = null;
            string strTemp3 = null;
            string strTemp4 = null;

            //Field cleanup
            PatientBirthDate(dataContext);

            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_PATIENT_BIRTHDATE");

            //Format checking
            if (string.IsNullOrEmpty(field.Text.Trim()))
            {
                field.SetDataConfirmed(true);
                return;
            }

            if (field.Text.Length != 10)
            {

                string ruleFailMessage = "Must be in the format of MM/DD/YYYY";
                //field.UimDataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                //throw new Exception(ruleFailMessage);
            }

            strTemp1 = field.Text.Substring(0, 2);
            strTemp2 = field.Text.Substring(2, 2);
            strTemp3 = field.Text.Substring(4, 4);

            DateTime result;
            if (!DateTime.TryParse(strTemp1 + "/" + strTemp2 + "/" + strTemp3, out result))
            {

                string ruleFailMessage = "Invalid Date Format";
                //field.UimDataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                //throw new Exception(ruleFailMessage);
            }

        }
        #endregion OCR_PATIENT_BIRTHDATE

        #region OCR_PATIENT_NAME
        private void OCR_PATIENT_NAME_AfterRecognition(IUimDataContext dataContext)
        {
            PatientName(dataContext);
        }
        public void ExitControlOCR_PATIENT_NAME(IUimFormControlContext controlContext)
        {
            PatientName(controlContext.ParentForm.UimDataContext);
        }   
        #endregion OCR_PATIENT_NAME

        #region OCR_PHONE

        private void OCR_PHONE_AfterRecognition(IUimDataContext dataContext)
        {
            PatientTelephone(dataContext);

        }
        public void ExitControlOCR_PHONE(IUimFormControlContext controlContext)
        {
            OCR_PHONE_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void OCR_PHONE_ValidateValue(IUimDataContext dataContext)
        {
            //Field cleanup
            PatientTelephone(dataContext);

            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_PHONE");

            //PatientTelephone must be blank, or length of 7 or 10 numeric
            if (field.Text.Trim().Length == 7 || field.Text.Trim().Length == 10)
            {
                long result;
                if (!long.TryParse(field.Text, out result))
                {
                    string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                    field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
            }
            else
            {
                if (field.Text.Trim().Length != 0)
                {
                    string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                    field.UimDataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
            }

        }
        #endregion OCR_PHONE

        #region OCR_POLICY_NO1
        public void ExitControlOCR_POLICY_NO1(IUimFormControlContext controlContext)
        {
            OCR_POLICY_NO1_ValidateValue(controlContext.ParentForm.UimDataContext);
        }
        private void OCR_POLICY_NO1_AfterRecognition(IUimDataContext dataContext)
        {
            long lngLen = 0;

            policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO1"));

            //Check PolicyNumber1 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result; 
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

        }

        private void OCR_POLICY_NO1_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            long lngLen = 0;

            if (dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper() == "")
            {
                //Field cleanup
                policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO1"));

                            //Check PolicyNumber1 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result; 
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            }

        }
        
        #endregion OCR_POLICY_NO1
            
        #region OCR_POLICY_NO2
        public void ExitControlOCR_POLICY_NO2(IUimFormControlContext controlContext)
        {
            OCR_POLICY_NO2_ValidateValue(controlContext.ParentForm.UimDataContext);
        }
        private void OCR_POLICY_NO2_AfterRecognition(IUimDataContext dataContext)
        {
            long lngLen = 0;

            policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO2"));

            //Check PolicyNumber2 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result; 
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

        }

        private void OCR_POLICY_NO2_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            long lngLen = 0;

            if (dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper() == "")
            {
                //Field cleanup
                policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO2"));

                            //Check PolicyNumber2 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result; 
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            }

        }

        #endregion OCR_POLICY_NO2

        #region OCR_POLICY_NO3
        public void ExitControlOCR_POLICY_NO3(IUimFormControlContext controlContext)
        {
            OCR_POLICY_NO3_ValidateValue(controlContext.ParentForm.UimDataContext);
        }
        private void OCR_POLICY_NO3_AfterRecognition(IUimDataContext dataContext)
        {
            long lngLen = 0;

            policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO3"));

            //Check PolicyNumber3 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result;
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

        }

        private void OCR_POLICY_NO3_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            long lngLen = 0;

            if (dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper() == "")
            {
                //Field cleanup
                policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO3"));

                //Check PolicyNumber3 field format

                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
                {
                    //Policy Number must be blank or 6 to 10 characters in length
                    lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text.Trim().Length;
                    if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                    {
                        string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }

                }

                //Bankers field format check

                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                {
                    //Policy Number should be no greater than 9 digits.
                    lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text.Trim().Length;

                    if (lngLen > 9)
                    {
                        string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }
                    int result;
                    if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text, out result) && lngLen > 0)
                    {
                        string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }

                }

            }

        }

        #endregion OCR_POLICY_NO3

        #region OCR_POLICY_NO4
        public void ExitControlOCR_POLICY_NO4(IUimFormControlContext controlContext)
        {
            OCR_POLICY_NO4_ValidateValue(controlContext.ParentForm.UimDataContext);
        }
        private void OCR_POLICY_NO4_AfterRecognition(IUimDataContext dataContext)
        {
            long lngLen = 0;

            policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO4"));

            //Check PolicyNumber4 field format

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
            {
                //Policy Number must be blank or 6 to 10 characters in length
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text.Trim().Length;
                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                {
                    string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

            //Bankers field format check

            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
            {
                //Policy Number should be no greater than 9 digits.
                lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text.Trim().Length;

                if (lngLen > 9)
                {
                    string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
                int result;
                if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text, out result) && lngLen > 0)
                {
                    string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }

            }

        }

        private void OCR_POLICY_NO4_ValidateValue(IUimDataContext dataContext)
        {
            //Variable declaration
            long lngLen = 0;

            if (dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper() == "")
            {
                //Field cleanup
                policyNumberCleanUp(dataContext.FindFieldDataContext("OCR_POLICY_NO4"));

                //Check PolicyNumber4 field format

                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
                {
                    //Policy Number must be blank or 6 to 10 characters in length
                    lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text.Trim().Length;
                    if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                    {
                        string ruleFailMessage = "Carmel Policy Numbers must be blank or 5 to 10 characters in length";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }

                }

                //Bankers field format check

                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                {
                    //Policy Number should be no greater than 9 digits.
                    lngLen = dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text.Trim().Length;

                    if (lngLen > 9)
                    {
                        string ruleFailMessage = "Bankers Policy Numbers should be no greater than 9 digits";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }
                    int result;
                    if (!int.TryParse(dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text, out result) && lngLen > 0)
                    {
                        string ruleFailMessage = "Bankers Policy Numbers must be numeric";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }

                }

            }

        }

        #endregion OCR_POLICY_NO4

        #region OCR_PROVIDER_TAX_ID
        private void OCR_PROVIDER_TAX_ID_AfterRecognition(IUimDataContext dataContext)
        {
            ProviderTaxID(dataContext);
        }

        public void ExitControlOCR_PROVIDER_TAX_ID(IUimFormControlContext controlContext)
        {
            OCR_PROVIDER_TAX_ID_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void OCR_PROVIDER_TAX_ID_ValidateValue(IUimDataContext dataContext)
        {

            if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper()))
            {
                //Field cleanup
                ProviderTaxID(dataContext);

                IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_PROVIDER_TAX_ID");

                //Check field format
                if (!string.IsNullOrEmpty(field.Text))
                {
                    int result;
                    if (field.Text.ToUpper() != "VA" && (!int.TryParse(field.Text, out result) || field.Text.Length > 9))
                    {
                        string ruleFailMessage = "Tax ID must be blank, VA or 9 digit";
                        //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                        //dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);

                    }
                }

                //Remove concatenated date
                if (field.Text.Length > 9)
                {
                    field.SetValue(field.Text.Substring(0, 9));
                }
            }

        }
        #endregion OCR_PROVIDER_TAX_ID

        #region OCR_STATE
        private void OCR_STATE_AfterRecognition(IUimDataContext dataContext)
        {
            PatientState(dataContext);
        }

        public void ExitControlOCR_STATE(IUimFormControlContext controlContext)
        {
            OCR_STATE_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void OCR_STATE_ValidateValue(IUimDataContext dataContext)
        {
            //Field cleanup
            PatientState(dataContext);

            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_STATE");

            //Must be blank, or 2 alpha
            if (field.Text.Trim().Length == 0 | field.Text.Trim().Length == 2)
            {
                if (field.Text.Trim().Length == 2)
                {
                    int result;
                    if (int.TryParse(field.Text.Trim(), out result))
                    {
                        string ruleFailMessage = "Patient State must be blank or 2 Alphanumeric characters";
                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                        //throw new Exception(ruleFailMessage);
                    }
                }
            }
            else
            {
                string ruleFailMessage = "Patient State must be blank or 2 Alphanumeric characters";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                //throw new Exception(ruleFailMessage);
            }


        }
        #endregion OCR_STATE

        #region OCR_TYPE_OF_BILL
        private void OCR_TYPE_OF_BILL_AfterRecognition(IUimDataContext dataContext)
        {
            if (externalValues.GetUpperBound(0) > 1)
            {
                if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper()) &&
                    externalValues[1].ToUpper().Substring(0, 2) == "UB")
                {
                    TOB(dataContext);
                }
            }

        }

        public void ExitControlOCR_TYPE_OF_BILL(IUimFormControlContext controlContext)
        {
            OCR_TYPE_OF_BILL_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void OCR_TYPE_OF_BILL_ValidateValue(IUimDataContext dataContext)
        {
            if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("Validation").Text.Trim().ToUpper()))
            {
                //TOB(dataContext);
            }
            else if (dataContext.FindFieldDataContext("TEMPLATE_CODE").Text.ToUpper().Substring(0, 2) == "UB") // else if (Strings.Left(CurrentDocument.Template.Code, 2) == "UB")    
            {
                TOB(dataContext);
            }
        }

        #endregion OCR_TYPE_OF_BILL

        #region OCR_ZIP
        private void OCR_ZIP_AfterRecognition(IUimDataContext dataContext)
        {
            PatientZip(dataContext);
        }

        public void ExitControlOCR_ZIP(IUimFormControlContext controlContext)
        {
            OCR_ZIP_ValidateValue(controlContext.ParentForm.UimDataContext);
        }
    
        private void OCR_ZIP_ValidateValue(IUimDataContext dataContext)
        {
            //Field cleanup
            PatientZip(dataContext);

            IUimFieldDataContext field = dataContext.FindFieldDataContext("OCR_ZIP");

            //Patient zip must be blank, or length of 5 or 9 numeric
            if (field.Text.Trim().Length == 5 || field.Text.Trim().Length == 9)
            {
                //make sure the value has no spaces
                field.SetValue(field.Text.Trim());
                int result;
                if (!int.TryParse(field.Text.Trim(), out result))
                {
                    string ruleFailMessage = "Patient zip must be blank or of length 5 or 9 numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
            }
            else
            {
                if (field.Text.Trim().Length != 0)
                {
                    string ruleFailMessage = "Patient zip must be blank or of length 5 or 9 numeric";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    //throw new Exception(ruleFailMessage);
                }
            }

        }
        #endregion OCR_ZIP

        #region ROUTE_CODE
        private void ROUTE_CODE_AfterRecognition(IUimDataContext dataContext)
        {
            RoutingCode(dataContext);
        }

        public void ExitControlROUTE_CODE(IUimFormControlContext controlContext)
        {
            ROUTE_CODE_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void ROUTE_CODE_ValidateValue(IUimDataContext dataContext)
        {
            RoutingCode(dataContext);
        }
        #endregion ROUTE_CODE

        #region Validation
        public void ExitControlValidation(IUimFormControlContext controlContext)
        {
            Validation_ValidateValue(controlContext.ParentForm.UimDataContext);
        }

        private void Validation_ValidateValue(IUimDataContext dataContext)
        {
            //TODO: how to get template code from classification?
            //CurrentDocument.Fields("TEMPLATE_CODE").Value = CurrentDocument.Template.Code; 
            //MsgBox("TEMP CODE: " & CurrentDocument.Fields("TEMPLATE_CODE").Value)

            IUimFieldDataContext field = dataContext.FindFieldDataContext("Validation");

            if (dataContext.FindFieldDataContext("TEMPLATE_CODE").Text.ToUpper().Substring(0, 2) == "UB")
            {
                TOB(dataContext);
            }

            if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("MANUAL_PROVIDER_TAX_ID").Text))
            {
                dataContext.FindFieldDataContext("MANUAL_PROVIDER_TAX_ID").SetValue(dataContext.FindFieldDataContext("OCR_PROVIDER_TAX_ID").Text);
            }

            if (string.IsNullOrEmpty(field.Text.Trim()) && dataContext.FindFieldDataContext("Rejected").Text != "1")
            {
                /*
                 	CurrentField.RequiresValidation = True
		            CurrentField.SetStatusError 
                 */
                //TODO
                field.SetDataConfirmed(false); //CurrentField.RequiresValidation = true;
                string ruleFailMessage = "Validation field is empty";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                Exception ex = new Exception(ruleFailMessage);
                field.SetValidationError(ex);
                //throw ex;
            }
            else if (field.Text.IndexOf("(IA:") >= 0 || dataContext.FindFieldDataContext("Rejected").Text == "1") //TODO
            {
                //Clear validation error on Validation field
                field.SetDataConfirmed(true);
                field.SetValidationError(null);

                //Clear validation error on reject field
                IUimFieldDataContext rejectField = dataContext.FindFieldDataContext("Rejected");
                rejectField.SetValidationError(null);

            }
            else
            {
                //Nothing happens
            }

        }
        #endregion Validation


        private void TOB(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField;
            string strTOB = null;
            bool validTOB = false;


            //'Initialize variables
            if (!string.IsNullOrEmpty(dataContext.FindFieldDataContext("MANUAL_TYPE_OF_BILL").Text)) {
                localDpDocField = dataContext.FindFieldDataContext("MANUAL_TYPE_OF_BILL");
            } else {
                localDpDocField = dataContext.FindFieldDataContext("OCR_TYPE_OF_BILL");
            }

            if (localDpDocField != null)
            {
  
                //translate all @ to 0s
                localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));

                localDpDocField.SetValue(localDpDocField.Text.Replace("i", "1"));

                localDpDocField.SetValue(localDpDocField.Text.Replace("O", "0"));

                //Remove all punctuation marks
                localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

                //Trim all leading and trailing spaces
                localDpDocField.SetValue(localDpDocField.Text.Trim());

                localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

                //remove leading zeroes
                if (localDpDocField.Text.Length == 4)
                {
                    if (localDpDocField.Text.IndexOf("0") == 0)
                    {
                        localDpDocField.SetValue(localDpDocField.Text.Substring(1,3));
                    }
                }

                //validate TOB
                strTOB = localDpDocField.Text;

                CommonParameters CP = new CommonParameters();
                Validation dataValidation = new Validation();

		        CP.TypeOfBill = strTOB;

		        validTOB =  dataValidation.ValidateTOB(ref CP);

                if (validTOB == false) //fail
                {

                    localDpDocField.SetDataConfirmed(false); //localDpDocField.RequiresValidation = true;
                    string ruleFailMessage = "TOB is not valid";
                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                    Exception ex = new Exception(ruleFailMessage);
                    localDpDocField.SetValidationError(ex);
                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);

                }
                else if (extraction == true) //extraction(recognition)
                {

                    dataContext.FindFieldDataContext("AUTO_TYPE_OF_BILL").SetDataConfirmed(true);
                    dataContext.FindFieldDataContext("AUTO_TYPE_OF_BILL").SetValue(strTOB);
                    dataContext.FindFieldDataContext("AUTO_TYPE_OF_BILL").SetValidationError(null);
                    dataContext.FindFieldDataContext("OCR_TYPE_OF_BILL").SetValidationError(null);

                }
                else //validation
                {
                    if (!string.IsNullOrEmpty(dataContext.FindFieldDataContext("MANUAL_TYPE_OF_BILL").Text))
                    {
                        localDpDocField.SetDataConfirmed(true);
                        localDpDocField.SetValidationError(null);
                        //If manually entered TOB is validated, clear validation error on OCR TOB field.
                        IUimFieldDataContext ocrTobField = dataContext.FindFieldDataContext("OCR_TYPE_OF_BILL");
                        ocrTobField.SetValidationError(null);
                    }
                }

            }

        }

        private void AutoValidation(IUimDataContext dataContext)
        {

            int lngCnt;
	        int lngReturnValue;
	        string strSourceData = "";
            string[] strPolNum = new string[5];
	        string strPatientBirthDateSlashes;
	        string strInsuredBirthDateSlashes;
	        //Dim NextField 						As DpDocField
            CommonParameters CP;
            Validation Validation; 


            try
            {
                AutoValidationRan = true;

                if (dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Length > 10 && dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Substring(0, 2) == "18")
                {
                    dataContext.FindFieldDataContext("OCR_POLICY_NO1").SetValue(dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Substring(2, 12));
                }
                if (dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Length > 10 && dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Substring(0, 2) == "18")
                {
                    dataContext.FindFieldDataContext("OCR_POLICY_NO2").SetValue(dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Substring(2, 12));
                }

                strPolNum[0] = dataContext.FindFieldDataContext("OCR_POLICY_NO1").Text.Trim();
                strPolNum[1] = dataContext.FindFieldDataContext("DummyPolicyNumber1").Text.Trim();
                strPolNum[2] = dataContext.FindFieldDataContext("OCR_POLICY_NO2").Text.Trim();
                strPolNum[3] = dataContext.FindFieldDataContext("OCR_POLICY_NO3").Text.Trim();
                strPolNum[4] = dataContext.FindFieldDataContext("OCR_POLICY_NO4").Text.Trim();

                //'If Chicago, remove front zero before lookup
                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                {
                    strPolNum[0] = ModCommon.RemoveFrontZeroes(strPolNum[0]);
                    strPolNum[1] = ModCommon.RemoveFrontZeroes(strPolNum[1]);
                    strPolNum[2] = ModCommon.RemoveFrontZeroes(strPolNum[2]);
                    strPolNum[3] = ModCommon.RemoveFrontZeroes(strPolNum[3]);
                    strPolNum[4] = ModCommon.RemoveFrontZeroes(strPolNum[4]);
                }

                //'Format dates to MM/DD/YYYY before passing to PolicyValidation
                strPatientBirthDateSlashes = FormatDate(dataContext.FindFieldDataContext("OCR_PATIENT_BIRTHDATE").Text);
                strInsuredBirthDateSlashes = FormatDate(dataContext.FindFieldDataContext("OCR_INSURED_BIRTHDATE").Text);

                CP = new CommonParameters();
                Validation = new Validation();

                foreach (string str in strPolNum)
                {
                    if (!string.IsNullOrEmpty(str)) //str > "" in VB
                    {
                        CP.ID = str;
                        CP.WorkCategory = "CLM";
                        CP.BirthDate = strPatientBirthDateSlashes;
				        CP.SiteID = dataContext.FindFieldDataContext("SITE_ID").Text.Trim();
                        CP.InsuredName = dataContext.FindFieldDataContext("OCR_INSURED_NAME").Text.Trim();
				        CP.PatientName = dataContext.FindFieldDataContext("OCR_PATIENT_NAME").Text.Trim();
				        CP.BirthDate = strInsuredBirthDateSlashes;
				        CP.ZipCode = dataContext.FindFieldDataContext("OCR_ZIP").Text.Trim();
				        CP.State = dataContext.FindFieldDataContext("OCR_STATE").Text.Trim();
				        CP.Phone = dataContext.FindFieldDataContext("OCR_PHONE").Text.Trim();
				        CP.ProviderTIN = dataContext.FindFieldDataContext("OCR_PROVIDER_TAX_ID").Text.Trim();
                        CP.SiteRestriction = "T";

                        lngReturnValue = Validation.AutoValidate(ref CP);

                        if (lngReturnValue == 0)
                        {
                            dataContext.FindFieldDataContext("PV_RETURN").SetValue(lngReturnValue.ToString());
                            dataContext.FindFieldDataContext("AUTO_POLICY_NO1").SetValue(CP.PolicyNo);
                            dataContext.FindFieldDataContext("AUTO_PATIENT_NAME").SetValue(CP.InsuredName);
                            dataContext.FindFieldDataContext("AUTO_INSURED_NAME").SetValue(CP.InsuredName);
                            dataContext.FindFieldDataContext("AUTO_PATIENT_BIRTHDATE").SetValue(CP.BirthDate);
                            dataContext.FindFieldDataContext("AUTO_STATE").SetValue(CP.State);
                            dataContext.FindFieldDataContext("AUTO_ZIP").SetValue(CP.ZipCode);
                            dataContext.FindFieldDataContext("AUTO_PHONE").SetValue(CP.Phone);
                            dataContext.FindFieldDataContext("AUTO_INSURED_BIRTHDATE").SetValue(CP.BirthDate);
                            dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue(CP.BusinessArea);
                            dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue(CP.WorkType);
                            dataContext.FindFieldDataContext("PLAN_CODE").SetValue(CP.PlanCode);
                            dataContext.FindFieldDataContext("ISSUE_STATE").SetValue(CP.IssueState);
                            dataContext.FindFieldDataContext("ADMIN_COMPANY_CODE").SetValue(CP.CompanyNo);
                            dataContext.FindFieldDataContext("ADMIN_SYSTEM_ID").SetValue(CP.SystemNo);
                            dataContext.FindFieldDataContext("SYSTEM_ID").SetValue(CP.SystemID);
                            dataContext.FindFieldDataContext("COMPANY_CODE").SetValue(CP.CompanyCode);
                            dataContext.FindFieldDataContext("VALIDATED").SetValue("(IA:AUTOVAL)(" + CP.Validation.ToString() + ")");
                            dataContext.FindFieldDataContext("Validation").SetValue("(IA:AUTOVAL)(" + CP.Validation.ToString() + ")");
                            //''CurrentDocument.Fields("OCRPolicyNumber").Value = strPolNum(lngCnt)
                            if (CP.SortCode.Trim().Length == 0 || CP.SortCode.Trim() == "000")
                            {
                                dataContext.FindFieldDataContext("SORT_CODE").SetValue("000");
                                dataContext.FindFieldDataContext("LOB").SetValue("MS");
                            }
                            else
                            {
                                dataContext.FindFieldDataContext("SORT_CODE").SetValue(CP.SortCode);
                                dataContext.FindFieldDataContext("LOB").SetValue(CP.LineOfBusiness);
                            }
                            dataContext.FindFieldDataContext("P_SITE_ID").SetValue(CP.SiteID);
                            dataContext.FindFieldDataContext("P_WORK_CATEGORY").SetValue(CP.WorkCategory);

                            ValidatePVReturn(dataContext); //NOPE: This will be running from ExecuteValidationRule event handler

                            //'Determine source data value
                            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC")
                            { // If Trim(CurrentDocument.Fields("SITE_ID").Value) = "CIC" Then
                                switch (dataContext.FindFieldDataContext("LOB").Text)
                                { // Select Case CurrentDocument.Fields("LOB").Value
                                    case "MS":
                                        strSourceData = "2034";
                                        break;
                                    case "OH":
                                        strSourceData = "2024";
                                        break;
                                    case "LTC":
                                        strSourceData = "2002";
                                        break;
                                }
                            }
                            else if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC")
                            {
                                switch (dataContext.FindFieldDataContext("LOB").Text)
                                {
                                    case "MS":
                                        strSourceData = "1935";
                                        break;
                                    case "OH":
                                        strSourceData = "1936";
                                        break;
                                }
                            }

                            //'set the LOBSourceData value
                            dataContext.FindFieldDataContext("LOB_SOURCE_DATA").SetValue(strSourceData);
                            break;

                        }
                        else //'we received something other than a zero
                        {
                            //save the pv return value in case it is an error
				            dataContext.FindFieldDataContext("PV_RETURN").SetValue(lngReturnValue.ToString());
                            continue;
                        }

                        // From 6.5. This part of code is unreachable.
                        if (externalValues[1].Trim().ToUpper() == "UB") //if (dataContext If Left(CurrentDocument.Template.Code,2) = "UB" Then
                        {
                            TOB(dataContext);
                        }
   
                    }   
                }//end of foreach loop


                if (string.IsNullOrEmpty(dataContext.FindFieldDataContext("Validation").Text.Trim()))
                {
                    dataContext.FindFieldDataContext("Validation").SetDataConfirmed(false);
                    Exception ex = new Exception();
                    dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
                }
                else
                {
                    dataContext.FindFieldDataContext("Validation").SetDataConfirmed(true);
                }
                CP.Clear();

                AutoValidationRan = false;

            }
            catch (Exception ex)
            {
                int errNum = 0;
                string batchName = "";

                ModCommon.WriteError(errNum, ex.Message, "AutoValidation", batchName);

                dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
                AutoValidationRan = false;
            }
        }

        private void LaunchValidationSearch(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField;
            int lngSearchReturnValue = 0;
            int lngContinueOkCancel = 0;
            string[] strPolNum = new string[6];
            int lngCnt = 0;
            bool blnOverride = false;
            string strStep = null;
            string strSourceData = null;
            IUimFieldDataContext NextField;

            //Initialize variables
	        blnOverride = false;

            Validation dataValidation = new Validation();

            CP.SiteID = dataContext.FindFieldDataContext("SITE_ID").Text.Trim();
            CP.WorkCategory = "CLM";
            CP.SiteRestriction = "T";

            Share.setDD_Running(true);
            lngSearchReturnValue = dataValidation.Validate(ref CP);
            Share.setDD_Running(false);

            if (!string.IsNullOrEmpty(CP.RejectCode))
            {
                dataContext.FindFieldDataContext("Rejected").SetValue("1");
                rejectReasonCode = CP.RejectCode;
                return;
            }

            if (lngSearchReturnValue == 0)
            {
                //pull back the form values

                dataContext.FindFieldDataContext("MANUAL_POLICY_NO1").SetValue(CP.PolicyNo);
                dataContext.FindFieldDataContext("MANUAL_PATIENT_NAME").SetValue(CP.InsuredName);
                dataContext.FindFieldDataContext("MANUAL_INSURED_NAME").SetValue(CP.InsuredName);
                dataContext.FindFieldDataContext("MANUAL_PATIENT_BIRTHDATE").SetValue(CP.BirthDate);
                dataContext.FindFieldDataContext("MANUAL_STATE").SetValue(CP.State);
                dataContext.FindFieldDataContext("MANUAL_ZIP").SetValue(CP.ZipCode);
                dataContext.FindFieldDataContext("MANUAL_PHONE").SetValue(CP.Phone);
                dataContext.FindFieldDataContext("MANUAL_INSURED_BIRTHDATE").SetValue(CP.BirthDate);
                dataContext.FindFieldDataContext("MANUAL_BUSINESS_AREA").SetValue(CP.BusinessArea);
                dataContext.FindFieldDataContext("MANUAL_WORK_TYPE").SetValue(CP.WorkType);
                dataContext.FindFieldDataContext("PLAN_CODE").SetValue(CP.PlanCode);
                dataContext.FindFieldDataContext("ISSUE_STATE").SetValue(CP.IssueState);
                dataContext.FindFieldDataContext("ADMIN_COMPANY_CODE").SetValue(CP.CompanyNo);
                dataContext.FindFieldDataContext("ADMIN_SYSTEM_ID").SetValue(CP.SystemNo);
                dataContext.FindFieldDataContext("SYSTEM_ID").SetValue(CP.SystemID);
                dataContext.FindFieldDataContext("COMPANY_CODE").SetValue(CP.CompanyCode);
                dataContext.FindFieldDataContext("VALIDATED").SetValue("(IA: MANUALVAL)" + "(" + Convert.ToString(CP.Validation) + ")");
                dataContext.FindFieldDataContext("Validation").SetValue("(IA: MANUALVAL)" + "(" + Convert.ToString(CP.Validation) + ")");


                if (CP.SortCode.Trim().Length == 0 || CP.SortCode.Trim() == "000")
                {
                    dataContext.FindFieldDataContext("SORT_CODE").SetValue("000");
                    dataContext.FindFieldDataContext("LOB").SetValue("MS");
                }
                else
                {
                    dataContext.FindFieldDataContext("SORT_CODE").SetValue(CP.SortCode);
                    dataContext.FindFieldDataContext("LOB").SetValue(CP.LineOfBusiness);
                }


                dataContext.FindFieldDataContext("P_SITE_ID").SetValue(CP.SiteID);
                dataContext.FindFieldDataContext("P_WORK_CATEGORY").SetValue(CP.WorkCategory);

                ValidateManualPVReturn(dataContext);

                //Determine source data value
	            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC") {
		            switch (dataContext.FindFieldDataContext("LOB").Text) {
			            case "MS":
				            strSourceData = "2034";
				            break;
			            case "OH":
				            strSourceData = "2024";
				            break;
			            case "LTC":
				            strSourceData = "2002";
				            break;
		            }

	            } else if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC") {
		            switch (dataContext.FindFieldDataContext("LOB").Text) {
			            case "MS":
				            strSourceData = "1935";
				            break;
			            case "OH":
				            strSourceData = "1936";
				            break;
		            }
	            }

                //'set the LOBSourceData value
		        dataContext.FindFieldDataContext("LOB_SOURCE_DATA").SetValue(strSourceData);

                //TODO
                /*'set validation as the next field
		        If CurrentField.Field.Name <> "Validation" Then
			        Set NextField = CurrentDocument.Fields("Validation")
			        SendKeys("~")
		        End If*/

            }
            else
            {
                DialogResult dRes = MessageBox.Show("No matching policy information was found." + System.Environment.NewLine +
                    "Click OK to continue searching, or Cancel to Exit to Form", "Policy information not found", MessageBoxButtons.OKCancel);
	            if (dRes == DialogResult.OK) {
		            LaunchValidationSearch(dataContext);
	            }

            }

            //'Additional logic for Chicago 
            if (!string.IsNullOrEmpty(dataContext.FindFieldDataContext("Validated").Text))
            {
                //If PolicyNumber is 9 digits and begins with 790 or smaller, RoutingCode value set to "10".
                if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "B" || dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                {
                    int result;
                    if (int.TryParse(dataContext.FindFieldDataContext("DatPolicyNumber").Text, out result) 
                        && Convert.ToInt32(dataContext.FindFieldDataContext("DatPolicyNumber").Text.Substring(0,3)) <= 790)
                    {
                        //Exclude policies starting with 2
                        if (Convert.ToInt32(dataContext.FindFieldDataContext("DatPolicyNumber").Text.Substring(0,1)) != 2)
                        {
                            dataContext.FindFieldDataContext("RoutingCode").SetValue("10");
                        }
                    }
                }
            }


        }

        private void ValidatePVReturn(IUimDataContext dataContext)
        {
            int lngLen;
            string strTemp1;
            string strTemp2;
            string strTemp3;
            string strTemp4;
            string strOutputMessage;

            try {

                foreach (IUimFieldDataContext field in dataContext.GetFieldDataContextArray())
                {
                    switch (field.Name)
                    {
                        case "AUTO_POLICY_NO1":
                            //field clean up
                            policyNumberCleanUp(field);

                            //CIG policy number format check
                            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
                            {
                                //Policy Number must be blank or 5 or 10 characters in length
                                lngLen = field.Text.Trim().Length;
                                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                                {
                                    string ruleFailMessage = "CIG Policy #'s must be blank or 6-10 chars";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
                                }
                            }

                            //BLC Policy Number format check
				            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                            {
                                //Policy Number should be no greater than 9 digits.
                                lngLen = field.Text.Trim().Length;
                                if (lngLen > 9)
                                {
                                    string ruleFailMessage = "BLC Policy #'s may not exceed 9 digits";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
                                }
                        

					            //Policy Number must be numeric
                                int result;
					            if (!int.TryParse(field.Text, out result) && lngLen > 0)
                                {
                                    string ruleFailMessage = "BLC Policy #'s must be numeric";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
                                }
                            }

                            break;
                        case "AUTO_PATIENT_NAME":
                            PatientName(dataContext);
                            break;
                        case "AUTO_INSURED_NAME":
                            PatientName(dataContext);
                            break;
                        case "AUTO_PATIENT_BIRTHDATE":
                            //Field cleanup
				            PatientBirthDate(dataContext);
				            //Format checking
				            if (field.Text.Trim() == "") 
                            {

                            }
    
				            //ElseIf Not IsNumeric(objDpField.Value) Then
				            //	strOutputMessage = "Patient Birthdate Must be numeric"
				            //	CurrentDocument.Fields("Validation").Value = strOutputMessage
				            //	CurrentDocument.Fields("Validation").OutputMessage = strOutputMessage
                            else if (field.Text.Length != 10)
                            {
                                string ruleFailMessage = "Patient Birthdate Must in the format of MM/DD/YYYY";
                                //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage);
                            }
                            else
                            {
                                //strTemp1 = Mid(objDpField.Value, 1, 2)
                                //strTemp2 = Mid(objDpField.Value, 3, 2)
                                //strTemp3 = Mid(objDpField.Value, 5, 4)

                                //If Not IsDate(strTemp1 + "/" + strTemp2 + "/" + strTemp3) Then
                                //	strOutputMessage = "Patient Birthdate - Invalid Date Format"
                                //	CurrentDocument.Fields("Validation").Value = strOutputMessage
                                //	CurrentDocument.Fields("Validation").OutputMessage = strOutputMessage
                                //End If

                            }
                            break;

                        case "AUTO_STATE":
                            PatientState(dataContext);

                            //Must be blank, or 2 alpha
                            if (field.Text.Trim().Length == 0 || field.Text.Trim().Length == 2) {
                                if (field.Text.Length == 2) {
                                    int result;
                                    if (int.TryParse(field.Text.Trim(), out result)) {
                                        string ruleFailMessage = "Patient State may not be numeric";
                                        //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                       // throw new Exception(ruleFailMessage);
                                    }

                                }
                            } else  {
                                string ruleFailMessage = "Patient State must be blank or 2 Alphanumeric characters";
                                //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage);
                            }
                            break;
                        case "AUTO_ZIP":
                            //Field cleanup
		                    PatientZip(dataContext);
		                    //Patient zip must be blank, or length of 5 or 9 numeric
		                    if (field.Text.Trim().Length == 5 || field.Text.Trim().Length == 9) {
			                    //make sure the value has no spaces
			                    field.SetValue(field.Text.Trim());
			                    int result;
                                if (!int.TryParse(field.Text.Trim(), out result)) {
				                    string ruleFailMessage = "Patient zip may only contain numbers";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
                                }
		                    } else {
			                    if (field.Text.Trim().Length != 0) {
				                    string ruleFailMessage = "Patient zip must be blank or of length 5 or 9 numeric";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
			                    }
                            }
                            break;

                        case "AUTO_PHONE":
                            //Field cleanup
				            PatientTelephone(dataContext);
				            //PatientTelephone must be blank, or length of 7 or 10 numeric
				            if (field.Text.Trim().Length == 7 || field.Text.Trim().Length == 10) {
                                long result;
                                if (!long.TryParse(field.Text.Trim(), out result))
                                {
						            string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
					            }
				            } else {
					            if (field.Text.Trim().Length != 0) {
						            string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
		 			            }
				            }
                            break;
                        case "AUTO_INSURED_BIRTHDATE":
                            //ensure fields are blank
			                strTemp1 = "";
			                strTemp2 = "";
			                strTemp3 = "";
			                //Field cleanup
			                InsuredBirthDate(dataContext);
			                //Format checking
			                if (field.Text.Trim() == "") {

                            }
			                //ElseIf Not IsNumeric(objDpField.Value) Then
			                //	strOutputMessage = "Must be numeric"
			                //	'objDpField.SetStatusError
			                //	objDpField.Value = ""
			                //	CurrentDocument.Fields("Validation").Value = strOutputMessage
			                //	CurrentDocument.Fields("Validation").OutputMessage = strOutputMessage
                            else if (field.Text.Length != 10) {
                                string ruleFailMessage = "Must be in the format of MM/DD/YYYY";
                                //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage);
                            } else {
				                //'strTemp1 = Mid(objDpField.Value, 1, 2)
				                //'strTemp2 = Mid(objDpField.Value, 3, 2)
				                //'strTemp3 = Mid(objDpField.Value, 5, 4)

				                //'If Not IsDate(strTemp1 + "/" + strTemp2 + "/" + strTemp3) Then
				                //'	strOutputMessage = "Invalid Date Format"
				                //'	'objDpField.SetStatusError
				                //'	'objDpField.Value = ""
				                //'	CurrentDocument.Fields("Validation").Value = strOutputMessage
				                //'	CurrentDocument.Fields("Validation").OutputMessage = strOutputMessage
				                //'End If
			                }
                            break;
                    }
                }
            } catch(Exception ex) {

                int errNum = 0;
                string batchName = "";

                ModCommon.WriteError(errNum, ex.Message, "ValidatePVReturn", batchName);

            }
        }

        private void ValidateManualPVReturn(IUimDataContext dataContext)
        {
            int lngLen;
            string strTemp1;
            string strTemp2;
            string strTemp3;
            string strTemp4;
            //string strOutputMessage;

            try {

                foreach (IUimFieldDataContext field in dataContext.GetFieldDataContextArray())
                {
                    switch (field.Name)
                    {
                        case "MANUAL_POLICY_NO1":
                            //field clean up
                            policyNumberCleanUp(field);

                            //CIG policy number format check
                            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "CIC")
                            {
                                //Policy Number must be blank or 5 or 10 characters in length
                                lngLen = field.Text.Trim().Length;
                                if (lngLen != 0 && (lngLen < 5 || lngLen > 10))
                                {
                                    
                                    //TODO test
                                    string ruleFailMessage = "CIG Policy #'s must be blank or 6-10 chars";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    Exception ex = new Exception(ruleFailMessage);
                                    field.SetValidationError(ex);
                                    //throw ex;
                                }
                            }

                            //BLC Policy Number format check
				            if (dataContext.FindFieldDataContext("SITE_ID").Text.Trim().ToUpper() == "BLC")
                            {
                                //Policy Number should be no greater than 9 digits.
                                lngLen = field.Text.Trim().Length;
                                if (lngLen > 9)
                                {
                                    //TODO
                                    string ruleFailMessage = "BLC Policy #'s may not exceed 9 digits";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    Exception ex = new Exception(ruleFailMessage);
                                    field.SetValidationError(ex);
                                    //throw ex;
                                }
                        

					            //Policy Number must be numeric
                                int result;
					            if (!int.TryParse(field.Text, out result) && lngLen > 0)
                                {
                                    //TODO
                                    string ruleFailMessage = "BLC Policy #'s must be numeric";
                                    //dataContext.FindFieldDataContext("Validation").SetValue(ruleFailMessage);
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    Exception ex = new Exception(ruleFailMessage);
                                    field.SetValidationError(ex);
                                    //throw ex;
                                }
                            }

                            break;
                        case "MANUAL_PATIENT_NAME":
                            PatientName(dataContext);
                            break;
                        case "MANUAL_INSURED_NAME":
                            PatientName(dataContext);
                            break;
                        case "MANUAL_PATIENT_BIRTHDATE":
                            //Field cleanup
				            PatientBirthDate(dataContext);
				            //Format checking
				            if (field.Text.Trim() == "") 
                            {

                            }
                            else if (field.Text.Length != 10)
                            {
                                field.SetValue("");
                                string ruleFailMessage = "Patient Birthdate Must in the format of MM/DD/YYYY";
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage); //TODO
                            }
                            else
                            {
                                //strTemp1 = Mid(objDpField.Value, 1, 2)
                                //strTemp2 = Mid(objDpField.Value, 3, 2)
                                //strTemp3 = Mid(objDpField.Value, 5, 4)

                                //If Not IsDate(strTemp1 + "/" + strTemp2 + "/" + strTemp3) Then
                                //	strOutputMessage = "Patient Birthdate - Invalid Date Format"
                                //	CurrentDocument.Fields("Validation").Value = strOutputMessage
                                //	CurrentDocument.Fields("Validation").OutputMessage = strOutputMessage
                                //End If

                            }
                            break;

                        case "MANUAL_STATE":
                            PatientState(dataContext);

                            //Must be blank, or 2 alpha
                            if (field.Text.Trim().Length == 0 || field.Text.Trim().Length == 2) {
                                if (field.Text.Length == 2) {
                                    int result;
                                    if (int.TryParse(field.Text.Trim(), out result)) {
                                        field.SetValue("");
                                        string ruleFailMessage = "Patient State may not be numeric";
                                        dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                        //throw new Exception(ruleFailMessage);
                                    }

                                }
                            } else  {
                                field.SetValue("");
                                string ruleFailMessage = "Patient State must be blank or 2 Alphanumeric characters";
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage);
                            }
                            break;
                        case "MANUAL_ZIP":
                            //Field cleanup
		                    PatientZip(dataContext);
		                    //Patient zip must be blank, or length of 5 or 9 numeric
		                    if (field.Text.Trim().Length == 5 || field.Text.Trim().Length == 9) {
			                    //make sure the value has no spaces
			                    field.SetValue(field.Text.Trim());
			                    int result;
                                if (!int.TryParse(field.Text.Trim(), out result)) {
                                    field.SetValue("");
				                    string ruleFailMessage = "Patient zip may only contain numbers";
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
                                }
		                    } else {
			                    if (field.Text.Trim().Length != 0) {
                                    field.SetValue("");
				                    string ruleFailMessage = "Patient zip must be blank or of length 5 or 9 numeric";
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
			                    }
                            }
                            break;

                        case "MANUAL_PHONE":
                            //Field cleanup
				            PatientTelephone(dataContext);
				            //PatientTelephone must be blank, or length of 7 or 10 numeric
				            if (field.Text.Trim().Length == 7 || field.Text.Trim().Length == 10) {
					            long result;
                                if (!long.TryParse(field.Text.Trim(), out result))
                                {
                                    field.SetValue("");
						            string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
					            }
				            } else {
					            if (field.Text.Trim().Length != 0) {
                                    field.SetValue("");
						            string ruleFailMessage = "PatientTelephone must be blank or of length 7 or 10 numeric";
                                    dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                    //throw new Exception(ruleFailMessage);
		 			            }
				            }
                            break;
                        case "MANUAL_INSURED_BIRTHDATE":
                            //ensure fields are blank
			                strTemp1 = "";
			                strTemp2 = "";
			                strTemp3 = "";
			                //Field cleanup
			                InsuredBirthDate(dataContext);
			                //Format checking
			                if (field.Text.Trim() == "") {

                            }
                            else if (field.Text.Length != 10) {
                                field.SetValue("");
                                string ruleFailMessage = "Must be in the format of MM/DD/YYYY";
                                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                                //throw new Exception(ruleFailMessage);
                            } else {
	
			                }
                            break;
                    }
                }
            } catch(Exception ex) {

                int errNum = 0;
                string batchName = "";

                ModCommon.WriteError(errNum, ex.Message, "ValidateManualPVReturn", batchName);

            }
        }

        private void policyNumberCleanUp(IUimFieldDataContext field)
        {
            if (field.Text.Trim().Length == 0)
            {
                return;
            }

            field.SetValue(field.Text.Replace('@', '0'));
            field.SetValue(field.Text.Replace('O', '0'));
            field.SetValue(field.Text.Replace('o', '0'));

            if (field.UimDataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "BLC")
            {
                field.SetValue(ModCommon.removeAlphaNumeric(field.Text));
            }

            if (field.UimDataContext.FindFieldDataContext("SITE_ID").Text.Trim() == "CIC")
            {
                if (field.Text.Length >= 2) {

                    if (field.Text.Substring(0, 2).ToUpper() == "PI" || field.Text.Substring(0, 2).ToUpper() == "P1" || field.Text.Substring(0, 2).ToUpper() == "PL")
                    {
                        field.SetValue("PL" + field.Text.Substring(2, field.Text.Length - 2));
                    }
                }
            }

            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            field.SetValue(field.Text.Replace(" ", ""));

        }

        private void InsuredName(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField;
 
            localDpDocField = dataContext.FindFieldDataContext("OCR_INSURED_NAME");

            localDpDocField.SetValue(localDpDocField.Text.Replace("\r", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace("\t", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace("\n", " "));

            //translate all @ to 0s
            localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));

            //names may have commas, so convert commas to spaces and remove double space
            localDpDocField.SetValue(localDpDocField.Text.Replace(",", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

            //Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

            //Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());
        }

        private void ProviderTaxID(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField;

            localDpDocField = dataContext.FindFieldDataContext("OCR_PROVIDER_TAX_ID");

            //translate all @ to 0s
            localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));
            //Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));
            //Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());

            localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

            if (localDpDocField.Text.Length >= 7)
            {
                if (ModCommon.ParseParam(localDpDocField.Text, 1, " ").Length == 2)
                {
                    localDpDocField.SetValue(localDpDocField.Text.Replace(" ", ""));
                }
            }

        }

        private void InsuredBirthDate(IUimDataContext dataContext)
        {

            //Variable declaration
            IUimFieldDataContext localDpDocField;
            string strTemp1;
            string strTemp2;
            string strTemp3;
            string strTemp4;

            //'Initialize variables
            localDpDocField = dataContext.FindFieldDataContext("OCR_INSURED_BIRTHDATE");

	        //debug=on

	        //'translate all @ to 0s
	        localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));
	        //'translate all Ss to 5s on Date fields
	        localDpDocField.SetValue(localDpDocField.Text.Replace("S", "5"));
	        //'Remove all punctuation marks
	        localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));
            //'Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());
            //'Remove slashes
            localDpDocField.SetValue(localDpDocField.Text.Replace( "/", " "));
            //'Remove dashes
            localDpDocField.SetValue(localDpDocField.Text.Replace( "-", " "));
            //'Remove Single spaces
            localDpDocField.SetValue(localDpDocField.Text.Replace( " ", ""));
            //'Remove Double spaces
            localDpDocField.SetValue(localDpDocField.Text.Replace( "  ", " "));

            if (localDpDocField.Text == "")
            {
                return;
            }

            //'Add spaces in correct locations
            if (localDpDocField.Text.Length < 8)
            {
                return;
            }
            localDpDocField.SetValue( localDpDocField.Text.Substring(0,2) + " " + localDpDocField.Text.Substring(2,2) + " " + localDpDocField.Text.Substring(4, 4)); //fix

            //'Format the date
            strTemp1 = ModCommon.ParseParam(localDpDocField.Text, 1, " ");
            strTemp2 = ModCommon.ParseParam(localDpDocField.Text, 2, " ");
            strTemp3 = ModCommon.ParseParam(localDpDocField.Text, 3, " ");

            if (strTemp1 != "" && strTemp2 != "" && strTemp3 != "")
            {// If strTemp1 > "" And strTemp2 > "" And strTemp3 > "" Then
                if (strTemp1.Length == 1)
                {// If Len(strTemp1) = 1 Then
                    strTemp1 = "0" + strTemp1;// strTemp1 = "0" + strTemp1
                }
                else if (strTemp1.IndexOf("8") == 0)
                {
                    strTemp1 = "0" + strTemp1.Substring(1, 1);
                }
                else if (strTemp1.IndexOf("6") == 0)
                {
                    strTemp1 = "0" + strTemp1.Substring(1, 1);
                }

                if (strTemp2.Length == 1)
                {// If Len(strTemp1) = 1 Then
                    strTemp2 = "0" + strTemp2;// strTemp1 = "0" + strTemp1
                }
                else if (strTemp2.IndexOf("8") == 0)
                {
                    strTemp2 = "0" + strTemp2.Substring(1, 1);
                }
                else if (strTemp2.IndexOf("6") == 0)
                {
                    strTemp2 = "0" + strTemp2.Substring(1, 1);
                }

                //Format four digit year
                if (strTemp3.Length == 2)
                {
                    strTemp4 = DateTime.Now.ToString("yyyy");
                    if (int.Parse("20" + strTemp3) > int.Parse(strTemp4))
                    {
                        strTemp3 = "19" + strTemp3;
                    }
                    else
                    {
                        strTemp3 = "20" + strTemp3;
                    }
                }
            }


                
	        //Concatenate date fields
	        if (strTemp1 == "" || strTemp2 == "" || strTemp3 == "") {
		        localDpDocField.SetValue("");
	        } else {
		        localDpDocField.SetValue(strTemp1 + "/" + strTemp2 + "/" + strTemp3);
	        }
             
             

        }

        private void PatientTelephone(IUimDataContext dataContext)
        {
            //Variable declaration
            IUimFieldDataContext localDpDocField;

            //Initialize variables
            localDpDocField = dataContext.FindFieldDataContext("OCR_PHONE");

            //'translate all @ to 0s
            localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));

            //'Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

            //'Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());

            //Remove Double spaces if any
            localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

        }

        private void PatientZip(IUimDataContext dataContext)
        {
            //Variable declaration
            IUimFieldDataContext localDpDocField;

            //Initialize variables
            localDpDocField = dataContext.FindFieldDataContext("OCR_ZIP");

            //'translate all @ to 0s
            localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));

            //'Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

            //'Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());

            //Remove Double spaces if any
            localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

            //'make sure the zip code is only the first 5 characters
            try
            {
                localDpDocField.SetValue(localDpDocField.Text.Substring(0, 5));
            }
            catch (Exception ex)
            {

            }
        }

        private void PatientState(IUimDataContext dataContext)
        {


            //Variable declaration
            IUimFieldDataContext localDpDocField;

	        //Initialize variables
	        localDpDocField = dataContext.FindFieldDataContext("OCR_STATE");

	        //'translate all @ to 0s
	        localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));

	        //'Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

            //'Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());

	        //Remove Double spaces if any
            localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));
        }

        private void PatientBirthDate(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField = null;
            string strTemp;// As String
            string strTemp1;// As String
            string strTemp2;// As String
            string strTemp3;// As String
            string strTemp4;// As String

            //Initialize variables
	        if (extraction) {
                if (dataContext.FindFieldDataContext("OCR_PATIENT_BIRTHDATE").Text != "") {
                    localDpDocField = dataContext.FindFieldDataContext("OCR_PATIENT_BIRTHDATE");
		        } else {//'If CurrentDocument.Fields("AUTO_PATIENT_BIRTHDATE").Value <> "" Then
			        localDpDocField = dataContext.FindFieldDataContext("AUTO_PATIENT_BIRTHDATE");
		        //Else
		        //	Set localDpDocField = CurrentDocument.Fields("AUTO_PATIENT_BIRTHDATE")
		        }
	        } else {
                //If CurrentDocument.Fields("MANUAL_PATIENT_BIRTHDATE").Value <> "" Then
			        localDpDocField = dataContext.FindFieldDataContext("MANUAL_PATIENT_BIRTHDATE");
		        //'Else
		        //'	Set localDpDocField = CurrentDocument.Fields("MANUAL_PATIENT_BIRTHDATE")
		        //'End If
	        }

            //'debug=on

	        //'translate all @ to 0s
	        localDpDocField.SetValue(localDpDocField.Text.Replace("@", "0"));
	        //translate all Ss to 5s on Date fields
            localDpDocField.SetValue(localDpDocField.Text.Replace( "S", "5"));
	        //'Remove all punctuation marks
            localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));
            localDpDocField.SetValue(localDpDocField.Text.Replace("/", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace("-", " "));
	        //'Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());
	        //'Remove Single spaces
            localDpDocField.SetValue(localDpDocField.Text.Replace(" ", ""));
	        //'Remove Double spaces
	        localDpDocField.SetValue(localDpDocField.Text.Replace("  ", " "));

            if (localDpDocField.Text.Trim().Length < 8)
            {
                return;
            }

	        //'Add spaces in correct locations
	        localDpDocField.SetValue(localDpDocField.Text.Substring(0,2) + " " + localDpDocField.Text.Substring(2, 2) + " " + localDpDocField.Text.Substring(4, 4));

	        //'Format the date before remove space
	        strTemp1 = ModCommon.ParseParam(localDpDocField.Text, 1, " ");
	        strTemp2 = ModCommon.ParseParam(localDpDocField.Text, 2, " ");
	        strTemp3 = ModCommon.ParseParam(localDpDocField.Text, 3, " ");

            if (strTemp1 != "" && strTemp2 != "" && strTemp3 != "") {

		        if (strTemp1.Length == 1) {
			        strTemp1 = "0" + strTemp1;
		        } else if (strTemp1.IndexOf('8') == 0) { //??
			        strTemp1 = "0" + strTemp1.Substring(1, 1);
		        } else if (strTemp1.IndexOf('6') == 0) {
			        strTemp1 = "0" + strTemp1.Substring(1, 1);
		        }

		        if (strTemp2.Length == 1) {
			        strTemp2 = "0" + strTemp2;
		        } else if (strTemp1.IndexOf('8') == 0) { //??
			        strTemp2 = "0" + strTemp2.Substring(1, 1);
		        } else if (strTemp1.IndexOf('6') == 0) {
			        strTemp2 = "0" + strTemp2.Substring(1, 1);
		        }
               
		        //'Format four digit year
		        if (strTemp3.Length == 2) {
                    strTemp4  = DateTime.Now.ToString("yyyy");
                    if (int.Parse("20" + strTemp3) > int.Parse(strTemp4)) {
                        strTemp3 = "19" + strTemp3;
                    } else {
                        strTemp3 = "20" + strTemp3;
                    }
                }
	        }


            if (strTemp1 == "" || strTemp2 == "" || strTemp3 == "") {
                localDpDocField.SetValue("");
            } else {
                //'Concatenate date fields
                localDpDocField.SetValue(strTemp1 + "/" + strTemp2 + "/" + strTemp3);
            }

            if (localDpDocField.Text != "") {
                if (extraction == true) {
                    dataContext.FindFieldDataContext("AUTO_PATIENT_BIRTHDATE").SetValue(localDpDocField.Text);
                } else {
                    dataContext.FindFieldDataContext("MANUAL_PATIENT_BIRTHDATE").SetValue(localDpDocField.Text);
                }
            }
            
        }

        private void PatientName(IUimDataContext dataContext)
        {
            IUimFieldDataContext localDpDocField;

            //Initialize variables
	        localDpDocField = dataContext.FindFieldDataContext("OCR_PATIENT_NAME");

	        //debug = on

	        //Data Cleanup
	        //Replace CR, LF and HT with spaces
	        localDpDocField.SetValue(localDpDocField.Text.Replace( "\r", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace( "\t", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace( "\n", " "));

	        //translate all @ to 0s
	        localDpDocField.SetValue(localDpDocField.Text.Replace( "@", "0"));

	        //names may have commas, so convert commas to spaces and remove double space
            localDpDocField.SetValue(localDpDocField.Text.Replace( ",", " "));
            localDpDocField.SetValue(localDpDocField.Text.Replace( "  ", " "));

	        //Remove all punctuation marks
	        localDpDocField.SetValue(ModCommon.RemovePunctuation(localDpDocField.Text));

	        //Trim all leading and trailing spaces
            localDpDocField.SetValue(localDpDocField.Text.Trim());


        }

        private void RoutingCode(IUimDataContext dataContext)
        {
            IUimFieldDataContext field = dataContext.FindFieldDataContext("ROUTE_CODE");

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            //Trim all leading and trailing spaces
            field.SetValue(field.Text.Trim());

            //Remove double spaces if any
            field.SetValue(field.Text.Replace("  ", " "));

        }

        private void LineOfBusiness(IUimDataContext dataContext)
        {
            IUimFieldDataContext field = dataContext.FindFieldDataContext("LOB");

            //translate all @ to 0s
            field.SetValue(field.Text.Replace('@', '0'));

            //remove all punctuation marks
            field.SetValue(ModCommon.RemovePunctuation(field.Text));

            //Trim all leading and trailing spaces
            field.SetValue(field.Text.Trim());

            //Remove double spaces if any
            field.SetValue(field.Text.Replace("  ", " "));

        }

        private string FormatDate(string date)
        {
            string strTemp;

            if (date.Length == 8)
            {
                strTemp = date.Substring(0, 2);
                strTemp = strTemp + "/" + date.Substring(2, 2);
                strTemp = strTemp + "/" + date.Substring(4, 4);
                return strTemp;
            }
            return date;
        }

    }
}
