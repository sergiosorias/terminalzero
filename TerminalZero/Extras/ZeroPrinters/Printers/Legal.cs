namespace ZeroPrinters.Printers
{
    /// <summary>
    /// Wrapper for Legal printers
    /// </summary>
    public class Legal : SystemPrinter
    {
        public Legal(PrinterInfo info)
            :base(info)
        {
            IsExistanceMandatory = true;
        }

        #region Overrides of SystemPrinter

        public override bool IsOnLine
        {
            get;
            protected set;
        }

        #endregion
    }
}