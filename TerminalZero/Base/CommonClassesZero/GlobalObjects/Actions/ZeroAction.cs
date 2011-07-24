using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ZeroAction : ZeroActionDelegate
    {
        public string Name { get; private set; }
        public string Alias { get; private set; }
        public string RuleToSatisfyName { get; private set; }
        protected List<ActionParameterBase> Parameters { get; set; }
        public bool IsOnMenu { get; protected set; }
        public bool IsOnMainPage { get; protected set; }
        
        private bool _canExecute;
        
        public ZeroAction(string name, Action<object> executeAction, string ruleToSatisfy = null, bool isOnMenu = true)
            :base(executeAction)
        {
            Name = name;
            Parameters = new List<ActionParameterBase>();
            IsOnMenu = isOnMenu;
            RuleToSatisfyName = ruleToSatisfy;
        }

        #region ICommand Members

        public override bool CanExecute(object parameter)
        {
            var sb = new StringBuilder();

            _canExecute = ValidateActionParams(sb);

            if (_canExecute && Predicate != null)
            {
                _canExecute = Predicate(this);
            }
            else if (_canExecute && RuleToSatisfyName != null)
            {
                _canExecute = false;
            }
            
            if(!_canExecute)
            {
                Trace.TraceWarning("Action {0} cannot execute - message {1}",Action.Method,sb);
            }
            return _canExecute;
        }

        public override void Execute(object parameter)
        {
            try
            {
                if (_canExecute)
                {
                    Action(parameter);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Ocurrio un error en la ejecución del comando {0}, ERROR - {1}", Name, ex);
                if (Terminal.Instance.Client != null && Terminal.Instance.Client.Notifier != null) 
                    Terminal.Instance.Client.Notifier.SendNotification("Ocurrio un error en la ejecución, por favor\n contacte al administrador del sistema.");
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
            foreach (var item in Parameters)
            {
                if (!Terminal.Instance.Session.SessionParams.ContainsKey(item.Name) && item.IsMandatory)
                {
                    ret = false;
                    result.AppendLine("Unasigned Parameter '" + item.Name + "'");
                }
            }
            return ret;
        }

        public override event EventHandler CanExecuteChanged;

        public override void RaiseCanExecuteChanged()
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

        public void RaiseExecuted()
        {
            base.OnExecuted();
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

        public void AddParam(object key, bool isMandatory)
        {
            Parameters.Add(new ActionParameterBase(key.ToString(),isMandatory,false));
        }
    }
}
