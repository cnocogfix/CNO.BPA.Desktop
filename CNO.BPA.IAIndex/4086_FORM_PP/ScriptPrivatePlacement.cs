using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
using CNO.BPA.DataDirector;
using CNO.BPA.DataValidation;
using CNO.BPA.Validation4086;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptPrivatePlacement : UimScriptDocument
    {
        private const string _validationRule = "One";
        //private string ddrunning;

        int _docNodeId;
        IUimFieldDataContext _valField;
        IUimDataContext _dataContext;

        //private Dictionary<string, string> values;
        DocumentNode docNode;

        public ScriptPrivatePlacement()
            : base()
        {

        }

        public void DocumentLoad(IUimDataContext dataContext)
        {

            dataContext.TaskFinishOnErrorNotAllowed = true;
            _dataContext = dataContext;

            //MessageBox.Show("DocumentLoad event!!!");

            try
            {

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

        
        public void FormLoad(IUimDataEntryFormContext form)
        {

        }

        public void DocumentExtracted(IUimDataContext dataContext) {

            //Set OperatorID
            if (Share.getOperatorID() == "*")
            {
                dataContext.FindFieldDataContext("Operator").SetValue(ModCommon.getOperatorID());
            }
            else
            {
                dataContext.FindFieldDataContext("Operator").SetValue(Share.getOperatorID());
            }

           // AutoValidation(dataContext);
        }

   
    
        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            //AT THE END CHECK IF VALIDATION ARE SUCCESFUL OR NOT ..cap locks
            /*if (dataContext.FindFieldDataContext("VALIDATED").Text != "SUCCESS")
            {
                string ruleFailMessage = "Not Validated";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }*/
            
        }

        



        //everything found in this region was written by Nick for Validation Scripting
        //IUimDataContext _dataContext;
   

        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                int intOffering;
                int intFinancial;
                int intLegal;
                int intNotCategorized;
                int intSelectedItems;
                int ReturnValue;

                //parse the barcode and assign values to respective fields
                ParseBarCode(dataContext.FindFieldDataContext("BARCODE").Text, dataContext);

                //Set the FileNetDocument Class value for Private Placement
                dataContext.FindFieldDataContext("F_DOCCLASSNAME").SetValue("Private_Placement");

                //Set the Issue Id value
                dataContext.FindFieldDataContext("DESCRIPTION2").SetValue(dataContext.FindFieldDataContext("ISSUER_ID").Text.Replace("\r", ""));

                //create and set an object for LoanSearch
                CNO.BPA.Validation4086.CusipSearch obj4086val = new CNO.BPA.Validation4086.CusipSearch();
                //create and set an object for CommonParameters
                CNO.BPA.DataHandler.CommonParameters cpDH = new CNO.BPA.DataHandler.CommonParameters();

                //set input parameters to lookup for values based on CUSIP number - datavalidation/4086 dll
                cpDH.AccountNumber = dataContext.FindFieldDataContext("ACCOUNT_NUMBER").Text;

                //call loanvalidation function and fetch values into data handler object
                ReturnValue = obj4086val.ValidateCUSIP(ref cpDH);

                //fetch the data elements from data handler object and set values for the index family variables
                if (ReturnValue == 0)
                {
                    dataContext.FindFieldDataContext("ACCOUNT_NUMBER").SetValue(cpDH.AccountNumber);
                    dataContext.FindFieldDataContext("VENDOR_NAME").SetValue(cpDH.VendorName);
                    dataContext.FindFieldDataContext("BORROWER_NAME").SetValue(cpDH.FullName);
                    dataContext.FindFieldDataContext("ISSUER_ID").SetValue(cpDH.Description2);
                }
                else
                {
                    ReturnValue = 0;
                }

                //validate if only one field among OFFERING< FINANCIAL< LEGAL< NOT_CATEGORIZED is selected
                //The interger variable would be assigned with 0 if the value fetched is null other it will be 1
                if (dataContext.FindFieldDataContext("OFFERING").Text.Trim().Length == 0)
                {
                    intOffering = 0;
                }
                else
                {
                    intOffering = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("OFFERING");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("OFFERING").Text, 1, "-").Replace("\r", "")); 
                }
                //FINANCIAL
                if (dataContext.FindFieldDataContext("FINANCIAL").Text.Trim().Length == 0)
                {
                    intFinancial = 0;
                }
                else
                {
                    intFinancial = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("FINANCIAL");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("FINANCIAL").Text, 1, "-").Replace("\r", ""));
                }
                //LEGAL
                if (dataContext.FindFieldDataContext("LEGAL").Text.Trim().Length == 0)
                {
                    intLegal = 0;
                }
                else
                {
                    intLegal = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("LEGAL");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("LEGAL").Text, 1, "-").Replace("\r", ""));
                }
                //NOT_CATEGORIZED
                if (dataContext.FindFieldDataContext("NOT_CATEGORIZED").Text.Trim().Length == 0)
                {
                    intNotCategorized = 0;
                }
                else
                {
                    intNotCategorized = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("NOT_CATEGORIZED");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue("NCD");
                }

                intSelectedItems = intOffering + intFinancial + intLegal + intNotCategorized;

                if (intSelectedItems > 1)
                {
                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue("");
                }

                //check if only one selection is made and CUSIP number is not null
                if (ReturnValue == 0 && intSelectedItems == 1 && dataContext.FindFieldDataContext("ACCOUNT_NUMBER").Text.Trim().Length > 0 && dataContext.FindFieldDataContext("DOCUMENT_DATE").Text.Trim().Length > 0)
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
                int errNum = 0;
                string batchName = "";
                ModCommon.WriteError(errNum, ex.Message, "AutoValidation", batchName);
                //not sure how to set output message or setstatuserror
            }

        }

        private void ParseBarCode(string BARCODE, IUimDataContext dataContext)
        {
            //extract field values from BARCODE variable
            DateTime dateObject = Convert.ToDateTime(ModCommon.ParseParam(BARCODE, 12, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("DOCUMENT_DATE").SetValue(dateObject.ToString("yyyy/MM/dd"));//TEST
            dataContext.FindFieldDataContext("VENDOR_NAME").SetValue(ModCommon.ParseParam(BARCODE, 13, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("BORROWER_NAME").SetValue(ModCommon.ParseParam(BARCODE, 14, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("ACCOUNT_NUMBER").SetValue(ModCommon.ParseParam(BARCODE, 15, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("ISSUER_ID").SetValue(ModCommon.ParseParam(BARCODE, 18, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("OFFERING").SetValue(ModCommon.ParseParam(BARCODE, 11, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("FINANCIAL").SetValue(ModCommon.ParseParam(BARCODE, 17, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("LEGAL").SetValue(ModCommon.ParseParam(BARCODE,16, "|").Replace("\r", ""));
            dataContext.FindFieldDataContext("NOT_CATEGORIZED").SetValue(ModCommon.ParseParam(BARCODE, 19, "|").Replace("\r", ""));
        }

  
    }
}
