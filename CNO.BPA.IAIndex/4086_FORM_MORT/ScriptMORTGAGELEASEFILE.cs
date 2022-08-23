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

    public class ScriptMORTGAGELEASEFILE : UimScriptDocument
    {
        private const string _validationRule = "One";

        public ScriptMORTGAGELEASEFILE()
            : base()
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

            //AutoValidation(dataContext);
        }

        


        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
        {
            //AT THE END CHECK IF VALIDATION ARE SUCCESFUL OR NOT ..cap locks
           /* if (dataContext.FindFieldDataContext("VALIDATED").Text != "SUCCESS")
            {
                string ruleFailMessage = "Not Validated";
                dataContext.SetValidationRuleFailMessage(_validationRule, ruleFailMessage);
                throw new Exception(ruleFailMessage);
            }*/
        }

        
       
        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                int intLease;
                int intNotCategorized;
                int intSelectedItems;
                int ReturnValue;

                //parse the barcode and assign values to respective fields
                ParseBarCode(dataContext.FindFieldDataContext("BARCODE").Text, dataContext);

                //Set the FileNetDocument Class value for Private Placement
                dataContext.FindFieldDataContext("F_DOCCLASSNAME").SetValue("Mortgage");


                //create and set an object for LoanSearch
                CNO.BPA.Validation4086.LoanSearch obj4086val = new CNO.BPA.Validation4086.LoanSearch();
                //create and set an object for CommonParameters
                CNO.BPA.DataHandler.CommonParameters cpDH = new CNO.BPA.DataHandler.CommonParameters();

                //set input parameters to lookup for values based on LOAN_NUMBER and PROPERTY_VALUE number - datavalidation/4086 dll
                cpDH.AccountNumber = dataContext.FindFieldDataContext("ACCOUNT_NUMBER").Text;
                cpDH.Address1 = dataContext.FindFieldDataContext("PROPERTY_ADDRESS").Text;

                //call loanvalidation function and fetch values into data handler object
                ReturnValue = obj4086val.loanValidation(ref cpDH);

                //fetch the data elements from data handler object and set values for the index family variables
                if (ReturnValue == 0)
                {
                    dataContext.FindFieldDataContext("ACCOUNT_NUMBER").SetValue(cpDH.AccountNumber.Trim());
                    dataContext.FindFieldDataContext("BORROWER_NAME").SetValue(cpDH.LastName.Trim());
                    dataContext.FindFieldDataContext("PROPERTY_NAME").SetValue(cpDH.FullName.Trim());
                    dataContext.FindFieldDataContext("PROPERTY_ADDRESS").SetValue(cpDH.Address1.Trim());
                    dataContext.FindFieldDataContext("PROPERTY_CITY").SetValue(cpDH.City.Trim());
                    dataContext.FindFieldDataContext("PROPERTY_STATE").SetValue(cpDH.State.Trim());
                    dataContext.FindFieldDataContext("FULL_NAME").SetValue(cpDH.FullName.Trim());
                }
                else
                {
                    //commented out in as-is process code
                    //dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    ReturnValue = 0;
                }

                //validate if only one field among LEASE, NOT_CATEGORIZED is selected
                //The interger variable would be assigned with 0 if the value fetched is null otherWISE it will be 1
                if (dataContext.FindFieldDataContext("LEASE").Text.Trim().Length == 0)
                {
                    intLease = 0;
                }
                else
                {
                    intLease = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("LEASE");
                    dataContext.FindFieldDataContext("DESCRIPTION2").SetValue("LEASE FILE SEC");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("LEASE").Text, 1, "-").Replace("\r", "").Trim()); 
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
                    dataContext.FindFieldDataContext("DESCRIPTION2").SetValue("NOT_CATEGORIZED");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue("NCD");
                }

                intSelectedItems = intLease + intNotCategorized;

                if (intSelectedItems > 1)
                {
                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("");
                    dataContext.FindFieldDataContext("DESCRIPTION2").SetValue("");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue("");
                }

                //check if only one selection is made and CUSIP number is not null
                if (ReturnValue == 0 && intSelectedItems == 1 && dataContext.FindFieldDataContext("ACCOUNT_NUMBER").Text.Trim().Length > 0 && dataContext.FindFieldDataContext("PROPERTY_ADDRESS").Text.Trim().Length > 0 && dataContext.FindFieldDataContext("DOCUMENT_DATE").Text.Trim().Length > 0)
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
            DateTime dateObject = Convert.ToDateTime(ModCommon.ParseParam(BARCODE, 13, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("DOCUMENT_DATE").SetValue(dateObject.ToString("yyyy/MM/dd"));//TEST
            dataContext.FindFieldDataContext("ACCOUNT_NUMBER").SetValue(ModCommon.ParseParam(BARCODE, 14, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("BORROWER_NAME").SetValue(ModCommon.ParseParam(BARCODE, 15, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_NAME").SetValue(ModCommon.ParseParam(BARCODE, 16, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_ADDRESS").SetValue(ModCommon.ParseParam(BARCODE, 17, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_CITY").SetValue(ModCommon.ParseParam(BARCODE, 20, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_STATE").SetValue(ModCommon.ParseParam(BARCODE, 21, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("FULL_NAME").SetValue(ModCommon.ParseParam(BARCODE,22, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("LEASE").SetValue(ModCommon.ParseParam(BARCODE, 12, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("NOT_CATEGORIZED").SetValue(ModCommon.ParseParam(BARCODE, 18, "|").Replace("\r", "").Trim());
        }

    }
}
