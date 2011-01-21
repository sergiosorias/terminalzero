using System;
using System.Collections.Generic;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects
{
    public enum ActionType
    {
        MenuItem = 0,
        MainViewButton = 1,
        BackgroudAction = 2,
    }

    public class ZeroAction : System.Windows.Input.ICommand
    {
        public event EventHandler Finished;
        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }
        public ZeroSession Session { get; private set; }
        public ActionType ActionType { get; private set; }
        public bool AlwaysVisible { get; set; }
        public string Name { get; private set; }
        public string Alias { get; set; }
        public string RuleToSatisfyName { get; private set; }
        public Predicate<object> RuleToSatisfy { get; set; }
        public Action ExecuteAction { get; private set; }
        public List<ZeroActionParameterBase> Parameters { get; private set; }
        private bool _canExecute;
        public ZeroAction(ZeroSession session, ActionType actionType, string name, Action executeAction)
        {
            AlwaysVisible = false;
            ActionType = actionType;
            Name = name;
            ExecuteAction = executeAction;
            Parameters = new List<ZeroActionParameterBase>();
            Session = session;
        }

        public ZeroAction(ZeroSession session, ActionType actionType, string name, Action executeAction, string ruleToSatisfy)
            : this(session, actionType, name, executeAction)
        {
            RuleToSatisfyName = ruleToSatisfy;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged!=null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            StringBuilder sb;
            if (parameter is StringBuilder)
                sb = parameter as StringBuilder;
            else
                sb = sb = new StringBuilder();

            if (RuleToSatisfyName == null && RuleToSatisfy == null)
            {
                _canExecute = true;
            }
            else if(RuleToSatisfyName != null && RuleToSatisfy == null)
            {
                _canExecute = false;
            }
            else
            {
                _canExecute = Session != null
                                  ? (ValidateActionParams(sb) && RuleToSatisfy.Invoke(sb))
                                  : RuleToSatisfy.Invoke(sb);
            }
            
            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_canExecute)
            {
                try
                {
                    ExecuteAction();
                    OnFinished();
                }
                catch (Exception ex)
                {
                    if (Session != null) Session.Notifier.SendNotification("Error: "+ex);
                    System.Diagnostics.Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceError,
                                                     string.Format("{2} on {1} throws-> {0}",ex,ExecuteAction.Method, ExecuteAction.Target.GetType()), "Error");
                }
            }
        }

        private bool ValidateActionParams(StringBuilder result)
        {
            bool ret = true;
            ZeroActionParameterBase obj = null;
            foreach (var item in Parameters)
            {
                if (Session.SessionParams.ContainsKey(item.Name))
                    obj = Session.SessionParams[item.Name];

                if ((obj == null || obj.Value == null) && item.IsMandatory)
                {
                    ret = false;
                    result.AppendLine("UnasignedParameter '" + item.Name + "'");
                }

                obj = null;
            }

            return ret;
        }

        #endregion
    }
}
