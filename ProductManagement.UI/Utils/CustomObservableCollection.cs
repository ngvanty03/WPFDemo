using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.UI.Utils
{
    public class CustomObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        public void ReplaceRange(IEnumerable<T> items)
        {
            _suppressNotification = true;  // Turn OFF binding

            Items.Clear();
            foreach (var item in items)
                Items.Add(item);

            _suppressNotification = false;  // Turn ON binding

            // Fire ONE single notification at the end
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset));
        }
    }
}
