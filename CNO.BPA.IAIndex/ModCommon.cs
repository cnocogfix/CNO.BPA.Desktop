using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Custom.InputAccel.UimScript
{
    static class ModCommon
    {

        [System.Runtime.InteropServices.DllImport("iaclnt32.dll", SetLastError = true)]
        static extern void IADebugOutVB_PHEOMB(int iaclient, string strText);

        
        public static string gstrMachineName;
        public static string gstrBackScanStep;
        public static string gstrArchiveStep;
        public static string gstrPrivacyStep;
        public static string gstrStep;
        public static string gstr4086Step;


        public const int CONST_A = 1;
        public const int CONST_B = 2;
        public const int CONST_C = 3;
        public const int CONST_D = 4;
        public const int CONST_E = 5;
        public const int CONST_F = 6;
        public const int CONST_G = 7;
        public const int CONST_H = 8;
        public const int CONST_I = 9;
        public const int CONST_J = 1;
        public const int CONST_K = 2;
        public const int CONST_L = 3;
        public const int CONST_M = 4;
        public const int CONST_N = 5;
        public const int CONST_O = 6;
        public const int CONST_P = 7;
        public const int CONST_Q = 8;
        public const int CONST_R = 9;
        public const int CONST_S = 2;
        public const int CONST_T = 3;
        public const int CONST_U = 4;
        public const int CONST_V = 5;
        public const int CONST_W = 6;
        public const int CONST_X = 7;
        public const int CONST_Y = 8;
        public const int CONST_Z = 9;


        public static string RemovePunctuation(string strValue)
        {
            string strTemp;
            int intCnt;
            string strChar;

            strTemp = "";

            for (intCnt = 0; intCnt < strValue.Length; intCnt++)
            {
                strChar = strValue.Substring(intCnt, 1);
                if ("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(strChar.ToUpper()))
                {
                    strTemp = strTemp + strChar;
                }
            }
            return strTemp;
        }

        public static string removeAlphaNumeric(string strValue)
        {
            StringBuilder strBuilder = new StringBuilder("");

            for (int i = 0; i < strValue.Length; i++)
            {
                if (strValue[i] >= 48 && strValue[i] <= 57) // ASCII: 48 == '0' 57 == '9'
                {
                    strBuilder.Append(strValue[i]);
                }
            }
            return strBuilder.ToString();
        }

        public static int ReplacewithDigit(int charPolicyNumber)
        {

            if (charPolicyNumber == 65)
            {
                charPolicyNumber = ModCommon.CONST_A;
            }
            else if (charPolicyNumber == 66)
            {
                charPolicyNumber = ModCommon.CONST_B;
            }
            else if (charPolicyNumber == 67)
            {
                charPolicyNumber = ModCommon.CONST_C;
            }
            else if (charPolicyNumber == 68)
            {
                charPolicyNumber = ModCommon.CONST_D;
            }
            else if (charPolicyNumber == 69)
            {
                charPolicyNumber = ModCommon.CONST_E;
            }
            else if (charPolicyNumber == 70)
            {
                charPolicyNumber = ModCommon.CONST_F;
            }
            else if (charPolicyNumber == 71)
            {
                charPolicyNumber = ModCommon.CONST_G;
            }
            else if (charPolicyNumber == 72)
            {
                charPolicyNumber = ModCommon.CONST_H;
            }
            else if (charPolicyNumber == 73)
            {
                charPolicyNumber = ModCommon.CONST_I;
            }
            else if (charPolicyNumber == 74)
            {
                charPolicyNumber = ModCommon.CONST_J;
            }
            else if (charPolicyNumber == 75)
            {
                charPolicyNumber = ModCommon.CONST_K;
            }
            else if (charPolicyNumber == 76)
            {
                charPolicyNumber = ModCommon.CONST_L;
            }
            else if (charPolicyNumber == 77)
            {
                charPolicyNumber = ModCommon.CONST_M;
            }
            else if (charPolicyNumber == 78)
            {
                charPolicyNumber = ModCommon.CONST_N;
            }
            else if (charPolicyNumber == 79)
            {
                charPolicyNumber = ModCommon.CONST_O;
            }
            else if (charPolicyNumber == 80)
            {
                charPolicyNumber = ModCommon.CONST_P;
            }
            else if (charPolicyNumber == 81)
            {
                charPolicyNumber = ModCommon.CONST_Q;
            }
            else if (charPolicyNumber == 82)
            {
                charPolicyNumber = ModCommon.CONST_R;
            }
            else if (charPolicyNumber == 83)
            {
                charPolicyNumber = ModCommon.CONST_S;
            }
            else if (charPolicyNumber == 84)
            {
                charPolicyNumber = ModCommon.CONST_T;
            }
            else if (charPolicyNumber == 85)
            {
                charPolicyNumber = ModCommon.CONST_U;
            }
            else if (charPolicyNumber == 86)
            {
                charPolicyNumber = ModCommon.CONST_V;
            }
            else if (charPolicyNumber == 87)
            {
                charPolicyNumber = ModCommon.CONST_W;
            }
            else if (charPolicyNumber == 88)
            {
                charPolicyNumber = ModCommon.CONST_X;
            }
            else if (charPolicyNumber == 89)
            {
                charPolicyNumber = ModCommon.CONST_Y;
            }
            else if (charPolicyNumber == 90)
            {
                charPolicyNumber = ModCommon.CONST_Z;
            }

            //return

            return charPolicyNumber;

        }

        public static string ParseParam(string strInput, int intSequence, string strSeparator)
        {
            //int intCnt;
            //int intPos;
            //int intLen;
            //string strTemp = "";
            string strOrigValue;
            string[] separators;
            separators = new string[] { strSeparator};

            strOrigValue = strInput;

            if (strOrigValue.Trim() == "" || intSequence < 1 || strSeparator == "")
            {
                return "";
            }
            
            string[] results = strInput.Split(separators, StringSplitOptions.None);

            if (intSequence > results.Length)
            {
                return "";
            }

            return results[intSequence - 1];


            /*
            for (intCnt = 1; intCnt <= intSequence; intCnt++)
            {
                intPos = strOrigValue.IndexOf(strSeparator);
                intLen = strOrigValue.Length;
                if (intPos == 0)
                {
                    strTemp = strOrigValue;
                }
                else
                {
                    strTemp = strOrigValue.Substring(1, intPos - 1).Trim();
                    strOrigValue = strOrigValue.Substring(intPos + 1, intLen - intPos);
                }
            }
            return strTemp;*/
        }

        public static string getMachineName()
        {
            //return Environment.MachineName;
            return System.Net.Dns.GetHostName();
        }

        public static string getOperatorID()
        {
            return Environment.UserName;
        }

        internal static string RemoveFrontZeroes(string strInput)
        {
            int lngFCnt;
            int lngFLen;
            int lngFPos;
            string sFChar;

            lngFLen = strInput.Length;
            if (lngFLen == 0)
            {
                return "";
            }

            lngFPos = 0;
            for (lngFCnt = 0; lngFCnt < lngFLen; lngFCnt++)
            {
                sFChar = strInput.Substring(lngFCnt, 1);
                if (sFChar == "0")
                {
                    lngFPos = lngFCnt;
                }
                else
                {
                    break;
                }
            }

            if (lngFPos > 0)
            {
                return strInput.Substring(lngFPos + 1, strInput.Length);
            }
            else
            {
                return strInput;
            }

       
        }

        public static void WriteError(int errorNum, string errorDesc, string errorLocation, string batchName)
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
