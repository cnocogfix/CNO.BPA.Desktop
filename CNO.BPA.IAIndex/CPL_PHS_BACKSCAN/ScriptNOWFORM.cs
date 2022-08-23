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

    public class ScriptNOWFORM : UimScriptDocument
    {
        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        private const string _validationRule = "One";
        
        public ScriptNOWFORM()
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

        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                string strPolicyNumber;
                int intlenPolicy_Number;
                string VALIDATED_STATUS; //VARIANT

                dataContext.FindFieldDataContext("AUTO_WORK_TYPE").SetValue("BACKSCAN");
                dataContext.FindFieldDataContext("AUTO_BUSINESS_AREA").SetValue("CPLPIFL");

                //Parse the barcode and assign values to respective fields
                strPolicyNumber = ParseBarcodeFunction(dataContext.FindFieldDataContext("BARCODE").Text, dataContext);

                intlenPolicy_Number = strPolicyNumber.Length;

                if (intlenPolicy_Number == 10 && strPolicyNumber != "")
                {
                    VALIDATED_STATUS = CheckDigitSum(strPolicyNumber, dataContext);
                    dataContext.FindFieldDataContext("VALIDATED").SetValue(VALIDATED_STATUS);
                    if (VALIDATED_STATUS == "")
                    {
                        //make policy number blank if it is invalid
                        dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue("");
                    }
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    //make policy number blank if it is invalid
                    dataContext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue("");
                }

         

                if (dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "" && dataContext.FindFieldDataContext("VALIDATED").Text != "" && dataContext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").Text != "")
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                }
                else
                {
                    dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                }
                //END: Code for PRIVACY form
            }
            catch (Exception ex)
            {
                int errNum = 0;
                string batchName = "";
                ModCommon.WriteError(errNum, ex.Message, "AutoValidation", batchName);
                //not sure how to set output message or setstatuserror
            }

        }
  
        private string ParseBarcodeFunction(string BARCODE, IUimDataContext datacontext)
        {
            string strTemp;
            string strPolicyNumber = null;
            string strDocType;
            int lenMoliDocType;

            try
            {


                //Extract field values from BARCODE variable
                string value = "9";
                strTemp = ModCommon.ParseParam(BARCODE, 4, value.ToString());
                strDocType = ModCommon.ParseParam(BARCODE, 4, value.ToString());
                strPolicyNumber = ModCommon.ParseParam(BARCODE, 4, value.ToString());

                datacontext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(ModCommon.ParseParam(strTemp, 2, "\r").Replace(value, ""));

                if(datacontext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "CPL" && datacontext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "ILI")
                {
                    datacontext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
                
                datacontext.FindFieldDataContext("AUTO_POLICY_NUMBER").SetValue(strPolicyNumber);

                lenMoliDocType = strDocType.Length;

                if(lenMoliDocType == 1)
                {
                    datacontext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("000"+ strDocType);
                }
                else if(lenMoliDocType == 2)
                {
                    datacontext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("00" + strDocType);
                }
                else
                {
                    datacontext.FindFieldDataContext("AUTO_MOLIDOC_TYPE").SetValue("");
                }

                //return strPolicyNumber;

            }
            catch (Exception ex)
            {
                int errNum = 0;
                string batchName = "";
                ModCommon.WriteError(errNum, ex.Message, "AutoValidation", batchName);
                //not sure how to set output message or setstatuserror
            }

            return strPolicyNumber;
        }

        private string CheckDigitSum(string Policy_Number, IUimDataContext dataContext)
        {
            int lngLenPolicy_Number = 0;
            char[] strArrayPolicy_Number = null;
            char[] tempPolicy_Number = null;// As Variant
            int ILoop = 0;
            int JLoop = 0;
            bool IsCharOrDigit1 = false;
            bool IsCharOrDigit2 = false;
            bool IsFirstCharANumber;
            bool IsSecondCharANumber;
            bool IsNumber = false;
            int firstDigit = 0;
            int secondDigit = 0;
            int Sum = 0;
            int Remainder = 0;
            char chkDigit_PolicyNumber; // As Variant
            string result = "";

            try
            {

                //'Finding the length of policy number
                lngLenPolicy_Number = Policy_Number.Length;

                //'Reallocate the "Policy_Number" storage space to "strArrayPolicy_Number" string array variable
                strArrayPolicy_Number = Policy_Number.ToCharArray();

                //'Reallocate the "Policy_Number"+1 storage space to "tempPolicy_Number" string array variable
                tempPolicy_Number = new char[lngLenPolicy_Number + 2];


                //Check the first two characters of Policy_Number are Digit or Alphabet and convert.
                if ((int)strArrayPolicy_Number[0] >= 65 && (int)strArrayPolicy_Number[0] <= 90)
                {
                    firstDigit = ModCommon.ReplacewithDigit((int)strArrayPolicy_Number[0]);

                    tempPolicy_Number[0] = (char)(firstDigit + 48);

                    IsCharOrDigit1 = true;
                    IsFirstCharANumber = false;
                }
                else if ((int)strArrayPolicy_Number[0] >= 48 && (int)strArrayPolicy_Number[0] <= 57)
                {
                    tempPolicy_Number[0] = strArrayPolicy_Number[0]; // error within old code.. Old Code: tempPolicy_Number(1) = strArrayPolicy_Number(1)

                    IsCharOrDigit1 = true;
                    IsFirstCharANumber = true;
                }
                else
                {
                    IsCharOrDigit1 = false;
                    IsFirstCharANumber = false;
                }

                if ((int)strArrayPolicy_Number[1] >= 65 && (int)strArrayPolicy_Number[1] <= 90)
                {
                    secondDigit = ModCommon.ReplacewithDigit((int)strArrayPolicy_Number[1]);

                    tempPolicy_Number[1] = (char)(secondDigit + 48);

                    IsCharOrDigit2 = true;
                    IsSecondCharANumber = false;
                }
                else if ((int)strArrayPolicy_Number[1] >= 48 && (int)strArrayPolicy_Number[1] <= 57)
                {
                    tempPolicy_Number[1] = strArrayPolicy_Number[1];

                    IsCharOrDigit2 = true;
                    IsSecondCharANumber = true;
                }
                else
                {
                    IsCharOrDigit2 = false;
                    IsSecondCharANumber = false;
                }


                //'If first two letters of Policy_Number are Alphabet or Digit
                if (IsFirstCharANumber != true || IsSecondCharANumber != true)
                {
                    if (IsCharOrDigit1 == true && IsCharOrDigit2 == true)
                    {
                        ILoop = 2;
                        //'Check the 3 to 9 characters of Policy_Number are Digit or Alphabet
                        while (ILoop <= lngLenPolicy_Number - 2)
                        {
                            if ((int)strArrayPolicy_Number[ILoop] >= 48 && (int)strArrayPolicy_Number[ILoop] <= 57)
                            {
                                IsNumber = true;
                            }
                            else
                            {
                                IsNumber = false;
                                break;
                            }
                            ILoop++;

                        }

                        //'If 3 to 9 cahracters of Policy_Number are Digit from 0 to 9
                        if (IsNumber == true)
                        {
                            //'Insert "0" value at third index of String Array variable
                            tempPolicy_Number[2] = '0';

                            //'Move the values from one string array variable to another string array variable
                            for (ILoop = 2; ILoop <= lngLenPolicy_Number - 2; ILoop++)
                            {
                                strArrayPolicy_Number[ILoop] = Policy_Number[ILoop];

                                tempPolicy_Number[ILoop + 1] = strArrayPolicy_Number[ILoop];

                            }

                            Sum = 0;

                            ILoop = lngLenPolicy_Number - 1;

                            JLoop = 2;

                            //'As per checkdigit algorithm calculated the Sum
                            while (ILoop >= 0 && JLoop <= 7)
                            {
                                Sum = Sum + (tempPolicy_Number[ILoop] * JLoop); // DANGEROUS.. multiplication as a char or a number? Ambiguity here due to variant type.. TODO

                                if (JLoop == 7)
                                {
                                    JLoop = 1;
                                }

                                JLoop = JLoop + 1;
                                ILoop = ILoop - 1;
                            }

                            //Peform Mod operation on Sum
                            Remainder = Sum % 11;

                            //Substract the remainder from 11
                            chkDigit_PolicyNumber = (char)((11 - Remainder) + 48);


                            if (chkDigit_PolicyNumber == 10)
                            {
                                //if checkdigit is 10 then replace with "0"
                                chkDigit_PolicyNumber = '0';


                            }
                            else if (chkDigit_PolicyNumber == 11)
                            {
                                chkDigit_PolicyNumber = 'B';

                            }



                            if (chkDigit_PolicyNumber == strArrayPolicy_Number[lngLenPolicy_Number - 1])
                            {
                                result = "SUCCESS";
                            }
                            else
                            {
                                result = "";
                            }

                        }
                        else
                        {
                            result = "";

                        }
                        //is a number if else ends

                    }
                    else
                    {
                        result = "";

                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                int errNum = 0;
                string batchName = "";
                ModCommon.WriteError(errNum, ex.Message, "CheckDigitSum", batchName);

                //dataContext.FindFieldDataContext("Validation").OutputMessage = strErrDesc;
                dataContext.FindFieldDataContext("Validation").SetValidationError(ex);
            }
            return null;
        }

    }
}
