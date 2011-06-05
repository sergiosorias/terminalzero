using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ZeroCommonClasses.Interfaces;
using Application = System.Windows.Application;
using DataGrid = System.Windows.Controls.DataGrid;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Panel = System.Windows.Controls.Panel;

namespace ZeroGUI
{
    public class LazyLoadingListControlUpgrade : DataGrid
    {
        public LazyLoadingListControlUpgrade()
        {
            LazyLoadEnable = true;
            Style = (Style) Application.Current.Resources["dataGridStyle"];
            Loaded += LazyLoadingListControl_Loaded;
            PreviewKeyDown += LazyLoadingListControl_PreviewKeyDown;
        }

        protected bool IsInDesignMode
        {
            get { return DesignerProperties.GetIsInDesignMode(this); }
        }

        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); OnControlModeChanged(value); }
        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode", typeof(ControlMode), typeof(LazyLoadingListControlUpgrade), null);
        
        public bool LazyLoadEnable
        {
            get { return (bool)GetValue(LazyLoadEnableProperty); }
            set { SetValue(LazyLoadEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LazyLoadEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LazyLoadEnableProperty =
            DependencyProperty.Register("LazyLoadEnable", typeof(bool), typeof(LazyLoadingListControlUpgrade), null);

        protected virtual void OnControlModeChanged(ControlMode newMode)
        {

        }

        private IEnumerable _fullItemList;
        private WaitCursorSimple waitCursor = new WaitCursorSimple();
        
        #region Events
        public event EventHandler ItemsLoaded;
        #endregion

        private void LazyLoadingListControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (ItemsSource != null)
            {
                IEnumerable items = ItemsSource;
                ItemsSource = null;
                StartListLoad(items);
            }
        }

        private void LazyLoadingListControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.D)
                {
                    OnKeyboardDeleteKeysPressed();
                }
                else if (e.Key == Key.Enter)
                {
                    OnKeyboardSelectItemKeysPressed();
                }
            }
        }
        
        protected virtual void OnKeyboardDeleteKeysPressed()
        {
            
        }

        protected virtual void OnKeyboardSelectItemKeysPressed()
        {

        }

        private void StartListLoad(IEnumerable items)
        {
            if (!IsInDesignMode)
            {
                _fullItemList = items ?? new ArrayList();
                if (_fullItemList != null)
                {
                    if (LazyLoadEnable)
                    {
                        var panel = Parent as Panel;
                        if (panel != null)
                        {
                            if (panel is Grid)
                            {
                                var grid = ((Grid) panel);
                                if (grid.ColumnDefinitions.Count > 0)
                                    waitCursor.SetValue(Grid.ColumnSpanProperty, grid.ColumnDefinitions.Count);
                                if (grid.RowDefinitions.Count > 0)
                                    waitCursor.SetValue(Grid.RowSpanProperty, grid.RowDefinitions.Count);
                            }
                            panel.Children.Add(waitCursor);
                        }

                        var loader = new BackgroundWorker();
                        loader.DoWork += loader_DoWork;
                        loader.RunWorkerCompleted += loader_RunWorkerCompleted;
                        loader.RunWorkerAsync();
                    }
                    else
                    {
                        FillList();
                    }
                }
            }
        }

        private void loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var P = Parent as Panel;
            if (P != null)
            {
                P.Children.Remove(waitCursor);
            }
        }

        private void loader_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(400);
            Dispatcher.BeginInvoke(new MethodInvoker(FillList), null);
        }

        private void FillList()
        {
            ItemsSource = _fullItemList;
            OnItemsLoaded();
        }

        private void OnItemsLoaded()
        {
            if (ItemsLoaded != null)
                ItemsLoaded(this, EventArgs.Empty);
        }

        #region Protected Methods
        
        #endregion

        #region Public Methods
        public virtual int ApplyFilter(string criteria, params object[] otherCriteriaObjects)
        {
            if (_fullItemList != null)
            {
                Items.Clear();

                foreach (var item in _fullItemList.OfType<ISelectable>().Where(p => p.Contains(criteria)))
                {
                    Items.Add(item);
                }

                if (otherCriteriaObjects != null)
                {
                    foreach (object other in otherCriteriaObjects)
                    {
                        if (other is DateTime)
                        {
                            var date = (DateTime) other;
                            foreach (var item in _fullItemList.OfType<ISelectable>().Where(p => p.Contains(date)))
                            {
                                Items.Add(item);
                            }
                        }
                    }
                }

                return Items.Count;
            }
            return 0;
        }

        public virtual void SelectItemByKey(EntityKey key)
        {
            if(key!=null)
            SelectedItem = Items.OfType<EntityObject>().FirstOrDefault(entObj => entObj.EntityKey.Equals(key));
            
        }

        public virtual void SelectItemByIndex(int index)
        {
            SelectedItem = Items[index];
        }

        public virtual void SelectItemByData(string data)
        {
            SelectedItem = _fullItemList.OfType<ISelectable>().FirstOrDefault(item => item.Contains(data));
        }

        public virtual void MoveNext()
        {
            if(SelectedIndex<= Items.Count)
            {
                SelectedIndex++;
            }
        }

        public virtual void MovePrevious()
        {
            if (SelectedIndex > 0)
                SelectedIndex--;
        }

        #endregion



        
    }
}
