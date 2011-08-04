using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroStock.Pages;
using ZeroStock.Properties;

namespace ZeroStock.Presentation
{
    public class ReturnReasonSelectionViewModel : ViewModelGui
    {
        #region Properties
        
        private ReturnReason returnReason;

        public ReturnReason ReturnReason
        {
            get { return returnReason; }
            set
            {
                if (returnReason != value)
                {
                    if (value != null)
                    {
                        ReturnReasonName = value.Name;
                    }
                    returnReason = value;
                    OnPropertyChanged("ReturnReason");
                }
            }
        }
        
        private TerminalTo selectedTerminal;

        public TerminalTo SelectedTerminal
        {
            get { return selectedTerminal; }
            set
            {
                if (selectedTerminal != value)
                {
                    selectedTerminal = value;
                    OnPropertyChanged("SelectedTerminal");
                }
            }
        }

        private ObservableCollection<TerminalTo> terminalCollection;

        public ObservableCollection<TerminalTo> TerminalCollection
        {
            get
            {
                if(terminalCollection==null)
                {
                    terminalCollection = new ObservableCollection<TerminalTo>(BusinessContext.Instance.Model.TerminalToes);
                    SelectedTerminal = terminalCollection.FirstOrDefault();
                }
                return terminalCollection;
            }
            set
            {
                if (terminalCollection != value)
                {
                    terminalCollection = value;
                    OnPropertyChanged("TerminalCollection");
                }
            }
        }
        
        private ObservableCollection<ReturnReason> returnReasonCollection;

        public ObservableCollection<ReturnReason> ReturnReasonCollection
        {
            get
            {
                return returnReasonCollection??(returnReasonCollection = new ObservableCollection<ReturnReason>(BusinessContext.Instance.Model.ReturnReasons));
            }
            set
            {
                if (returnReasonCollection != value)
                {
                    returnReasonCollection = value;
                    OnPropertyChanged("ReturnReasonCollection");
                }
            }
        }

        private string returnReasonName;

        public string ReturnReasonName
        {
            get { return returnReasonName; }
            set
            {
                if (returnReasonName != value)
                {
                    returnReasonName = value;
                    OnPropertyChanged("ReturnReasonName");
                }
            }
        }
        
        #endregion

        public ReturnReasonSelectionViewModel() 
            : base(new ReturnReasonSelectionView())
        {
            
        }

        public override bool CanAccept(object parameter)
        {
            if(string.IsNullOrWhiteSpace(ReturnReasonName))
            {
                return false;
            }
            string name = ReturnReasonName.Trim();
            ReturnReason = BusinessContext.Instance.Model.ReturnReasons.FirstOrDefault(r => r.Name == name);
            if(ReturnReason == null)
            {
                ReturnReason = new ReturnReason(Terminal.Instance.Code) {Name = name};
            }
            return base.CanAccept(parameter);
        }
    }
}
