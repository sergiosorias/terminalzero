using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects
{
    public enum ActionType
    {
        MenuItem = 0,
        MainViewButton = 1,
        BackgroudAction = 2,
    }

    public delegate void ZeroActionHandle(ZeroRule rule);
        
    public class ZeroAction : System.Windows.Input.ICommand
    {
        public event EventHandler Finished;
        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }

        public ActionType ActionType { get; private set; }
        public bool AlwaysVisible { get; set; }
        public bool Enabled { get { return CanExecute(null); } }
        public string Name { get; private set; }
        public string Alias { get; set; }
        public string RuleToSatisfyName { get; private set; }
        public ZeroRule RuleToSatisfy { get; set; }
        public ZeroActionHandle ActionDelegate { get; private set; }
        public List<ZeroActionParameterBase> Parameters { get; private set; }
        
        private bool _CanExecute = false;

        public ZeroAction(ActionType actionType, string name, ZeroActionHandle currentAction)
        {
            AlwaysVisible = false;
            ActionType = actionType;
            Name = name;
            ActionDelegate = currentAction;
            Parameters = new List<ZeroActionParameterBase>();
        }

        public ZeroAction(ActionType actionType, string name, ZeroActionHandle currentAction, string ruleToSatisfy)
            :this(actionType,name,currentAction)
        {
            RuleToSatisfyName = ruleToSatisfy;
        }

        private void OnCanExecuteChanged()
        {
            if (CanExecuteChanged!=null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (RuleToSatisfy != null)
                RuleToSatisfy.Check();

            if (RuleToSatisfy == null || (RuleToSatisfy.Satisfied.HasValue && RuleToSatisfy.Satisfied.Value))
                _CanExecute = true;
            else
                _CanExecute = false;

            return _CanExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_CanExecute)
            {
                ActionDelegate((ZeroRule)parameter);
                OnFinished();
            }
        }

        #endregion
    }
}
