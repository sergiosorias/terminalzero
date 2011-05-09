using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI.Classes;

namespace ZeroGUI
{
    public class LazyLoadingListControl : DataGrid
    {
        public LazyLoadingListControl()
        {
            LazyLoadEnable = true;
            Style = (Style) Application.Current.Resources["dataGridStyle"];
        }

        public ControlMode ControlMode
        {
            get { return (ControlMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); OnControlModeChanged(value); }

        }

        public bool LazyLoadEnable
        {
            get { return (bool)GetValue(LazyLoadEnableProperty); }
            set { SetValue(LazyLoadEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LazyLoadEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LazyLoadEnableProperty =
            DependencyProperty.Register("LazyLoadEnable", typeof(bool), typeof(LazyLoadingListControl), null);

        protected virtual void OnControlModeChanged(ControlMode newMode)
        {

        }

        // Using a DependencyProperty as the backing store for ControlMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register("ControlMode1", typeof(ControlMode), typeof(NavigationBasePage), null);
        
        
        private IEnumerable _fullItemList;
        private WaitCursorSimple waitCursor = new WaitCursorSimple();
        
        #region Events
        public event EventHandler<ItemActionEventArgs> ItemRemoving;
        public event EventHandler<ItemActionEventArgs> ItemRemoved;
        #endregion

        protected void StartListLoad(IEnumerable items)
        {
            _fullItemList = items ?? new ArrayList();
            if (_fullItemList!=null && !DesignerProperties.GetIsInDesignMode(this))
            {
                if (LazyLoadEnable)
                {
                    Panel panel = Parent as Panel;
                    if (panel != null)
                    {
                        if(panel is Grid)
                        {
                            Grid grid = ((Grid) panel);
                            if(grid.ColumnDefinitions.Count>0)
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

        private void loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Panel P = Parent as Panel;
            if (P != null)
            {
                P.Children.Remove(waitCursor);
            }
        }

        private void loader_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(400);
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(FillList), null);
        }

        private void FillList()
        {
            foreach (var item in _fullItemList)
            {
                Items.Add(item);
            }
        }

        private void _currentItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = SelectedItem as EntityObject;
        }

        #region Protected Methods
        
        protected void OnRemoving(ItemActionEventArgs e)
        {
            if (ItemRemoving != null)
            {
                ItemRemoving(this, e);
            }
        }

        protected void OnRemoved(ItemActionEventArgs e)
        {
            EventHandler<ItemActionEventArgs> handler = ItemRemoved;
            if (handler != null) handler(this, e);
        }
        #endregion

        #region Public Methods
        public virtual int ApplyFilter(string criteria, params object[] otherCriteriaObjects)
        {
           Items.Clear();

            foreach (var item in _fullItemList.OfType<ISelectable>().Where(p => p.Contains(criteria)))
            {
                Items.Add(item);
            }
            
            if(otherCriteriaObjects!=null)
            {
                foreach (object other in otherCriteriaObjects)
                {
                    if (other is DateTime)
                    {
                        var date = (DateTime)other;
                        foreach (var item in _fullItemList.OfType<ISelectable>().Where(p => p.Contains(date)))
                        {
                            Items.Add(item);
                        }
                    }
                }
            }

            return Items.Count;
        }

        public virtual void AddItem(EntityObject item)
        {
            Items.Add(item);
        }

        public virtual void TryRemoveItem(EntityObject item)
        {
            var args = new ItemActionEventArgs(item);
            OnRemoving(args);
            if (!args.Cancel)
            {
                Items.Remove(item);
                OnRemoved(args);
            }
        }

        public virtual void Clear()
        {
            Items.Clear();
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
