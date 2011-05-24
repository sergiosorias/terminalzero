using System.Windows;
using System.Windows.Documents;

namespace ZeroGUI.Reporting
{
    public class ReportColumnInfo : TableColumn
    {
        public ReportColumnInfo(string label, GridLength width)
        {
            Name = label;
            Width = width;
        }
    }
}