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

    public class ScriptCPLNBFORM : UimScriptDocument
    {
        private const string _validationRule = "One";
      
        public ScriptCPLNBFORM()
            : base()
        {

		}
        
       
        public void ExecuteValidationRuleOne(IUimDataContext dataContext)
		{
			if (dataContext.FindFieldDataContext("VALIDATED").Text != "SUCCESS")
			{

			}
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

            /*
            dataContext.FindFieldDataContext("AUTO_POLICY_NO").SetValue(ModCommon.RemovePunctuation(dataContext.FindFieldDataContext("AUTO_POLICY_NO").Text));

            dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").SetValue(dataContext.StepCustomValue);

            if (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "U30")
            {
                parseBarcode(dataContext);
            }
            else
            {
                parseCompanyCode(dataContext);
            }
            
            //AutoValidate
            AutoValidation(dataContext);*/
        }


        private void AutoValidation(IUimDataContext dataContext)
        {
            try
            {
                string[] strBarcodeValue = new string[6];
                string ReturnValue;
                string strPolicyNumber;
                string VALIDATED_STATUS = ""; //As Variant

                validateProductType(dataContext);

                strPolicyNumber = dataContext.FindFieldDataContext("AUTO_POLICY_NO").Text;

                //Call method CheckDigitNumber
                if (strPolicyNumber.Length == 10 && strPolicyNumber != "")
                {
                    VALIDATED_STATUS = CheckDigitSum(strPolicyNumber, dataContext);
                }

                if (VALIDATED_STATUS.ToString() == "SUCCESS")
                {
                    dataContext.FindFieldDataContext("AUTO_POLICY_NO").SetValue(strPolicyNumber);
                }
                else
                {
                    dataContext.FindFieldDataContext("AUTO_POLICY_NO").SetValue("");
                }

                //Replacement Indicator

                if (dataContext.FindFieldDataContext("AUTO_REPLACEMENT_INDICATOR").Text == "1")
                {
                    dataContext.FindFieldDataContext("AUTO_REPLACEMENT_INDICATOR").SetValue("Yes");
                }
                else if (dataContext.FindFieldDataContext("AUTO_REPLACEMENT_INDICATOR").Text == "0")
                {
                    dataContext.FindFieldDataContext("AUTO_REPLACEMENT_INDICATOR").SetValue("No");
                }

                //Check whether Fields are recognized 100% or not for Claims and Reinstatements forms
                if (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "HCLAIM" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "LCLAIM" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "CPLREINST1" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "CPLREINST2" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "CPLREINST3" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "BLCREINST1" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "BLCREINST2")
                {
                    if (dataContext.FindFieldDataContext("AUTO_POLICY_NO").Text != "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "")
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    }
                }

                //Check where Fields are recognized 100% or not for Live Guaranteed Issue and Live Underwritten forms
                if(dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPGI" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPLUW")
                {
                    if(dataContext.FindFieldDataContext("AUTO_POLICY_NO").Text != "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "" && dataContext.FindFieldDataContext("AUTO_MEMBER_NO").Text != "" && dataContext.FindFieldDataContext("AUTO_PRODUCT_TYPE").Text != "")
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    }
                }

                //check whether Fields are recognized 100% or not for guranteed issue, underwritten and patriot forms
                if (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPBLC" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPGIFLI" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPPAT" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPGIFC" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPGIE" || dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "APPUW")
                {
                    if (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text == "" && dataContext.FindFieldDataContext("AUTO_MEMBER_NO").Text == "" && dataContext.FindFieldDataContext("AUTO_PRODUCT_TYPE").Text == "")
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    }
                }

                //check whether fields are recognized 100% or not for U30 forms
                if (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text == "U30")
                {
                    if(dataContext.FindFieldDataContext("AUTO_DOC_TYPE").Text != "" && dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text != "")
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("SUCCESS");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("VALIDATED").SetValue("");
                    }
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

        private void parseBarcode(IUimDataContext dataContext)
        {
            try
            {
                string[] barcodeVals;
                string strTemplateCode;
                int barcodeLength;
                string CPL_Chkbox;
                string ILI_Chkbox;
                int intOMR_DOC_TYPE_APPI;
                int intOMR_DOC_TYPE_2SIDED;
                int intOMR_DOC_TYPE_3SIDED;
                int intOMR_DOC_TYPE_4SIDED;
                int intOMR_DOC_TYPE_5SIDED;
                int intTotal;

                char[] delimiter = new char[] { '|' };
                barcodeVals = dataContext.FindFieldDataContext("OCR_417").Text.Split(delimiter, 2);
                strTemplateCode = dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text.Trim();

                //The following commented out code was also commented out in the as-is code
                /*
                barcodeVals = dataContext.FindFieldDataContext("OCR_417").Text;
                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue(ModCommon.ParseParam(barcodeVals, 3, "\t").Trim().Replace("\r", ""));
                dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(ModCommon.ParseParam(barcodeVals, 4, "\t"));
                 */

                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue(barcodeVals[0].ToString());

                if (barcodeVals[1].ToString().Substring(barcodeVals[1].Length - 1) == "|")
                {
                    barcodeVals[1] = barcodeVals[1].ToString().Substring(0, barcodeVals[1].Length - 1);
                }

                dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue(barcodeVals[1]);
                if (strTemplateCode == "U30")
                {
                    if (dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text == "")
                    {
                        CPL_Chkbox = dataContext.FindFieldDataContext("OMR_COMPANY_CODE_CPL").Text;
                        ILI_Chkbox = dataContext.FindFieldDataContext("OMR_COMPANY_CODE_ILI").Text;

                        if (CPL_Chkbox == "1" && ILI_Chkbox == "0")
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                        }
                        else if (CPL_Chkbox == "0" && ILI_Chkbox == "1")
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                        }
                        else if ((CPL_Chkbox == "1" && ILI_Chkbox == "1") || (CPL_Chkbox == "0" && ILI_Chkbox == "0"))
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                        }
                        else
                        {
                            dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                        }
                    }

                    if (dataContext.FindFieldDataContext("AUTO_DOC_TYPE").Text == "")
                    {
                        intOMR_DOC_TYPE_APPI = Convert.ToInt32(dataContext.FindFieldDataContext("OMR_DOC_TYPE_APPI").Text);
                        intOMR_DOC_TYPE_2SIDED = Convert.ToInt32(dataContext.FindFieldDataContext("OMR_DOC_TYPE_2SIDED").Text);
                        intOMR_DOC_TYPE_3SIDED = Convert.ToInt32(dataContext.FindFieldDataContext("OMR_DOC_TYPE_3SIDED").Text);
                        intOMR_DOC_TYPE_4SIDED = Convert.ToInt32(dataContext.FindFieldDataContext("OMR_DOC_TYPE_4SIDED").Text);
                        intOMR_DOC_TYPE_5SIDED = Convert.ToInt32(dataContext.FindFieldDataContext("OMR_DOC_TYPE_5SIDED").Text);

                        intTotal = intOMR_DOC_TYPE_APPI + intOMR_DOC_TYPE_2SIDED + intOMR_DOC_TYPE_3SIDED + intOMR_DOC_TYPE_4SIDED + intOMR_DOC_TYPE_5SIDED;

                        if (intTotal == 1)
                        {
                            if (intOMR_DOC_TYPE_APPI == 1)
                            {
                                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("APPI");
                            }
                            else if (intOMR_DOC_TYPE_2SIDED == 1)
                            {
                                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("2SIDED");
                            }
                            else if (intOMR_DOC_TYPE_3SIDED == 1)
                            {
                                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("3SIDED");
                            }
                            else if (intOMR_DOC_TYPE_4SIDED == 1)
                            {
                                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("4SIDED");
                            }
                            else if (intOMR_DOC_TYPE_5SIDED == 1)
                            {
                                dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("5SIDED");
                            }
                        }
                        else
                        {
                            dataContext.FindFieldDataContext("AUTO_DOC_TYPE").SetValue("");
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }

        }

        private void parseCompanyCode(IUimDataContext dataContext)
        {
            string strTemplateCode;
            string CPL_Chkbox= null;
            string ILI_Chkbox = null;

            try
            {
                if (dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").Text == "")
                {
                    if (dataContext.FindFieldDataContext("OCR_COMPANY_CODE").Text.ToUpper().Contains("COLONIAL PENN"))
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                    }
                    else if(dataContext.FindFieldDataContext("OCR_COMPANY_CODE").Text.ToUpper().Contains("BANKERS"))
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                    }
                }

                strTemplateCode = dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text;

                //Specific to Reinstatements
                if("CPLREINST1" == strTemplateCode || strTemplateCode == "CPLREINST2" || strTemplateCode == "CPLREINST3")
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                }
                else if(strTemplateCode == "BLCREINST1" || strTemplateCode == "BLCREINST2")
                {
                    dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                }

                //specific to claims
                if (strTemplateCode == "HCLAIM" || strTemplateCode == "LCLAIM")
                {
                    CPL_Chkbox = dataContext.FindFieldDataContext("OMR_COMPANY_CODE_CPL").Text;
                    ILI_Chkbox = dataContext.FindFieldDataContext("OMR_COMPANY_CODE_ILI").Text;


                    if (CPL_Chkbox == "1" && ILI_Chkbox == "0")
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("CPL");
                    }
                    else if (CPL_Chkbox == "0" && ILI_Chkbox == "1")
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("ILI");
                    }
                    else if ((CPL_Chkbox == "1" && ILI_Chkbox == "1") || (CPL_Chkbox == "0" && ILI_Chkbox == "0"))
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                    }
                    else
                    {
                        dataContext.FindFieldDataContext("AUTO_COMPANY_CODE").SetValue("");
                    }
                }
            }
            catch (Exception ex)
            {
                //ModCommon.WriteError(ex.Message.ToString(), "AutoValidation");
                //Commented out in the as-is process
            }
        }

        private void validateProductType(IUimDataContext dataContext)
        {
            try
            {
                //Product Type
                string productType = dataContext.FindFieldDataContext("AUTO_PRODUCT_TYPE").Text;
                //all changes to this fielddata are stroed in this string and written to the field at the end of this task
                switch (dataContext.FindFieldDataContext("AUTO_TEMPLATE_CODE").Text)
                {
                    case "APPGI":
                        if (productType.Contains("G") || productType.Contains("R"))
                        {
                            productType = "GI";
                        }
                        else
                        {
                            productType = "";
                        }
                        break;
                    case "APPGIFLI":
                        if (productType.Contains("G") || productType.Contains("R"))
                        {
                            productType = "GI";
                        }
                        else
                        {
                            productType = "";
                        }
                        break;
                    case "APPLUW":
                        //this case contains the same actions as the one underneath it, hence no breaks
                    case "APPUW":
                        //every case within this switch will yield the same result hence no breaks
                        switch (productType)
                        {
                            case "T5YR":
                            case "T20YR":
                            case "T20NR":
                            case "T99F":
                            case "SILC":
                            case "WLSLC":
                            case "JWL":
                                productType = "UW";
                                break;
                            default:
                                productType = "";
                                break;
                        }
                        break;
                    case "APPPAT":
                        switch(productType)
                        {
                            //same conditions occur in t90 and siwl
                            case "T90":
                            case "SIWL":
                                productType = "PP";
                                break;
                            default:
                                productType = "";
                                break;
                        }
                        break;
                }
                
                //writes the value of productType to the field
                dataContext.FindFieldDataContext("AUTO_PRODUCT_TYPE").SetValue(productType);
            }
            catch (Exception EX)
            {
                //this clause was left empty in the as-is code
            }
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
