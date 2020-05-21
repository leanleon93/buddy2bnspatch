using System;
using System.Collections.Generic;
using System.Text;

namespace buddy2patcher
{
    public class Addon
    {
        public string Filename { get; set; }
        public List<Tuple<string, string>> SearchReplacePairs { get; set; }
        public List<PatchNode> patchNodes { get; set; }
    }
}
