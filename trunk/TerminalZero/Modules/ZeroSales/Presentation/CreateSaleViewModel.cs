using System.Data.Objects.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroGUI.Reporting;

namespace ZeroSales.Presentation
{
    public class CreateSaleViewModel : ViewModelGui
    {
        private int saleType = -1;
        private SaleHeader saleHeader;

        public SaleHeader SaleHeader
        {
            get { return saleHeader; }
            set { saleHeader = value;
                OnPropertyChanged("SaleHeader");
            }
        }

        protected override void PrintCommandExecution(object parameter)
        {
            base.PrintCommandExecution(parameter);
            ReportBuilder.Create("Lista de productos",
                BusinessContext.Instance.ModelManager.Products.OrderBy(product => product.Name)
                    .Select(product =>
                        new
                        {
                            Codigo = product.MasterCode,
                            Nombre = product.Name,
                            Precio = "$ " + SqlFunctions.StringConvert(product.Price1.Value),
                            Activo = product.Enable
                        }).ToList(),
                new ReportColumnInfo("Código", new GridLength(150)),
                new ReportColumnInfo("Nombre", new GridLength(100, GridUnitType.Star)),
                new ReportColumnInfo("Precio", new GridLength(100)));
        }

        public CreateSaleViewModel(NavigationBasePage view, int saleType)
            :base(view)
        {
            this.saleType = saleType;
            SaleHeader = new SaleHeader(BusinessContext.Instance.ModelManager.SaleTypes.First(st => st.Code == saleType));
        }
    }
}
