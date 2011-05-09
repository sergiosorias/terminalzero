//using System;
//using System.Windows;
//using System.Windows.Input;
//using ZeroCommonClasses.Interfaces;

//namespace ZeroCommonClasses.GlobalObjects
//{
//    public sealed class NullAction : ICommand
//    {
//        #region ICommand Members

//        public bool CanExecute(object parameter)
//        {
//            return false;
//        }

//        public event EventHandler CanExecuteChanged;

//        public void Execute(object parameter)
//        {
            
//        }

//        #endregion
//    }

//    public class NullActionSource : ICommandSource
//    {
//        #region ICommandSource Members

//        private NullAction action = new NullAction();

//        public virtual ICommand Command
//        {
//            get { return action; }
//        }

//        public virtual object CommandParameter
//        {
//            get { return null; }
//        }

//        public virtual IInputElement CommandTarget
//        {
//            get { return null; }
//        }

//        #endregion
//    }

//    public abstract class ActionsSourceBase : IMenuCommandSource
//    {
//        #region IMenuCommandSource Members

//        private NullActionSource nullActionSource = new NullActionSource();

//        public virtual ICommandSource Save
//        {
//            get { return nullActionSource; }
//        }

//        public virtual ICommandSource Cancel
//        {
//            get { return nullActionSource; }
//        }

//        public virtual ICommandSource New
//        {
//            get { return nullActionSource; }
//        }

//        public virtual ICommandSource Print
//        {
//            get { return nullActionSource; }
//        }

//        #endregion
//    }
//}
