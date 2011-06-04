using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroConfiguration.Presentantion
{
    public class PropertiesViewModel : ViewModelGui
    {
        private ConfigurationModelManager dataProvider;

        public PropertiesViewModel(NavigationBasePage view)
            :base(view)
        {
            dataProvider = new ConfigurationModelManager();
        }

        private ICommand savePropertiesCommand;
        public ICommand SavePropertiesCommand
        {
            get { return savePropertiesCommand ?? (savePropertiesCommand = new ZeroActionDelegate(SaveChanges)); }
        }

        private Terminal selectedTerminal;
        public Terminal SelectedTerminal
        {
            get { return selectedTerminal; }
            set 
            { 
                selectedTerminal = value;
                if(!selectedTerminal.Modules.IsLoaded)
                    selectedTerminal.Modules.Load();
                OnPropertyChanged("SelectedTerminal");
            }

        }

        private bool areControlsEnable;
        public bool  AreControlsEnable
        {
            get { return areControlsEnable; }
            set 
            { 
                areControlsEnable = value;
                OnPropertyChanged("AreControlsEnable");
            }
        }
        
        public IEnumerable<Terminal> Terminals
        {
            get
            {
                IEnumerable<Terminal> result;
                if (View.ControlMode == ControlMode.ReadOnly)
                {
                    AreControlsEnable = false;
                    result = dataProvider.Terminals.Where(t => t.Code == ZeroCommonClasses.Terminal.Instance.TerminalCode);
                }
                else
                {
                    AreControlsEnable = true;
                    result = dataProvider.Terminals;    
                }
                
                return result;
            }
        }

        private void SaveChanges(object parameter)
        {
            dataProvider.SaveChanges();
        }

        
    }
}
