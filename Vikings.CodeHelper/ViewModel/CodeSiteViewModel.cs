using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System;
using System.Diagnostics;
using System.Linq;
using VSLangProj;

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
                CodeElementViewModel.CodeSiteCodeImport = null;
                CodeElementViewModel.LoadChildren(dte.ActiveDocument.ProjectItem.FileCodeModel.CodeElements);
                CodeElementViewModel.All().Where(f => f.IsCodeFunction).ToList().ForEach(f => f.IsChecked = f.ExistsCodeSite);
            }
            CodeElementViewModel.Sort();
        }

        public bool IsEnabled => CodeElementViewModel.Any();

        string findText;
        public string FindText
        {
            get => findText;
            set
            {
                if (findText == value) return;
                findText = value;
                Find();
            }
        }

        void Find(bool? isNext = null)
        {
            var current = CodeElementViewModel.All().SingleOrDefault(f => f.IsSelected) ?? CodeElementViewModel.First();
            if (!isNext.HasValue)
            {
                if (current.Name.ToLower().Contains(FindText.ToLower())) return;
                else isNext = true;
            }
            var list = CodeElementViewModel.All().SkipWhile(f => f != current);
            list = list.Concat(CodeElementViewModel.All().Except(list)).Skip(1);
            if (isNext == false) list = list.Reverse();
            var find = list.FirstOrDefault(f => f.Name.ToLower().Contains(FindText.ToLower()));
            if (find != null) find.IsSelected = true;
        }

        public bool IncludeCatch
        {
            get => Properties.Settings.Default.IncludeCatch;
            set => Properties.Settings.Default.IncludeCatch = value;
        }

        RelayCommand okCommand;
        public RelayCommand OkCommand => okCommand ?? (okCommand = new RelayCommand(() =>
        {
            dte.UndoContext.Open("CodeSite方法跟踪器");
            try
            {
                Project prj = dte.ActiveDocument.ProjectItem.ContainingProject;
                if (prj.Kind == PrjKind.prjKindCSharpProject)//是C#项目
                {
                    VSProject vsp = (VSProject)prj.Object;
                    if (vsp.References.Find(CodeElementViewModel.CodeSiteName) == null)
                        vsp.References.Add(CodeElementViewModel.CodeSiteName);//添加本地引用
                    prj.InstallPackage("Vikings.CodeSite.Extensions");//添加NuGet包
                }
                if (CodeElementViewModel.CodeSiteCodeImport == null)//添加using
                {
                    FileCodeModel2 fcm2 = (FileCodeModel2)dte.ActiveDocument.ProjectItem.FileCodeModel;
                    fcm2.AddImport(CodeElementViewModel.CodeSiteName, 0, null);
                }
                CodeElementViewModel.All().Where(f => f.IsCodeFunction && f.ExistsCodeSite != f.IsChecked).ToList().ForEach(f =>
                {
                    Debug.Assert(f.IsChecked.HasValue);
                    if (f.IsChecked == true)
                        f.CodeFunction.AddCodeSite();
                    else
                        f.CodeFunction.DeleteCodeSite();
                });
                Properties.Settings.Default.Save();
            }
            finally
            {
                dte.UndoContext.Close();
            }
        }));

        RelayCommand<bool> findCommand;
        public RelayCommand<bool> FindCommand => findCommand ?? (findCommand = new RelayCommand<bool>(f => Find(f)) { IsEnabled = IsEnabled });

        RelayCommand<bool> traceAllCommand;
        public RelayCommand<bool> TraceAllCommand => traceAllCommand ?? (traceAllCommand = new RelayCommand<bool>(trace =>
        {
            CodeElementViewModel.ToList().ForEach(f => f.IsChecked = trace);
        })
        { IsEnabled = IsEnabled });

        RelayCommand<bool> expandAllCommand;
        public RelayCommand<bool> ExpandAllCommand => expandAllCommand ?? (expandAllCommand = new RelayCommand<bool>(expand =>
        {
            CodeElementViewModel.All().ToList().ForEach(f => f.IsExpanded = expand);
        })
        { IsEnabled = IsEnabled });
    }
}
