using Godot;

[GlobalClass]
public partial class GameModeTag : Tag
{
    public virtual void Setup(Node rootNode) { }
    public virtual void Cleanup(Node rootNode) { }
}
