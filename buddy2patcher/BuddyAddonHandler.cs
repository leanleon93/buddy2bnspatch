using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace buddy2patcher
{
    /// <summary>
    /// Handles the .patch file to objects conversion
    /// </summary>
    public class BuddyAddonHandler
    {
        private List<string> FilePaths { get; set; }
        public List<string> ModifiedFiles { get; set; }
        public List<Addon> Addons { get; set; }

        private readonly bool skipSkills = true;

        public BuddyAddonHandler(string folderPath)
        {
            this.FilePaths = Directory.EnumerateFiles(folderPath).Where(x => Path.GetExtension(x) == ".patch").ToList();
            this.ModifiedFiles = new List<string>();
            this.Addons = new List<Addon>();
            SetAllModifiedFiles();
            FillAddonList();
        }

        private void SetAllModifiedFiles()
        {
            foreach(var file in this.FilePaths)
            {
                this.ModifiedFiles.Add(GetModifiedFilename(file));
            }
            this.ModifiedFiles = this.ModifiedFiles.Distinct().ToList();
            if(skipSkills)
            {
                this.ModifiedFiles = this.ModifiedFiles.Where(x => !x.Contains("contextscript")).ToList();
            }
        }

        private void FillAddonList()
        {
            foreach(var file in this.FilePaths)
            {
                var parsedAddon = ParseAddon(file);
                if(parsedAddon != null)
                {
                    this.Addons.Add(parsedAddon);
                }
            }
        }

        private Addon ParseAddon(string fullpath)
        {
            if (skipSkills)
            {
                if (GetModifiedFilename(fullpath).Contains("contextscript")) return null;
            }
            var addon = new Addon();
            var searchReplacePairs = new List<Tuple<string, string>>();
            var addonLines = File.ReadAllLines(fullpath).ToList();
            addonLines.RemoveAt(0);
            addonLines.RemoveAt(addonLines.Count - 1);
            for(var i = 0; i<addonLines.Count; i++)
            {
                if(addonLines[i].Contains("Search"))
                {
                    var search = addonLines[i].Split(" = ")[1].Trim();
                    var replace = addonLines[i+1].Split(" = ")[1].Trim();
                    searchReplacePairs.Add(new Tuple<string, string>(search, replace));
                    i++;
                }
            }
            addon.Filename = GetModifiedFilename(fullpath);
            addon.SearchReplacePairs = searchReplacePairs;
            return addon;
        }

        private string GetModifiedFilename(string fullpath)
        {
            var addonLines = File.ReadAllLines(fullpath).ToList();
            return addonLines.First().Trim().Split('=')[1].Trim().Split("\\\\")[1];
        }
    }
}
