// Credits to: 
// http://grokys.blogspot.fr/2010/07/mvvm-and-multiple-selection-part-iii.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Kanji.Interface.Utilities
{
    public class MultiSelectCollectionView<T> : ListCollectionView, IMultiSelectCollectionView
    {
        public delegate void SelectionChangedHandler(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e);
        public event SelectionChangedHandler SelectionChanged;

        public MultiSelectCollectionView(IList list)
            : base(list)
        {
            SelectedItems = new ObservableCollection<T>();
        }

        void IMultiSelectCollectionView.AddControl(Selector selector)
        {
            this.controls.Add(selector);
            SetSelection(selector);
            selector.SelectionChanged += control_SelectionChanged;
        }

        void IMultiSelectCollectionView.RemoveControl(Selector selector)
        {
            if (this.controls.Remove(selector))
            {
                selector.SelectionChanged -= control_SelectionChanged;
            }
        }

        public ObservableCollection<T> SelectedItems { get; private set; }

        void SetSelection(Selector selector)
        {
            MultiSelector multiSelector = selector as MultiSelector;
            ListBox listBox = selector as ListBox;

            if (multiSelector != null)
            {
                multiSelector.SelectedItems.Clear();

                foreach (T item in SelectedItems)
                {
                    multiSelector.SelectedItems.Add(item);
                }
            }
            else if (listBox != null)
            {
                listBox.SelectedItems.Clear();

                foreach (T item in SelectedItems)
                {
                    listBox.SelectedItems.Add(item);
                }
            }
        }

        void control_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!this.ignoreSelectionChanged)
            {
                bool changed = false;

                this.ignoreSelectionChanged = true;

                try
                {
                    foreach (T item in e.AddedItems)
                    {
                        if (!SelectedItems.Contains(item))
                        {
                            // Was not selected. Add.
                            SelectedItems.Add(item);
                            changed = true;
                        }
                        else
                        {
                            // Was already selected. Remove.
                            SelectedItems.Remove(item);
                            changed = true;
                        }
                    }

                    foreach (T item in e.RemovedItems)
                    {
                        if (!SelectedItems.Remove(item))
                        {
                            //e.AddedItems.Add(item);
                            //e.RemovedItems.Remove(item);
                            SelectedItems.Add(item);
                        }

                        changed = true;
                    }

                    if (changed)
                    {
                        foreach (Selector control in this.controls)
                        {
                            if (control != sender)
                            {
                                SetSelection(control);
                            }
                        }

                        if (SelectionChanged != null)
                        {
                            SelectionChanged(this, null);
                        }
                    }
                }
                finally
                {
                    this.ignoreSelectionChanged = false;
                }
            }
        }

        bool ignoreSelectionChanged;
        List<Selector> controls = new List<Selector>();
    }  
}
