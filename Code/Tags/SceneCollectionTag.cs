using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SceneCollectionTag : Tag
{
    [Export]
    public string EditorDisplayName { get; set; }

    [Export]
    public GameModeTag GameMode { get; set; }

    [Export]
    public Array<PackedScene> Scenes { get; set; }
}
