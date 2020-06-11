using buddy2patcher;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.XmlDiffPatch;
using System.Xml;

namespace Tests
{
    [TestClass]
    public class ProgramTests
    {
        //Expected is a patch file that 100% works in Bns
        private readonly string expectedFilePath = "..\\..\\..\\testfiles\\expected.xml";
        private readonly string testPatchesPath = "..\\..\\..\\testfiles\\addons\\";

        /// <summary>
        /// Test the conversion from .patch to patches.xml
        /// </summary>
        [TestMethod]
        public void FileConversionTest()
        {
            XmlDocument expectedDocument = new XmlDocument();
            expectedDocument.Load(expectedFilePath);
            BuddyAddonHandler buddyAddonHandler = new BuddyAddonHandler(testPatchesPath);
            XMLPatchHandler patchHandler = new XMLPatchHandler(buddyAddonHandler.ModifiedFiles, buddyAddonHandler.Addons);
            var documentContext = patchHandler.GenerateXMLDocument();
            XmlDiff diff = new XmlDiff
            {
                Options = XmlDiffOptions.IgnoreWhitespace | XmlDiffOptions.IgnoreComments | XmlDiffOptions.IgnoreXmlDecl | XmlDiffOptions.IgnoreChildOrder
            };
            Assert.IsTrue(diff.Compare(expectedDocument, documentContext));
        }
    }
}
