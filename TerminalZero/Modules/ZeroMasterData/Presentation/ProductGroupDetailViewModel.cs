using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Presentation
{
    public class ProductGroupDetailViewModel : ViewModelGui
    {
        private ProductGroup currentProductGroup;

        public ProductGroup CurrentProductGroup
        {
            get { return currentProductGroup; }
            set
            {
                if (currentProductGroup != value)
                {
                    currentProductGroup = value;
                    OnPropertyChanged("CurrentProductGroup");
                }
            }
        }
        
        public ProductGroupDetailViewModel() 
            : base(new ProductGroupDetail())
        {
            CurrentProductGroup = ProductGroup.CreateProductGroup(BusinessContext.Instance.Model.ProductGroups.Count());
            ViewHeader = Properties.Resources.NewGroup;
        }

        public ProductGroupDetailViewModel(ProductGroup productGroup)
            : base(new ProductGroupDetail())
        {
            base.View.ControlMode = ZeroCommonClasses.Interfaces.ControlMode.Update;
            CurrentProductGroup = productGroup;
            ViewHeader = Properties.Resources.EditGroup;
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            if (ret)
            {
                var valid = ContextExtentions.ValidateEntity(CurrentProductGroup);
                if (!valid.IsValid)
                {
                    Terminal.Instance.Client.ShowDialog(String.Join("\n", valid.Errors), "Error", null, ZeroCommonClasses.GlobalObjects.MessageBoxButtonEnum.OK);
                }
                ret = valid.IsValid;
                if(ret)
                {
                    if(View.ControlMode != ZeroCommonClasses.Interfaces.ControlMode.Update)
                        BusinessContext.Instance.Model.ProductGroups.AddObject(CurrentProductGroup);
            
                    BusinessContext.Instance.Model.SaveChanges(SaveOptions.AcceptAllChangesAfterSave, true);
                }
            }
            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            if(CurrentProductGroup.EntityState == System.Data.EntityState.Modified)
                BusinessContext.Instance.Model.Refresh(RefreshMode.StoreWins, CurrentProductGroup);

            return base.CanCancel(parameter);
        }

    }
}
