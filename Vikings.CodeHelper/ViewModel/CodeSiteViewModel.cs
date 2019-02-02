using Raize.CodeSiteLogging;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vikings.CodeHelper.ViewModel
{
    public class CodeSiteViewModel : ViewModelBase
    {
        DTE dte;

        public CodeElementViewModel CodeElementViewModel { get; } = new CodeElementViewModel(null);

        public CodeSiteViewModel()
        {
            dte = Package.GetGlobalService(typeof(DTE)) as DTE;
            if (dte == null)
            {
                CodeElementViewModel.Add(new CodeElementViewModel(null));
                CodeElementViewModel.Add(new CodeElementViewModel(null));
            }
            else
            {
                CodeElementViewModel.LoadChildren(dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElements);
                CodeElementViewModel.All().Where(f => f.IsCodeFunction).ToList().ForEach(f => f.IsChecked = f.ExistsCodeSite);
            }
        }
    }
}
