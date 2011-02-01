using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroBarcode
{
    public class EANBarcode
    {
        private EANBarcode()
        {
        }

        private static EANBarcode _Instance;
        public static EANBarcode Instance
        {
            get { return _Instance ?? (_Instance = new EANBarcode()); }
        }

        public string EAN8(string chaine)
        {
            int i;
            double checksum;
            string CodeBarre = "";
            checksum = 0;
            if (chaine.Length == 7)
            {
                for (i = 1; i <= 7; i++)
                {
                    int L1 = Convert.ToChar(chaine.Substring(i - 1, 1));
                    if ((L1 < 48) || (L1 > 57))
                    {
                        i = 0;
                        break;
                    }
                }
                if (i == 8)
                {
                    for (i = 7; i > 0; i = i - 2)
                    {
                        checksum = checksum + Convert.ToInt32(chaine.Substring(i - 1, 1));
                    }
                    checksum = checksum * 3;

                    for (i = 6; i > 0; i = i - 2)
                    {
                        checksum = checksum + Convert.ToInt32(chaine.Substring(i - 1, 1));
                    }
                    chaine = chaine + (10 - checksum % 10) % 10;

                    CodeBarre = ":";

                    for (i = 1; i <= 4; i++)
                    {
                        CodeBarre = CodeBarre + Convert.ToChar(65 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                    }
                    CodeBarre = CodeBarre + "*";
                    for (i = 5; i <= 8; i++)
                    {
                        CodeBarre = CodeBarre + Convert.ToChar(97 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                    }
                    CodeBarre = CodeBarre + "+";
                }
            }
            return CodeBarre;
        }

        public object EAN13(string chaine)
        {
            object functionReturnValue = null;
            int i;
            int checksum = 0;
            int first;
            string CodeBarre;
            bool tableA;
            functionReturnValue = "";
            if (chaine.Length == 12)
            {
                for (i = 1; i <= 12; i++)
                {
                    int L1 = Convert.ToChar(chaine.Substring(i - 1, 1));
                    if (L1 < 48 || L1 > 57)
                    {
                        i = 0;
                        break;
                    }
                }
                if (i == 13)
                {
                    for (i = 12; i >= 1; i += -2)
                    {
                        checksum = checksum + Convert.ToInt32(chaine.Substring(i - 1, 1));
                    }
                    checksum = checksum * 3;
                    for (i = 11; i >= 1; i += -2)
                    {
                        checksum = checksum + Convert.ToInt32(chaine.Substring(i - 1, 1));
                    }
                    chaine = chaine + (10 - checksum % 10) % 10;
                    CodeBarre = chaine.Substring(0, 1) + Convert.ToChar(65 + Convert.ToInt32((chaine.Substring(1, 1))));
                    first = Convert.ToInt32(chaine.Substring(0, 1));

                    for (i = 3; i <= 7; i++)
                    {
                        tableA = false;
                        switch (i)
                        {
                            case 3:
                                switch (first)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                    case 3:
                                        tableA = true;
                                        break;
                                }
                                break;
                            case 4:
                                switch (first)
                                {
                                    case 0:
                                    case 4:
                                    case 7:
                                    case 8:
                                        tableA = true;
                                        break;
                                }
                                break;
                            case 5:
                                switch (first)
                                {
                                    case 0:
                                    case 1:
                                    case 4:
                                    case 5:
                                    case 9:
                                        tableA = true;
                                        break;
                                }
                                break;
                            case 6:
                                switch (first)
                                {
                                    case 0:
                                    case 2:
                                    case 5:
                                    case 6:
                                    case 7:
                                        tableA = true;
                                        break;
                                }
                                break;
                            case 7:
                                switch (first)
                                {
                                    case 0:
                                    case 3:
                                    case 6:
                                    case 8:
                                    case 9:
                                        tableA = true;
                                        break;
                                }
                                break;
                        }
                        if (tableA)
                        {
                            CodeBarre = CodeBarre + Convert.ToChar(65 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                        }
                        else
                        {
                            CodeBarre = CodeBarre + Convert.ToChar(75 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                        }
                    }
                    CodeBarre = CodeBarre + "*";
                    for (i = 8; i <= 13; i++)
                    {
                        CodeBarre = CodeBarre + Convert.ToChar(97 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                    }
                    CodeBarre = CodeBarre + "+";
                    functionReturnValue = CodeBarre;
                }
            }
            return functionReturnValue;
        }

        public string AddOn(string chaine)
        {
            int i;
            int checksum = 0;
            string AddOnn = "";

            bool tableA;

            if (chaine.Length == 2 || chaine.Length == 5)
            {
                for (i = 1; i < chaine.Length; i++)
                {
                    int L1 = Convert.ToChar(chaine.Substring(i - 1, 1));

                    if (L1 < 48 || L1 > 57)
                    {
                        break;
                    }
                    if (chaine.Length == 2)
                    {
                        checksum = 10 + Convert.ToInt32(chaine) % 4;
                    }
                    else if (chaine.Length == 5)
                    {
                        for (i = 1; i == 5; i = i - 2)
                        {
                            checksum = checksum + Convert.ToInt32(chaine.Substring(i - 1, 1));
                        }
                        checksum = (checksum * 3 + Convert.ToInt32(chaine.Substring(2, 1)) * 9 + Convert.ToInt32(chaine.Substring(4, 1)) * 9) % 10;
                    }
                    AddOnn = "[";
                    for (i = 1; i <= chaine.Length; i++)
                    {
                        tableA = false;

                        switch (i)
                        {
                            case 1:
                                int[] str = { 4, 9, 10, 11 };
                                for (int j = 0; j < str.Length; j++)
                                {
                                    if (str[j] == checksum)
                                    {
                                        tableA = false;
                                        break;
                                    }
                                }
                                break;

                            case 2:
                                int[] str1 = { 1, 2, 3, 5, 6, 7, 10, 12 };
                                for (int j = 0; j < str1.Length; j++)
                                {
                                    if (str1[j] == checksum)
                                    {
                                        tableA = false;
                                        break;
                                    }
                                }
                                break;
                            case 3:
                                int[] str2 = { 0, 2, 3, 6, 7 };
                                for (int j = 0; j < str2.Length; j++)
                                {
                                    if (str2[j] == checksum)
                                    {
                                        tableA = false;
                                        break;
                                    }
                                }
                                break;
                            case 4:
                                int[] str3 = { 1, 3, 4, 8, 9 };
                                for (int j = 0; j < str3.Length; j++)
                                {
                                    if (str3[j] == checksum)
                                    {
                                        tableA = false;
                                        break;
                                    }
                                }
                                break;
                            case 5:
                                int[] str4 = { 0, 1, 2, 4, 5, 7 };
                                for (int j = 0; j < str4.Length; j++)
                                {
                                    if (str4[j] == checksum)
                                    {
                                        tableA = false;
                                        break;
                                    }
                                }
                                break;
                        }

                        if (tableA)
                            AddOnn = AddOnn + Convert.ToChar(65 + Convert.ToInt32(chaine.Substring(i - 1, 1)));
                        else
                            AddOnn = AddOnn + Convert.ToChar(75 + Convert.ToInt32(chaine.Substring(i - 1, 1)));

                        if ((chaine.Length == 2 && i == 1) || (chaine.Length == 5 && i < 5))
                            AddOnn = AddOnn + Convert.ToChar(92);

                    }


                }
            }
            return AddOnn;
        }

    }
}
