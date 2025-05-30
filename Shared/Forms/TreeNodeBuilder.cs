using ColorControl.Shared.Common;
using static ColorControl.Shared.Common.NestedItemsBuilder;

namespace ColorControl.Shared.Forms;

public class TreeNodeBuilder
{
    public static TreeNode CreateTree(object obj, string text)
    {
        var nestedItem = NestedItemsBuilder.CreateTree(obj, text);

        return BuildTree(nestedItem);
    }

    private static TreeNode BuildTree(NestedItem nestedItem)
    {
        var treeNode = new TreeNode
        {
            Text = nestedItem.Value,
            Name = nestedItem.Name
        };
        var nodes = nestedItem.NestedItems.Select(BuildTree);

        treeNode.Nodes.AddRange([.. nodes]);

        return treeNode;
    }
}
