using Godot;

[GlobalClass]
public partial class GameModeTag : Tag
{
    [Export]
    public PackedScene RootGameModeScene { get; set; }

    public virtual void Setup() { }
    public virtual void Cleanup() { }
}
