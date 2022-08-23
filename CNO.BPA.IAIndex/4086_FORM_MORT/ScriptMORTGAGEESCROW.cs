﻿using System;
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

    public class ScriptMORTGAGEESCROW : UimScriptDocument
    {
        private const string _validationRule = "One";

        public ScriptMORTGAGEESCROW()
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
            /*
            if (dataContext.FindFieldDataContext("VALIDATED").Text != "SUCCESS")
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
                int intInsurance;
                int intRealEstateTax;
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

                //validate if only one field among INSURNACE, REAL ESTATE TAX, NOT_CATEGORIZED is selected
                //The interger variable would be assigned with 0 if the value fetched is null otherWISE it will be 1
                if (dataContext.FindFieldDataContext("INSURANCE").Text.Trim().Length == 0)
                {
                    intInsurance = 0;
                }
                else
                {
                    intInsurance = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("INSURANCE");
                    dataContext.FindFieldDataContext("DESCRIPTION2").SetValue("ESCROW FILE SEC");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("INSURANCE").Text, 1, "-").Replace("\r", "").Trim()); 
                }
                //REAL_ESTATE_TAX
                if (dataContext.FindFieldDataContext("REAL_ESTATE_TAX").Text.Trim().Length == 0)
                {
                    intRealEstateTax = 0;
                }
                else
                {
                    intRealEstateTax = 1;

                    dataContext.FindFieldDataContext("DESCRIPTION").SetValue("REAL ESTATE TAX");
                    dataContext.FindFieldDataContext("DESCRIPTION2").SetValue("ESCROW FILE SEC");
                    dataContext.FindFieldDataContext("DOCUMENT_TYPE").SetValue(ModCommon.ParseParam(dataContext.FindFieldDataContext("REAL ESTATE TAX").Text, 1, "-").Replace("\r", "").Trim()); 
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

                intSelectedItems = intInsurance + intRealEstateTax + intNotCategorized;

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
            DateTime dateObject = Convert.ToDateTime(ModCommon.ParseParam(BARCODE, 14, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("DOCUMENT_DATE").SetValue(dateObject.ToString("yyyy/MM/dd"));//TEST
            dataContext.FindFieldDataContext("ACCOUNT_NUMBER").SetValue(ModCommon.ParseParam(BARCODE, 15, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("BORROWER_NAME").SetValue(ModCommon.ParseParam(BARCODE, 16, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_NAME").SetValue(ModCommon.ParseParam(BARCODE, 17, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_ADDRESS").SetValue(ModCommon.ParseParam(BARCODE, 19, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_CITY").SetValue(ModCommon.ParseParam(BARCODE, 22, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("PROPERTY_STATE").SetValue(ModCommon.ParseParam(BARCODE, 23, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("FULL_NAME").SetValue(ModCommon.ParseParam(BARCODE,24, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("INSURANCE").SetValue(ModCommon.ParseParam(BARCODE, 13, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("REAL_ESTATE_TAX").SetValue(ModCommon.ParseParam(BARCODE, 18, "|").Replace("\r", "").Trim());
            dataContext.FindFieldDataContext("NOT_CATEGORIZED").SetValue(ModCommon.ParseParam(BARCODE, 20, "|").Replace("\r", "").Trim());
        }

      
    }
}
