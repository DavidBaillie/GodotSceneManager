using Godot;
using System;
using System.Linq;

public partial class MAIN : Node3D
{
    [Export]
    public PackedScene TScene { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		var scene = GD.Load<PackedScene>(TScene.ResourcePath).Instantiate();
		GetTree().Root.CallDeferred(MethodName.AddChild, scene);

		CallDeferred(MethodName.CheckAfter);
	}

	public void CheckAfter()
	{
		foreach (var node in GetTree().Root.GetChildren())
		{
			GD.Print($"IsSubclass: {node.GetType().IsSubclassOf(typeof(Node))}");
			//GD.Print($"{node.Name} --> IsControl: {node is Control}, IsNode3D: {node is Node3D}, IsNode2D: {node is Node2D}");
		}


        //GD.Print(string.Join(",", GetTree().Root.GetChildren().ToList().Select(x => x.Name)));
    }
}
