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

    public class ScriptCPLSURVEYFORM : UimScriptDocument
    {
        private const string _validationRule = "One";
 
        public ScriptCPLSURVEYFORM()
            : base()
        {
         
        }
        

        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
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

            //validateSelection(dataContext);
            //AutoValidation(dataContext);
        }

        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                string strOCRFormName;
                long lngRetValTelesales;
                long lngRetValCustomer;

                //Correspondence Indicator
                if (dataContext.FindFieldDataContext("AUTO_CORRESPONDENCE_INDICATOR").Text != "")
                {
                    dataContext.FindFieldDataContext("AUTO_CORRESPONDENCE_INDICATOR").SetValue("Y");
                }
                else
                {
                    dataContext.FindFieldDataContext("AUTO_CORRESPONDENCE_INDICATOR").SetValue("N");
                }

                strOCRFormName = dataContext.FindFieldDataContext("OCR_FORM_NAME").Text.Trim();

                lngRetValTelesales = strOCRFormName.IndexOf("TELESALES SURVEY");
                lngRetValCustomer = strOCRFormName.IndexOf("CUSTOMER SERVICE SURVEY");

                if(lngRetValTelesales >= 0 && lngRetValCustomer == -1)
                {
                    dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("CPLDTML");
                }
                else if (lngRetValTelesales == -1 && lngRetValCustomer >= 0)
                {
                    dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("CPLDTSL");
                }
                else
                {
                    dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("");
                }

                if (dataContext.FindFieldDataContext("AUTO_MEMBER_NO").Text != "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "" && dataContext.FindFieldDataContext("AUTO_DIRECTORY_NO").Text != "" && dataContext.FindFieldDataContext("AUTO_PROFESSIONAL").Text != "" && dataContext.FindFieldDataContext("AUTO_COURTEOUS").Text != "" && dataContext.FindFieldDataContext("AUTO_KNOWLEDGEABLE").Text != "" && dataContext.FindFieldDataContext("AUTO_THOROUGH").Text != "" && dataContext.FindFieldDataContext("AUTO_QUICK").Text != "" && dataContext.FindFieldDataContext("AUTO_LIKELY").Text != "" && dataContext.FindFieldDataContext("AUTO_CORRESPONDENCE_INDICATOR").Text != "" && dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").Text != "")
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
                //TODO this is what the as-is code looks like
                /*     Dim lngErrNum    As Long
                Dim strErrDesc   As String

                lngErrNum = Err.Number
                strErrDesc = Err.Description

                'Call WriteError(lngErrNum, strErrDesc, "AutoValidation")
                CurrentDocument.Fields("Validation").OutputMessage = strErrDesc
                CurrentDocument.Fields("Validation").SetStatusError*/
                int errNum = 0;
                ModCommon.WriteError(errNum, ex.Message.ToString(), "AutoValidation", "");
                //not sure how to set output message or setstatuserror
            }
        }

        private void validateSelection(IUimDataContext dataContext)
        {
            try
            {

                string question6;
                string selectedItem;
                bool blnItemSelected;
                int intQuestion1SelectionItems;
                int intQuestion2SelectionItems; 
                int intQuestion3SelectionItems;
                int intQuestion4SelectionItems;
                int intQuestion5SelectionItems;
                int intQuestion6SelectionItems;

                //there are 5 questions with the same conditions, created a loop to achieve this procedure, 6TH question is a special case and is not in the loop
                for (int number = 1; number <= 6; number++)
                {
                    string value = "";
                    if (dataContext.FindFieldDataContext("Q" + number.ToString() + "_NOT").Text == "1")
                    {
                        value = "4";
                    }
                    else if (dataContext.FindFieldDataContext("Q" + number.ToString() + "_SOMEWHATNOT").Text == "1")
                    {
                        value = "3";
                    }
                    else if (dataContext.FindFieldDataContext("Q" + number.ToString() + "_SOMEWHAT").Text == "1")
                    {
                        value = "2";
                    }
                    else if (dataContext.FindFieldDataContext("Q" + number.ToString() + "_VERY").Text == "1")
                    {
                        value = "1";
                    }

                    switch (number)
                    {//switch statement to write the values to their respective fields
                        case 1:
                            dataContext.FindFieldDataContext("AUTO_PROFESSIONAL").SetValue(value);
                            break;
                        case 2:
                            dataContext.FindFieldDataContext("AUTO_COURTEOUS").SetValue(value);
                            break;
                        case 3: 
                            dataContext.FindFieldDataContext("AUTO_KNOWLEDGEABLE").SetValue(value);
                            break;
                        case 4:
                            dataContext.FindFieldDataContext("AUTO_THOROUGH").SetValue(value);
                            break;
                        case 5:
                            dataContext.FindFieldDataContext("AUTO_QUICK").SetValue(value);
                            break;
                    }
                }

                //question 6 is a special case so I left it out of the loop
                if (dataContext.FindFieldDataContext("Q6_VERYUNLIKELY").Text == "1")
                {
                    question6 = "4";
                }
                else if (dataContext.FindFieldDataContext("Q6_SOMEWHATUNLIKELY").Text == "1")
                {
                    question6 = "3";
                }
                else if (dataContext.FindFieldDataContext("Q6_LIKELY").Text == "1")
                {
                    question6 = "2";
                }
                else if (dataContext.FindFieldDataContext("Q6_VERY").Text == "1")
                {
                    question6 = "1";
                }
                else
                {
                    question6 = "";
                }
                dataContext.FindFieldDataContext("AUTO_LIKELY").SetValue(question6);
            }
            catch (Exception ex)
            {
                //TODO this is what the as-is code looks like
                /*     Dim lngErrNum    As Long
                Dim strErrDesc   As String

                lngErrNum = Err.Number
                strErrDesc = Err.Description

                'Call WriteError(lngErrNum, strErrDesc, "AutoValidation")
                CurrentDocument.Fields("Validation").OutputMessage = strErrDesc
                CurrentDocument.Fields("Validation").SetStatusError*/
                int errNum = 0;
                ModCommon.WriteError(errNum, ex.Message.ToString(), "AutoValidation", "");
                //not sure how to set output message or setstatuserror
            }
        }
   
    }
}
