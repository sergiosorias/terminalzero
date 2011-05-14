using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using ZeroCommonClasses.Context;

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
        public ActionType ActionType { get; private set; }
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string RuleToSatisfyName { get; private set; }
        public Predicate<object> RuleToSatisfy { get; set; }
        public Action ExecuteAction { get; private set; }
        protected List<ActionParameterBase> Parameters { get; set; }

        private bool _canExecute;
        public ZeroAction(ActionType actionType, string name, Action executeAction)
        {
            ActionType = actionType;
            Name = name;
            ExecuteAction = executeAction;
            Parameters = new List<ActionParameterBase>();
        }

        public ZeroAction(ActionType actionType, string name, Action executeAction, string ruleToSatisfy)
            : this(actionType, name, executeAction)
        {
            RuleToSatisfyName = ruleToSatisfy;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged!=null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Members

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
                _canExecute = RuleToSatisfy(sb);
            }
            else if (_canExecute && RuleToSatisfyName != null)
            {
                _canExecute = false;
            }
            
            ///TODO: log sb result if false
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_canExecute)
            {
                ExecuteAction();
                OnFinished();
            }
        }

        #endregion

        private bool ValidateActionParams(StringBuilder result)
        {
            bool ret = true;
            ActionParameterBase obj = null;
            foreach (var item in Parameters)
            {
                if (Terminal.Instance.Session.SessionParams.ContainsKey(item.Name))
                    obj = Terminal.Instance.Session.SessionParams[item.Name];

                if ((obj == null || obj.Value == null) && item.IsMandatory)
                {
                    ret = false;
                    result.AppendLine("UnasignedParameter '" + item.Name + "'");
                }

                obj = null;
            }

            return ret;
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
