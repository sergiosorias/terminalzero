using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ZeroCommonClasses.Interfaces;
using ZeroGUI.Classes;

namespace ZeroGUI
{
    public class ListNavigationControl : NavigationBasePage
    {
        public ListNavigationControl()
        {
            Loaded += ListNavigationControl_Loaded;        
        }

        private bool _startEmpty;
        private IEnumerable _fullItemList;
        private MultiSelector _currentItemList;
        private WaitCursorSimple waitCursor = new WaitCursorSimple();
        public EntityObject SelectedItem { get; protected set; }

        #region Events
        public event EventHandler<ItemActionEventArgs> ItemRemoving;
        public event EventHandler<ItemActionEventArgs> ItemRemoved;
        #endregion

        private void FillList()
        {
            foreach (var item in _fullItemList)
            {
                _currentItemList.Items.Add(item);
            }
            
        }

        private void ListNavigationControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_fullItemList!=null && !DesignerProperties.GetIsInDesignMode(this))
            {
                Panel P = _currentItemList.Parent as Panel;
                if (P != null)
                    P.Children.Add(waitCursor);
                
                var loader = new BackgroundWorker();
                loader.DoWork += loader_DoWork;
                loader.RunWorkerCompleted += loader_RunWorkerCompleted;
                loader.RunWorkerAsync();
            }
        }

        private void loader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Panel P = _currentItemList.Parent as Panel;
            if (P != null)
            {
                P.Children.Remove(waitCursor);
            }
        }

        private void loader_DoWork(object sender, DoWorkEventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(FillList), null);
        }

        private void _currentItemList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedItem = _currentItemList.SelectedItem as EntityObject;
        }

        #region Protected Methods
        protected void InitializeList(MultiSelector listToFill)
        {
            _startEmpty = true;
            InitializeList(listToFill, null);
        }

        protected void InitializeList(MultiSelector listToFill, IEnumerable items)
        {
            _fullItemList = items;
            _currentItemList = listToFill;
            if (_startEmpty)
            {
                _fullItemList = new ArrayList();
            }
            _currentItemList.SelectionChanged += _currentItemList_SelectionChanged;
            
        }

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
            _currentItemList.Items.Clear();

            foreach (var item in _fullItemList.OfType<ISelectable>().Where(p => p.Contains(criteria)))
            {
                _currentItemList.Items.Add(item);
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
                            _currentItemList.Items.Add(item);
                        }
                    }
                }
            }

            return _currentItemList.Items.Count;
        }

        public virtual void AddItem(EntityObject item)
        {
            _currentItemList.Items.Add(item);
        }

        public virtual void RemoveItem(EntityObject item)
        {
            var args = new ItemActionEventArgs(item);
            OnRemoving(args);
            if (!args.Cancel)
            {
                _currentItemList.Items.Remove(item);
                OnRemoved(args);
            }
        }

        public virtual void Clear()
        {
            _currentItemList.Items.Clear();
        }

        public virtual void SelectItemByKey(EntityKey key)
        {
            _currentItemList.SelectedItem =
                _currentItemList.Items.OfType<EntityObject>().FirstOrDefault(entObj => entObj.EntityKey == key);
        }

        public virtual void SelectItemByIndex(int index)
        {
            _currentItemList.SelectedItem = _currentItemList.Items[index];
        }

        #endregion

        
    }
}
