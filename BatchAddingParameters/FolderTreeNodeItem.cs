using System.Collections.Generic;
using System.Collections.ObjectModel;
using ZetaLongPaths;

namespace BatchAddingParameters
{

    public class FolderTreeNodeItem
    {
        public string Node { get; set; }
        public List<FolderTreeNodeItem> SubNode { get; set; }
        public FolderTreeNodeItem()
        {
            SubNode = new List<FolderTreeNodeItem>();
        }

        public List<FolderTreeNodeItem> AllFolders;

        public Collection<FolderTreeNodeItem> Folders(string path)
        {
            int i = 0;
            var folderTreeItems = new Collection<FolderTreeNodeItem>();

            var node1 = new FolderTreeNodeItem();
            var folderPath = new ZlpDirectoryInfo(path);
            node1.Node = folderPath.Name;
            folderTreeItems.Add(node1);

            //foreach (var filePath in folderPath.GetFiles())
            //{ 

            //}
            ZlpDirectoryInfo[] subfolderPaths;
            do
            {

                subfolderPaths = folderPath.GetDirectories();
                foreach (var subfolderPath in subfolderPaths)
                {
                    var node = new FolderTreeNodeItem();
                    node.Node = subfolderPath.Name;
                    folderTreeItems[i].SubNode.Add(node);
                }
                i += 1;
            } while (subfolderPaths != null);

            return folderTreeItems;
        }
        public Collection<FolderTreeNodeItem> FolderTreeNodeItems()
        {
            var folderTreeNodeItems = new Collection<FolderTreeNodeItem> { new FolderTreeNodeItem { Node = "Folder 01" }, new FolderTreeNodeItem { Node = "Folder 02" } };
            folderTreeNodeItems[0].SubNode.Add(new FolderTreeNodeItem { Node = "Folder 03" });
            folderTreeNodeItems[0].SubNode.Add(new FolderTreeNodeItem { Node = "Folder 04" });
            folderTreeNodeItems[0].SubNode[0].SubNode.Add(new FolderTreeNodeItem { Node = "Folder 05" });

            return folderTreeNodeItems;
        }

    }

}
