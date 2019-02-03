﻿using EnvDTE;
using Microsoft;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Vikings.CodeHelper.View;
using Task = System.Threading.Tasks.Task;

namespace Vikings.CodeHelper
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CodeSiteCmd
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("497d5c2b-5985-4ad0-a581-eb084e00195b");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeSiteCmd"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private CodeSiteCmd(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CodeSiteCmd Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        //private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in CodeSiteCmd's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new CodeSiteCmd(package, commandService);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void Execute(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (await CheckCodeModelAsync()) new CodeSiteWindow().ShowModal();
            //string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            //string title = "CodeSiteCmd";

            //// Show a message box to prove we were here
            //VsShellUtilities.ShowMessageBox(
            //    this.package,
            //    message,
            //    title,
            //    OLEMSGICON.OLEMSGICON_INFO,
            //    OLEMSGBUTTON.OLEMSGBUTTON_OK,
            //    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        async Task<bool> CheckCodeModelAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                DTE dte = await package.GetServiceAsync(typeof(DTE)) as DTE;
                Assumes.Present(dte);
                Document doc = dte.ActiveDocument;
                if (doc == null)
                    ShowMessage("没有活动文档");
                else
                {
                    FileCodeModel fcm = doc.ProjectItem.FileCodeModel;
                    if (fcm == null)
                        ShowMessage("活动文档没有代码模型");
                    else if (!fcm.Language.Equals(CodeModelLanguageConstants.vsCMLanguageCSharp))
                        ShowMessage("代码的编程语言不是C#语言");
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                //ex.SendCodeSite();
                ShowMessage("未知异常\r\n" + ex.Message);
            }
            return false;
        }

        void ShowMessage(string message)
        {
            VsShellUtilities.ShowMessageBox(package, message, "加载文档",
                OLEMSGICON.OLEMSGICON_CRITICAL, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
