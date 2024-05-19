using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GameModeTag : Tag
{
    [Export]
    public Array<SceneReferenceTag> Scenes { get; set; }

    public virtual void Setup() { }
    public virtual void Cleanup() { }
}
