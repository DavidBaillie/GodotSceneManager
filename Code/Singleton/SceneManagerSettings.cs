using Godot;

[GlobalClass]
public partial class SceneManagerSettings : Node
{
    public const string SingletonPath = "/root/Scene_Manager_Settings";

    [Export]
    public PackedScene DefaultLoadingScreen { get; set; } = null;
}
