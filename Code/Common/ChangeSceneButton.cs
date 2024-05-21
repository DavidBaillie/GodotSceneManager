using Godot;
using System;

[GlobalClass]
public partial class ChangeSceneButton : Button
{
    [Export(PropertyHint.File)]
    public string CollectionPath { get; set; }

    private SceneCollectionTag sceneCollection;

    public override void _Ready()
    {
        sceneCollection = GD.Load<SceneCollectionTag>(CollectionPath);
    }

    public override void _Pressed()
    {
        var sceneManager = GetNode<SceneManager>(SceneManager.SingletonPath);
        sceneManager.LoadSceneCollection(sceneCollection);
    }
}
