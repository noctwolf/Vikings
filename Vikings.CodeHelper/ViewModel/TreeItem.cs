using EnvDTE;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vikings.CodeHelper.ViewModel
{
    public class TreeItem : ObservableCollection<TreeItem>
    {
        public TreeItem Parent { get; private set; }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) e.NewItems.OfType<TreeItem>().ToList().ForEach(f => f.Parent = this);
            base.OnCollectionChanged(e);
        }

        public CodeElement CodeElement { get; set; }

        public bool ExistsCodeSite { get; private set; }

        public bool IsCodeFunction => CodeElement is CodeFunction;

        public bool IsChecked { get; set; }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }
    }
}
