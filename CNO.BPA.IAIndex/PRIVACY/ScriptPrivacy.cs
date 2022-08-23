using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using CNO.BPA.DataHandler;
//using CNO.BPA.DataDirector;
//using CNO.BPA.DataValidation;
using CNO.BPA.PrivacyMailingValidation;

namespace Custom.InputAccel.UimScript
{
    using Emc.InputAccel.CaptureClient;
    using Emc.InputAccel.UimScript;

    public class ScriptPrivacy : UimScriptDocument
    {
        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";

        
        public ScriptPrivacy() : base()
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

            AutoValidate(dataContext);
        }

    

        private void AutoValidate(IUimDataContext dataContext)
        {
            try
            {
                CNO.BPA.DataHandler.CommonParameters cpDH = new CNO.BPA.DataHandler.CommonParameters();
                CNO.BPA.PrivacyMailingValidation.PrivacyValidation privVal = new CNO.BPA.PrivacyMailingValidation.PrivacyValidation();

                int ReturnValue;

                ParseValue(dataContext);

                if (dataContext.FindFieldDataContext("TemplateCode").Text == "PRIVACY")//needs to be tested written as CurrentDocument.Template.Code
                {
                    if (dataContext.FindFieldDataContext("OCR_NAME").Text == "")
                    {
                        //this was left empty in the as-is code
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("AUTO_NAME").SetValue(dataContext.FindFieldDataContext("OCR_NAME"));
                    }

                    if (dataContext.FindFieldDataContext("OCR_DNS_3RD").Text == "1")
                    {
                        dataContext.FindFieldDataContext("OCR_DNS_3RD").SetValue("T");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("OCR_DNS_3RD").SetValue("F");
                    }

                    if (dataContext.FindFieldDataContext("OCR_DNS").Text == "1")
                    {
                        dataContext.FindFieldDataContext("OCR_DNS").SetValue("T");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("OCR_DNS").SetValue("F");
                    }

                    cpDH.PrivMasterID = dataContext.FindFieldDataContext("OCR_UNIQUE_ID").Text;
                    cpDH.FirstName = dataContext.FindFieldDataContext("OCR_FIRST_NAME").Text;
                    cpDH.LastName = dataContext.FindFieldDataContext("OCR_LAST_NAME").Text;
                    cpDH.Address1 = dataContext.FindFieldDataContext("OCR_STREET_ADDRESS").Text;
                    cpDH.State = dataContext.FindFieldDataContext("OCR_STATE").Text;
                    cpDH.ZipCode = dataContext.FindFieldDataContext("OCR_ZIP").Text;

                    if (dataContext.FindFieldDataContext("OCR_DNS_3RD").Text == "F" && dataContext.FindFieldDataContext("OCR_DNS").Text == "F")
                    {
                        cpDH.PrivShare3rdPartyOptOut = "T";
                        cpDH.PrivShareOptOut = "T";
                    }
                    else
                    {
                        cpDH.PrivShare3rdPartyOptOut = dataContext.FindFieldDataContext("OCR_DNS_3RD").Text;
                        cpDH.PrivShareOptOut = dataContext.FindFieldDataContext("OCR_DNS").Text;
                    }

                    ReturnValue = privVal.AutoValidate(ref cpDH);

                    if (ReturnValue == 0)
                    {
                        dataContext.FindFieldDataContext("AUTO_UNIQUE_ID").SetValue(cpDH.PrivMasterID);
                        dataContext.FindFieldDataContext("AUTO_FIRST_NAME").SetValue(cpDH.FirstName);
                        dataContext.FindFieldDataContext("AUTO_MIDDLE_NAME").SetValue(cpDH.MiddleName);
                        dataContext.FindFieldDataContext("AUTO_LAST_NAME").SetValue(cpDH.LastName);
                        dataContext.FindFieldDataContext("AUTO_STREET_ADDRESS1").SetValue(cpDH.Address1);
                        dataContext.FindFieldDataContext("AUTO_STREET_ADDRESS2").SetValue(cpDH.Address2);
                        dataContext.FindFieldDataContext("AUTO_CITY").SetValue(cpDH.City);
                        dataContext.FindFieldDataContext("AUTO_STATE").SetValue(cpDH.State);
                        dataContext.FindFieldDataContext("AUTO_ZIP").SetValue(cpDH.ZipCode);
                        dataContext.FindFieldDataContext("AUTO_DNS_3RD").SetValue(cpDH.PrivShare3rdPartyOptOut);
                        dataContext.FindFieldDataContext("AUTO_DNS").SetValue(cpDH.PrivShareOptOut);
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("(IA:" + ModCommon.gstrStep + ")" + "(" + cpDH.Validation.ToString() + ")");

                    }
                    else
                    {
                        dataContext.FindFieldDataContext("AUTO_DNS_3RD").SetValue(cpDH.PrivShare3rdPartyOptOut);
                        dataContext.FindFieldDataContext("AUTO_DNS").SetValue(cpDH.PrivShareOptOut);
                        dataContext.FindFieldDataContext("VALIDATED").SetValue(cpDH.ValidationMessage);
                        dataContext.FindFieldDataContext("VALIDATED").FlaggedReason = cpDH.ValidationMessage;
                        //dataContext.FindFieldDataContext("VALIDATED").SetValue(cpDH.ValidationMessage);//TODO
                        //^^^^TEST- should be written as dataContext.FindFieldDataContext("VALIDATED").OutputMessage = cpDH.ValidationMessage
                    }
                }
                else
                {
                    if (dataContext.FindFieldDataContext("Validated").Text == "")
                    {
                        dataContext.FindFieldDataContext("Validated").FlaggedReason = "RequiresValidation"; //TEST should be written as dataContext.FindFieldDataContext("Validated").RequiresValidation = true
                        dataContext.FindFieldDataContext("Validated").SetValidationError(new Exception());//TEST
                    }
                    else
                    {
                        //nothing needs to happen because there are no errors
                        //CurrentDocument.Fields("Validated").RequiresValidation = False
                        //CurrentDocument.Fields("Validated").SetStatusOK
                    }
                }
                cpDH.Clear();

            }
            catch (Exception ex)
            {
                int intErrorNum = 0; //No such thing as error number
                string strErrDesc = ex.Message;

                WriteError(intErrorNum, strErrDesc, "AutoValidation", "");
                   CNO.BPA.DataHandler.CommonParameters cpDH = new CNO.BPA.DataHandler.CommonParameters();
            }
        }

