using System.Collections.ObjectModel;
using System.Linq;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroGUI;
using ZeroMasterData.Pages;

namespace ZeroMasterData.Presentation
{
    public class ProductsUpdateViewModel : ViewModelGui
    {
        private readonly ProductGroup allGroup = new ProductGroup {Code = -1,Name = "Todos los grupos"};
        private ProductGroup selectedProductGroup;

        public ProductGroup SelectedProductGroup
        {
            get { return selectedProductGroup; }
            set
            {
                if (selectedProductGroup != value)
                {
                    selectedProductGroup = value;
                    if (SelectedProductGroup == allGroup)
                        itemsCount = BusinessContext.Instance.Model.Products.Count();
                    else 
                        ItemsCount = BusinessContext.Instance.Model.Products.Where(p => p.Group1.Value == value.Code).Count();

                    OnPropertyChanged("SelectedProductGroup");
                }
            }
        }

        private ObservableCollection<ProductGroup> productGroupList;

        public ObservableCollection<ProductGroup> ProductGroupList
        {
            get
            {
                if(productGroupList==null)
                {
                    productGroupList =
                        new ObservableCollection<ProductGroup>(BusinessContext.Instance.Model.ProductGroups);
                    productGroupList.Insert(0, allGroup);
                }
                return productGroupList;
            }
            set
            {
                if (productGroupList != value)
                {
                    productGroupList = value;
                    OnPropertyChanged("ProductGroupList");
                }
            }
        }

        private double percentage;

        public double Percentage
        {
            get { return percentage; }
            set
            {
                if (percentage != value)
                {
                    percentage = value;
                    OnPropertyChanged("Percentage");
                }
            }
        }

        private long itemsCount;

        public long ItemsCount
        {
            get
            {
                return itemsCount;
            }
            set
            {
                if (itemsCount != value)
                {
                    itemsCount = value;
                    OnPropertyChanged("ItemsCount");
                }
            }
        }
        
        public ProductsUpdateViewModel() : base(new ProductsUpdate())
        {
            SelectedProductGroup = allGroup;
        }

        public override bool CanAccept(object parameter)
        {
            if (SelectedProductGroup == allGroup)
            {
                foreach (Price price in BusinessContext.Instance.Model.Products.Select(p=>p.Price1))
                {
                    price.Value += (price.Value * (percentage / 100));
                }
            }
            else
            {
                foreach (Price price in BusinessContext.Instance.Model.Products.Where(p => p.Group1.Value == SelectedProductGroup.Code).Select(p => p.Price1))
                {
                    price.Value += (price.Value*(percentage/100));
                }
            }

            return true;

        }
    }
}
