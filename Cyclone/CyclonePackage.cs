using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using AV.Cyclone.Service;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace AV.Cyclone
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid(GuidList.guidCyclonePkgString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class ExamplesPackage : Package
    {
        private static DTE2 _dte;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public ExamplesPackage()
        {
            Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidCyclonePkgCmdSet, (int)0x100);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var txtMgr =
                (IVsTextManager)GetService(typeof(SVsTextManager));
            var mustHaveFocus = 1;
            IVsTextView vTextView;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

            var cycloneStateManager = GetCycloneService();
            cycloneStateManager.StartCyclone(vTextView);
        }

        private IWpfTextViewHost GetIWpfTextViewHost()
        {
            // get an instance of IVsTextManager
            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
            IVsTextView vTextView = null;
            int mustHaveFocus = 1;
            // get the active view from the TextManager
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

            // cast as IVsUSerData
            IVsUserData userData = vTextView as IVsUserData;
            if (userData == null)
            {
                Trace.WriteLine("No text view is currently open");
                return null;
            }

            IWpfTextViewHost viewHost;
            object holder;
            // get the IWpfTextviewHost using the predefined guid for it
            Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            // convert to IWpfTextviewHost
            viewHost = (IWpfTextViewHost)holder;
            return viewHost;
        }

        private ICycloneService GetCycloneService()
        {
            // get an instance of the associated IWpfTextViewHost
            IWpfTextViewHost viewHost = GetIWpfTextViewHost();
            if (viewHost == null)
            {
                return null;
            }
            //var name = Dte.ActiveDocument.ProjectItem.ContainingProject.Name;

            ICycloneService cycloneService = viewHost.TextView.Properties.GetOrCreateSingletonProperty
                (() => new CycloneService());

            return cycloneService;
        }

        internal static DTE2 Dte
        {
            get { return _dte ?? (_dte = ServiceProvider.GlobalProvider.GetService(typeof (DTE)) as DTE2); }
        }

        #endregion

    }
}
