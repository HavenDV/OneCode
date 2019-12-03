using System.Collections.ObjectModel;
using OneCode.Core;

namespace OneCode.VsExtension.Windows
{
    public class Node
    {
        public string Name { get; set; }
        public Method Method { get; set; }
        public Class Class { get; set; }
        public CodeFile CodeFile { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}