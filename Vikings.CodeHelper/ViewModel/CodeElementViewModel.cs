using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vikings.CodeHelper.ViewModel
{
    public class CodeElementViewModel : ObservableCollection<CodeElementViewModel>
    {
        const string codeSiteName = "Raize.CodeSiteLogging";
        static CodeImport codeSiteCodeImport;

        public CodeElementViewModel(CodeElement codeElement)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            CodeElement = codeElement;
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
                ExistsCodeSite = false;
                //IsChecked = ExistsCodeSite;
            }
        }

        internal void LoadChildren(CodeElements codeElements)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (CodeElement item in codeElements) LoadChild(item);
        }

        private void LoadChild(CodeElement item)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.OfType<CodeElementViewModel>().ToList().ForEach(f => f.Parent = this);
            e.OldItems?.OfType<CodeElementViewModel>().ToList().ForEach(f => f.Parent = null);
            base.OnCollectionChanged(e);
        }

        public CodeElement CodeElement { get; }

#pragma warning disable VSTHRD010 // 在主线程上调用单线程类型
        public CodeFunction CodeFunction => CodeElement as CodeFunction;
#pragma warning restore VSTHRD010 // 在主线程上调用单线程类型

        public bool ExistsCodeSite { get; }

        public bool IsCodeFunction => CodeFunction != null;

        public string Name { get; set; }

        public bool? IsChecked { get; set; }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }
    }
}
