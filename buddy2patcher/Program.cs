using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace buddy2patcher
{
    class Program
    {
        private static readonly string FolderPath = "..\\..\\..\\files\\addons\\";
        private static readonly string OutputPath = "..\\..\\..\\files\\patches.xml";
        static void Main(string[] args)
        {
            BuddyAddonHandler buddyAddonHandler = new BuddyAddonHandler(FolderPath);
            XMLPatchHandler patchHandler = new XMLPatchHandler(buddyAddonHandler.ModifiedFiles, buddyAddonHandler.Addons);
            var patchesDocument = patchHandler.GenerateXMLDocument();
            patchesDocument.Save(OutputPath);
        }
    }
}
