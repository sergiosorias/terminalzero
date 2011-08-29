using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Input;

namespace SLFramework.ViewModel
{
    public abstract class ViewModel : ComplexObject
    {
        #region Statics

        
        private static bool? _isInDesignMode;

        /// <summary>
        /// Gets a value indicating whether the control is in design mode (running in Blend
        /// or Visual Studio).
        /// </summary>
        public static bool IsInDesignModeStatic
        {
            get
            {
                if (!_isInDesignMode.HasValue)
                {
#if SILVERLIGHT
                    _isInDesignMode = DesignerProperties.IsInDesignTool;
#else
            var prop = DesignerProperties.IsInDesignModeProperty;
            _isInDesignMode
                = (bool)DependencyPropertyDescriptor
                .FromProperty(prop, typeof(FrameworkElement))
                .Metadata.DefaultValue;
#endif
                }

                return _isInDesignMode.Value;
            }
        }
        #endregion

        #region Properties

        #endregion

        protected ViewModel()
        {
            if(!IsInDesignModeStatic)
                Initialize();
        }

        protected virtual void Initialize()
        {
            
        }

        protected void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
    }

    public class DelegateCommand : ICommand
    {
        private Action<object> action;
        private Predicate<object> predicate;

        public DelegateCommand(Action<object> action) : this(action, null)
        {
        }

        public DelegateCommand(Action<object> action, Predicate<object> predicate)
        {
            this.action = action;
            this.predicate = predicate;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter)
        {
            return predicate == null || predicate(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void InvokeCanExecuteChanged(EventArgs e)
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
