using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Vikings.CodeHelper.ViewModel
{
    public class CodeElementViewModel : ObservableCollection<CodeElementViewModel>
    {
        const string codeSiteName = "Raize.CodeSiteLogging";
        static CodeImport codeSiteCodeImport;

        protected virtual void RaisePropertyChanged([CallerMemberName]string propertyName = null) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public CodeElementViewModel(CodeElement codeElement)
        {
            CodeElement = codeElement;
            Name = Guid.NewGuid().ToString();
            if (CodeElement == null) return;
            Name = CodeElement.Name;
            if (CodeElement.Kind == vsCMElement.vsCMElementProperty && CodeElement is CodeProperty codeProperty)
            {
                if (codeProperty.Getter != null) LoadChild((CodeElement)codeProperty.Getter);
                if (codeProperty.Setter != null) LoadChild((CodeElement)codeProperty.Setter);
            }
            else if (CodeElement.Children != null) LoadChildren(CodeElement.Children);
            if (IsCodeFunction)
            {
                if (CodeFunction.MustImplement || !(CodeFunction.HasBody() || CodeFunction.HasExpressionBody()))
                {
                    CodeElement = null;
                    return;//跳过抽象方法和没有主体的方法
                }
                Name = CodeFunction.GetSetName() ?? Name;
                ExistsCodeSite = CodeFunction.ExistsCodeSite();
                //IsChecked = ExistsCodeSite;
            }
        }

        internal void LoadChildren(CodeElements codeElements)
        {
            foreach (CodeElement item in codeElements) LoadChild(item);
        }

        private void LoadChild(CodeElement item)
        {
            switch (item.Kind)
            {
                case vsCMElement.vsCMElementImportStmt:
                    if (item is CodeImport codeImport && codeImport.Namespace == codeSiteName)
                        codeSiteCodeImport = codeImport;
                    break;
                case vsCMElement.vsCMElementClass:
                case vsCMElement.vsCMElementProperty:
                case vsCMElement.vsCMElementNamespace:
                case vsCMElement.vsCMElementStruct:
                case vsCMElement.vsCMElementFunction:
                    var itemViewModel = new CodeElementViewModel(item);
                    if (itemViewModel.Any() || itemViewModel.IsCodeFunction) Add(itemViewModel);
                    break;
                default:
                    break;
            }
        }

        public CodeElementViewModel Parent { get; private set; }

        public IEnumerable<CodeElementViewModel> All()
        {
            foreach (var item in this)
            {
                yield return item;
                foreach (var r in item.All()) yield return r;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.OfType<CodeElementViewModel>().ToList().ForEach(f => f.Parent = this);
            e.OldItems?.OfType<CodeElementViewModel>().ToList().ForEach(f => f.Parent = null);
            base.OnCollectionChanged(e);
        }

        public CodeElement CodeElement { get; }

        public CodeFunction CodeFunction => CodeElement as CodeFunction;

        public bool ExistsCodeSite { get; }

        public bool IsCodeFunction => CodeFunction != null;

        public string Name { get; }

        bool inChecked = false;//抑制递归
        bool? isChecked;
        public bool? IsChecked
        {
            get => isChecked;
            set
            {
                if (isChecked == value || inChecked) return;
                inChecked = true;            ;
                isChecked = value;
                //设置子
                if (isChecked.HasValue) this.ToList().ForEach(f => f.IsChecked = isChecked);
                //设置父
                if (Parent != null)
                {
                    if (Parent.All(f => f.IsChecked == isChecked))
                        Parent.IsChecked = IsChecked;
                    else
                        Parent.IsChecked = null;
                }
                inChecked = false;
                RaisePropertyChanged();
            }
        }

        bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected == value) return;
                isSelected = value;
                RaisePropertyChanged();
            }
        }

        bool isExpanded = true;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded == value) return;
                isExpanded = value;
                RaisePropertyChanged();
            }
        }
    }
}
