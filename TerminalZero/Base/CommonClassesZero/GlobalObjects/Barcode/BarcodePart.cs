using System.Collections.Generic;

namespace ZeroCommonClasses.GlobalObjects.Barcode
{
    public class BarCodePart
    {
        public static List<BarCodePart> BuildComposition(string composition, string values)
        {
            var Parts = new List<BarCodePart>();
            if (composition != null)
            {
                char[] aux = composition.ToCharArray();
                int length = 0;
                for (int i = 0; i < aux.Length; i++)
                {
                    var ms = new BarCodePart(aux[i]);
                    for (int j = i + 1; j < aux.Length; j++)
                    {
                        if (aux[i] == aux[j])
                        {
                            ms.OriginalLength++;
                            i = j;
                        }
                        else
                            break;
                    }
                    if (values.Length >= length + ms.OriginalLength)
                    {
                        ms.Code = int.Parse(values.Substring(length, ms.OriginalLength));
                        length += ms.OriginalLength;
                    }
                    Parts.Add(ms);
                }
            }

            return Parts;
        }

        internal static string ResolveName(char compositionChar)
        {
            string ret = "";
            switch (compositionChar)
            {
                case 'Y':
                    ret = "Año";
                    break;
                case 'M':
                    ret = "Mes";
                    break;
                case 'D':
                    ret = "Día";
                    break;
                case 'P':
                    ret = "Producto";
                    break;
                case 'Q':
                    ret = "Cantidad";
                    break;
                default:
                    ret = "Unknown";
                    break;
            }
            return ret;
        }

        private BarCodePart(char compositionChar)
        {
            Name = ResolveName(compositionChar);
            Composition = compositionChar;
            OriginalLength = 1;
            IsValid = true;
            Code = 0;
        }

        public bool IsValid { get; set; }

        public int OriginalLength { get; set; }
        public string Name { get; set; }
        public char Composition { get; set; }
        public int Code { get; set; }
    }
}
