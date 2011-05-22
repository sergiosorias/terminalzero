using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ZeroAction : ICommand
    {
        public event EventHandler Finished;
        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string RuleToSatisfyName { get; private set; }
        public Predicate<object> RuleToSatisfy { get; set; }
        public Action ExecuteAction { get; private set; }
        protected List<ActionParameterBase> Parameters { get; set; }
        public bool IsOnMenu { get; protected set; }
        public bool IsOnMainPage { get; protected set; }
        
        private bool _canExecute;
        
        public ZeroAction(string name, Action executeAction, string ruleToSatisfy, bool isOnMenu)
        {
            Name = name;
            ExecuteAction = executeAction;
            Parameters = new List<ActionParameterBase>();
            IsOnMenu = isOnMenu;
            RuleToSatisfyName = ruleToSatisfy;
        }

        #region ICommand Members

        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            StringBuilder sb;
            if (parameter is StringBuilder)
                sb = parameter as StringBuilder;
            else
                sb = new StringBuilder();

            _canExecute = ValidateActionParams(sb);

            if (_canExecute && RuleToSatisfy != null)
            {
                _canExecute = RuleToSatisfy(this);
            }
            else if (_canExecute && RuleToSatisfyName != null)
            {
                _canExecute = false;
            }
            
            ///TODO: log sb result if false
            return _canExecute;
        }

        public virtual void Execute(object parameter)
        {
            if (_canExecute)
            {
                ExecuteAction();
                OnFinished();
            }
        }

        public bool TryExecute()
        {
            bool ret = false;
            try
            {
                if (CanExecute(null))
                {
                    Execute(null);
                    ret = true;
                }
            }
            catch
            {

            }

            return ret;
        }

        #endregion

        private bool ValidateActionParams(StringBuilder result)
        {
            bool ret = true;
            ActionParameterBase obj = null;
            foreach (var item in Parameters)
            {
                obj = Terminal.Instance.Session[item.Name];
                if ((obj == null || obj.Value == null) && item.IsMandatory)
                {
                    ret = false;
                    result.AppendLine("UnasignedParameter '" + item.Name + "'");
                }
                obj = null;
            }

            return ret;
        }

        public virtual void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                bool canExecuteOld = _canExecute;
                CanExecute(null);
                if (canExecuteOld != _canExecute)
                {
                    CanExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        public void SetAlias(string[] nameParts)
        {
            if (nameParts.Length > 1)
            {
                Alias = nameParts[1].Trim();
            }
            else
            {
                Alias = nameParts[0].Substring(nameParts[0].LastIndexOf('@') + 1).Trim(); ;
            }
        }

        public void AddParam(string name, bool isMandatory)
        {
            Parameters.Add(new ActionParameterBase(name,isMandatory,false));
        }

        public void AddParam(Type type, bool isMandatory)
        {
            Parameters.Add(new ActionParameterBase(type, isMandatory,false));
        }

        
    }
}
