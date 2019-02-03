using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using NuGet.VisualStudio;
using System;

namespace Vikings.CodeHelper.ViewModel
{
    public static class NuGetExtension
    {
        public static bool InstallPackage(this Project project, string packageId)
        {
            try
            {
                var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
                IVsPackageInstallerServices installerServices = componentModel.GetService<IVsPackageInstallerServices>();
                if (!installerServices.IsPackageInstalled(project, packageId))
                {
                    var installer = componentModel.GetService<IVsPackageInstaller>();
                    installer.InstallPackage(null, project, packageId, (Version)null, false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
