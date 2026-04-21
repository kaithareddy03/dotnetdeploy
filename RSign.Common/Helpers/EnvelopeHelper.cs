
namespace RSign.Common.Helpers
{
    public class EnvelopeHelper
    {
        public static string GetExpiryType(Guid ExpiryTypeID)
        {
            if (ExpiryTypeID == Constants.ExpiryType.One_Weeks)
                return "One Week";
            else if (ExpiryTypeID == Constants.ExpiryType.Two_Weeks)
                return "Two Weeks";
            else if (ExpiryTypeID == Constants.ExpiryType.Thirty_Days)
                return "One Month";
            else if (ExpiryTypeID == Constants.ExpiryType.Three_Months)
                return "Three Months";
            else if (ExpiryTypeID == Constants.ExpiryType.One_Day)
                return "One Day";
            else if (ExpiryTypeID == Constants.ExpiryType.Two_Days)
                return "Two Days";
            else if (ExpiryTypeID == Constants.ExpiryType.Three_Days)
                return "Three Days";
            else if (ExpiryTypeID == Constants.ExpiryType.Four_Days)
                return "Four Days";
            else if (ExpiryTypeID == Constants.ExpiryType.Five_Days)
                return "Five Days";
            else if (ExpiryTypeID == Constants.ExpiryType.Six_Days)
                return "Six Days";
            else if (ExpiryTypeID == Constants.ExpiryType.Ten_Days)
                return "Ten Days";
            return string.Empty;

        }
        public static string GetRecipentType(Guid RecipintID)
        {
            if (RecipintID == new Guid("63EA73C2-4B64-4974-A7D5-0312B49D29D0"))
                return "CC";
            else if (RecipintID == new Guid("C20C350E-1C6D-4C03-9154-2CC688C099CB"))
                return "Signer";
            else if (RecipintID == new Guid("26E35C91-5EE1-4ABF-B421-3B631A34F677"))
                return "Sender";
            else if (RecipintID == new Guid("712F1A0B-1AC6-4013-8D74-AAC4A9BF5568"))
                return "Prefill";
            return string.Empty;

        }
        public static string GetMaxCharacter(Guid MaxCharID)
        {
            if (MaxCharID == new Guid("37136A24-7456-42F3-B153-232E98D09112"))
                return "10";
            else if (MaxCharID == new Guid("65009F0A-CD91-4033-8D1A-2D09A38B9BCF"))
                return "20";
            else if (MaxCharID == new Guid("95507D6E-807D-46BC-8E2B-73DDE86F1A87"))
                return "40";
            else if (MaxCharID == new Guid("E96D0ABD-37FC-4660-BC42-A6FE41DF6D7C"))
                return "30";
            else if (MaxCharID == new Guid("CD02DA0C-BAAD-432C-A7BB-FC2B9D9B27CA"))
                return "50";
            return string.Empty;

        }
        public static string GetTextType(Guid TextTypeID)
        {
            if (TextTypeID == new Guid("B0443A47-89C3-4826-BECC-378D81738D03"))
                return "Numeric";
            else if (TextTypeID == new Guid("26C0ACEA-3CC8-43D6-A255-A870A8524A77"))
                return "Text";
            else if (TextTypeID == new Guid("8348E5CD-59EA-4A77-8436-298553D286BD"))
                return "Date";
            else if (TextTypeID == new Guid("DCBBE75C-FDEC-472C-AE25-2C42ADFB3F5D"))
                return "SSN";
            else if (TextTypeID == new Guid("5121246A-D9AB-49F4-8717-4EF4CAAB927B"))
                return "ZIP";
            else if (TextTypeID == new Guid("1AD2D4EC-4593-435E-AFDD-F8A90426DE96"))
                return "Email";
            else if (TextTypeID == new Guid("88A0B11E-5810-4ABF-A8B6-856C436E7C49"))
                return "Alphabet";
            //else if (TextTypeID == new Guid("81EAA78E-55FC-4843-8593-D1D671D6ACD7"))
            //    return "Custom";
            return string.Empty;
        }
        public static string GetFontName(Guid FontID)
        {
            if (FontID == new Guid("1AB25FA7-A294-405E-A04A-3B731AD795AC"))
                return "Arial";
            else if (FontID == new Guid("1875C58D-52BD-498A-BE6D-433A8858357E"))
                return "Cambria";
            else if (FontID == new Guid("D4A45ECD-3865-448A-92FA-929C2295EA34"))
                return "Courier";
            else if (FontID == new Guid("956D8FD3-BB0F-4E30-8E55-D860DEABB346"))
                return "Times New Roman";

            return string.Empty;

        }
    }
}
