using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;
using Terminal = ZeroCommonClasses.Terminal;

namespace ZeroConfiguration.Presentantion
{
    public class MainViewModel : ViewModelBase
    {
        private MainViewActionItem selectedAction;

        public MainViewActionItem SelectedAction
        {
            get { return selectedAction; }
            set
            {
                if (selectedAction != value)
                {
                    selectedAction = value;
                    OnPropertyChanged("SelectedAction");
                }
            }
        }

        private ObservableCollection<MainViewActionItem> shortcutAction;

        public ObservableCollection<MainViewActionItem> ShortcutActions
        {
            get { return shortcutAction ?? (shortcutAction = LoadActions()); }
            set
            {
                if (shortcutAction != value)
                {
                    shortcutAction = value;
                    OnPropertyChanged("ShortcutActions");
                }
            }
        }

        private ObservableCollection<MainViewActionItem> LoadActions()
        {
            var actions = new List<ZeroAction>();
            var ret = new string[] { };
            using (var conf = new ConfigurationModelManager())
            {
                TerminalProperty prop = conf.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == Terminal.Instance.TerminalCode && tp.Code == SystemProperty.HomeShortcut.Code);
                if (prop != null)
                {
                    if (prop.LargeValue != null)
                    {
                        ret = prop.LargeValue.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        ZeroAction act = null;
                        foreach (var item in ret)
                        {
                            string[] actParts = item.Split('|');
                            if (Terminal.Instance.Session.Actions.Exists(actParts[0].Trim()))
                            {
                                act = Terminal.Instance.Session.Actions[actParts[0].Trim()];
                                act.SetAlias(actParts);
                                actions.Add(act);
                            }
                        }
                    }
                }
            }
            return new ObservableCollection<MainViewActionItem>(actions.Select(a => new MainViewActionItem(a)));
        }

        public class MainViewActionItem
        {
            public MainViewActionItem(ZeroAction action)
            {
                Action = action;
                IsEnabled = Action.CanExecute(null);
                action.CanExecuteChanged += (o, e) => { IsEnabled = Action.CanExecute(null); };
            }

            public ZeroAction Action { get; private set; }
            public bool IsEnabled { get; private set; }  
        }
    
    }
}
