using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SceneCollectionTag : Tag
{
    [Export]
    public string EditorDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// When a required node in this collection already exists in the previous collection, should the transition reload that node or keep it?
    /// </summary>
    [Export]
    public bool ReloadAlreadyExistingNodes { get; set; } = false;

    [Export]
    public GameModeTag GameMode { get; set; } = null;

    [Export]
    public PackedScene LoadingScreenOverride { get; set; } = null;

    [Export]
    public Array<PackedScene> Scenes { get; set; } = new Array<PackedScene>();
}
