using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace buddy2patcher
{
    /// <summary>
    /// Handles the generation of the patches.xml
    /// </summary>
    public class XMLPatchHandler
    {
        private XmlDocument documentContext = new XmlDocument();
        private List<string> ModifiedFiles { get; set; }
        public List<Addon> Addons { get; set; }

        public XMLPatchHandler(List<string> modifiedFiles, List<Addon> addons)
        {
            this.ModifiedFiles = modifiedFiles;
            this.Addons = addons;
            SetPatchNodes();
        }

        public XmlDocument GenerateXMLDocument()
        {
            XmlDeclaration xmlDeclaration = documentContext.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = documentContext.DocumentElement;
            documentContext.InsertBefore(xmlDeclaration, root);
            XmlElement rootNode = documentContext.CreateElement(string.Empty, "patches", string.Empty);
            documentContext.AppendChild(rootNode);
            XmlDocumentFragment xfrag = documentContext.CreateDocumentFragment();
            xfrag.InnerXml = @"<?include .\patches\*.xml?>";
            rootNode.AppendChild(xfrag);
            foreach (var file in this.ModifiedFiles)
            {
                var patchNode = GenerateForFile(file);
                rootNode.AppendChild(patchNode);
            }
            return documentContext;
        }

        private void SetPatchNodes()
        {
            foreach(var addon in this.Addons)
            {
                var patchNodes = new List<PatchNode>();
                foreach(var change in addon.SearchReplacePairs)
                {
                    var lineSplit = change.Item2.Split('<')[1].Trim().Split(' ');
                    var tag = lineSplit[0].Trim();
                    var name = lineSplit.Where(x => x.Contains("name")).FirstOrDefault().Split('=')[1];
                    var value = lineSplit.Where(x => x.Contains("value")).FirstOrDefault().Split('=')[1];
                    patchNodes.Add(new PatchNode { Tag = tag, Name = name, Value = value });
                }
                addon.patchNodes = patchNodes;
            }
        }

        private XmlElement GenerateForFile(string filename)
        {
            XmlDocument doc = documentContext;
            var addonsForFile = this.Addons.Where(x => x.Filename == filename).ToList();
            XmlElement patch = doc.CreateElement(string.Empty, "patch", string.Empty);
            patch.SetAttribute("file", filename);
            foreach(var addon in addonsForFile)
            {
                foreach(var element in GetLinesForAddon(addon))
                {
                    patch.AppendChild(element);
                }
            }
            var asString = patch.OuterXml;
            return patch;
         }

        private List<XmlElement> GetLinesForAddon(Addon addon)
        {
            var elements = new List<XmlElement>();
            foreach(var node in addon.patchNodes)
            {
                XmlDocument doc = documentContext;
                XmlElement absoluteNode = doc.CreateElement(string.Empty, "select-node", string.Empty);
                var tagQuery = String.Format("//{0}[@name='{1}']/@value", node.Tag, node.Name.Replace("\"", ""));
                var value = node.Value.Replace("\"", "");
                absoluteNode.SetAttribute("query", tagQuery);
                XmlElement valueNode = doc.CreateElement(string.Empty, "set-value", string.Empty);
                valueNode.SetAttribute("value", value);
                absoluteNode.AppendChild(valueNode);
                elements.Add(absoluteNode);
            }
            return elements;
        }

    }
}
