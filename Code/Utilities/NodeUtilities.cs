using Godot;

public static class NodeUtilities
{
    public static bool IsSceneNode(this Node node) 
    {
        return node is Node3D || node is Node2D;
    }

    public static bool IsUiNode(this Node node)
    {
        return node is Control;
    }
}