using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Based on https://github.com/diimdeep/VisualStudioFileOpenTool
namespace MemTraceTool
{
    class VisualStudioHelper
    {
        public static string GetVersionString(int visualStudioVersionNumber)
        {
            //  Source: http://www.mztools.com/articles/2011/MZ2011011.aspx
            switch (visualStudioVersionNumber)
            {
                case 13:
                    return "VisualStudio.DTE.12.0";
                case 12:
                    return "VisualStudio.DTE.11.0";
                case 10:
                    return "VisualStudio.DTE.10.0";
                case 8:
                    return "VisualStudio.DTE.9.0";
                case 5:
                    return "VisualStudio.DTE.8.0";
                case 3:
                    return "VisualStudio.DTE.7.1";
                case 2:
                    return "VisualStudio.DTE.7";
            }

            // Don't know this Visual Studio version.
            return "";
        }

        private static bool OpenVisualStudioByFileLineInternal(String filename, int fileline, int vsVersion)
        {
            try
            {
                EnvDTE80.DTE2 dte2;
                string vsString = GetVersionString(vsVersion);
                dte2 = (EnvDTE80.DTE2)System.Runtime.InteropServices.Marshal.GetActiveObject(vsString);
                dte2.MainWindow.Activate();
                EnvDTE.Window w = dte2.ItemOperations.OpenFile(filename, "{7651A703-06E5-11D1-8EBD-00A0C90F26EA}");
                ((EnvDTE.TextSelection)dte2.ActiveDocument.Selection).GotoLine(fileline, true);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public static bool OpenVisualStudioByFileLine(String filename, int fileline)
        {
            if (filename == null || fileline < 0)
                return false;

            int[] tryVersions = { 13, 12, 10, 8, 5, 3, 2 };

            foreach (int element in tryVersions)
            {
                if (OpenVisualStudioByFileLineInternal(filename, fileline, element))
                    return true;
            }
            return false;
        }
    }
}