        private void ParseValue(IUimDataContext dataContext)
        {
            string[] arrValues;
            string revADD2;
            string[] x;
            int intWordsInCity;
            string strName;

            if (dataContext.FindFieldDataContext("OCR_NAME").Text != "")
            {
                strName = dataContext.FindFieldDataContext("OCR_NAME").Text;
                //Read in values
                arrValues = strName.Split();

                dataContext.FindFieldDataContext("OCR_LAST_NAME").SetValue(arrValues[0]);

                if (arrValues.GetUpperBound(0) >= 1)
                {
                    dataContext.FindFieldDataContext("OCR_FIRST_NAME").SetValue(arrValues[1]);
                }
            }

            if (dataContext.FindFieldDataContext("OCR_CITY_STATE_ZIP").Text != "")
            {
                //Reverse the string so we can work backward
                revADD2 = Reverse(dataContext.FindFieldDataContext("OCR_CITY_STATE_ZIP").Text);

                //zip will be the first column in all cases
                string value = ParseString(revADD2, " ", 1);
                dataContext.FindFieldDataContext("OCR_ZIP").SetValue(Reverse(value.Trim()));

                //state will be the second column in all cases
                value = ParseString(revADD2, " ", 2);
                dataContext.FindFieldDataContext("OCR_STATE").SetValue(Reverse(value.Trim()));
            }
        }

        private string ParseString(string Expression, string Delimiter, int Column)
        {
            try
            {
                int intFound;
                int intFoundOld;
                int intCurrLocation = 0;
                string strTemp = null;

                if (Expression.Length > 0 && Expression.IndexOf(Delimiter) > 0 && Column > 0)
                {
                    intFound = 1;
                    intFoundOld = 1;

                    while (Expression.Substring(intFoundOld + 1).IndexOf(Delimiter) > 0)
                    {
                        intFoundOld = intFound;
                        intFound = Expression.Substring(intFound + 1).IndexOf(Delimiter);
                        intCurrLocation = intCurrLocation + 1;
                    }

                    if (Column > intCurrLocation)
                    {
                        return null; //exit function in as-is code
                    }

                    intFound = 1;
                    intCurrLocation = 0;

                    do
                    {
                        intFoundOld = intFound;
                        intFound = Expression.Substring(intFound + 1).IndexOf(Delimiter);

                        if (strTemp.Trim() == "")
                        {
                            strTemp = Expression.Substring(1, intFound - 1);
                            intCurrLocation = intCurrLocation + 1;
                        }
                        else
                        {
                            if (intFound > 0)
                            {
                                strTemp = Expression.Substring(intFoundOld + 1, (intFound - 1) - intFoundOld);
                            }
                            else
                            {
                                Expression.Substring(intCurrLocation + 1);
                            }
                            intCurrLocation = intCurrLocation + 1;
                        }
                        if (intCurrLocation == Column)
                        {
                            return strTemp;
                        }
                    } while (true);
                }
                    return strTemp;
            }
            catch (Exception ex)
            {
                /* '         DispMsg "The application has failed due to the following: " _
'            & vbCrLf & vbCrLf & "     Error Location: ParseString Function" _
'            & vbCrLf & "     Error Number: " & lngErrNum _
'            & vbCrLf & "     Error Description: " & strErrDesc*/
                //^^commented out in as-is process code
                return null;
            }
        }

        private string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
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
                //If this fails Nothing more we can do...
            }

        }
    }
}
