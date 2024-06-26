using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[GlobalClass]
public partial class SceneManager : Node
{
    public const string SingletonPath = "/root/Scene_Manager";

    [Export]
    public PackedScene DefaultLoadingScreen { get; set; }

    public SceneCollectionTag CurrentCollection { get; set; } = null;
    public Node CurrentBaseNode { get; set; } = null;


    /// <summary>
    /// Loads the provided game mode and scenes for the mode
    /// </summary>
    /// <param name="gameMode">Gamemode to load</param>
    public void LoadSceneCollection(SceneCollectionTag collection, bool freeUntrackedScenes = true) 
    {
        if (collection == null)
        {
            GD.PrintErr($"Cannot load a null collection.");
            return;
        }

        if (CurrentCollection == collection)
        {
            GD.PrintErr($"Cannot load the provided collection {collection.ResourceName} because it is already live!");
            return;
        }

        CallDeferred(MethodName.LoadSceneCollection_Deferred, collection, freeUntrackedScenes);
    }
    private void LoadSceneCollection_Deferred(SceneCollectionTag collection, bool freeUntrackedScenes = true)
    {
        var root = GetTree().Root.GetChildren().ToList();

        //Spin up the loading screen
        var loadingScreen = collection.LoadingScreenOverride ?? DefaultLoadingScreen;
        var loadingScreenNode = ResourceLoader.Load<PackedScene>(loadingScreen.ResourcePath).Instantiate<Node>();
        GetTree().Root.AddChild(loadingScreenNode);

        //End previous game mode and decide what we're doing
        CurrentCollection?.GameMode?.Cleanup(CurrentBaseNode);
        var nodeActions = GenerateSceneActions(collection);

        //Create new node to hold everything
        Node newNode = new Node();
        newNode.Name = string.IsNullOrWhiteSpace(collection.EditorDisplayName) ? collection.ResourceName : collection.EditorDisplayName;
        GetTree().Root.AddChild(newNode);

        //Tranfer nodes we're keeping
        foreach (var node in nodeActions.NodesToTransfer)
        {
            node.GetParent().RemoveChild(node);
            newNode.AddChild(node);
        }

        //Kill the old node holding children
        if (CurrentBaseNode is not null)
            CurrentBaseNode.QueueFree();

        //Kill untracked nodes
        foreach (var node in nodeActions.UntrackedNodes)
        {
            node.QueueFree();
        }

        //Load the new nodes
        CurrentBaseNode = newNode;
        foreach (var scene in nodeActions.ScenesToLoad)
        {
            var path = scene.ResourcePath;
            var loaded = GD.Load<PackedScene>(path);
            var instance = loaded.Instantiate();

            CurrentBaseNode.AddChild(instance);
        }

        //Start new mode and remove loading screen
        CurrentCollection = collection;
        collection.GameMode?.Setup(CurrentBaseNode);
        loadingScreenNode.QueueFree();
    }

    /// <summary>
    /// Determines what load actions need to be taken to transition from one scene collection to another
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    private SceneActionCollection GenerateSceneActions(SceneCollectionTag collection)
    {
        var existingActions = GenerateActionsForExistingScenes(collection);
        var newActions = GenerateActionsForNewCollectionScenes(collection);

        return new SceneActionCollection()
        {
            ScenesToLoad = newActions,
            NodesToTransfer = existingActions.toMove,
            UntrackedNodes = existingActions.unknown
        };
    }

    /// <summary>
    /// Given a collection, determines what nodes need to be unloaded and what nodes are 'unknown'
    /// </summary>
    /// <param name="collection">Collection being loaded</param>
    private (List<Node> toMove, List<Node> unknown) GenerateActionsForExistingScenes(SceneCollectionTag collection)
    {
        var pretty = GetTreeStringPretty();
        var rootNodes = GetTree().Root.GetChildren().ToList();
        var unknownNodes = rootNodes.Where(x => NodeUtilities.IsSceneNode(x) || NodeUtilities.IsUiNode(x)).ToList();
        var toMoveNodes = new List<Node>();

        if (CurrentBaseNode != null && !collection.ReloadAlreadyExistingNodes)
        {
            foreach (var node in CurrentBaseNode.GetChildren().Where(x => NodeUtilities.IsSceneNode(x) || NodeUtilities.IsUiNode(x)))
            {
                if (!string.IsNullOrEmpty(node.SceneFilePath) && collection.Scenes.Any(x => x.ResourcePath == node.SceneFilePath))
                    toMoveNodes.Add(node);
            }
        }

        return (toMoveNodes, unknownNodes);
    }

    /// <summary>
    /// Given a collection, determines whick of the nodes need to be loaded into the game
    /// </summary>
    /// <param name="collection">Collection being loaded</param>
    private List<PackedScene> GenerateActionsForNewCollectionScenes(SceneCollectionTag collection)
    {
        if (CurrentBaseNode is null || collection.ReloadAlreadyExistingNodes)
            return collection.Scenes.ToList();

        var currentNodes = CurrentBaseNode.GetChildren()
            .Where(x => NodeUtilities.IsSceneNode(x) || NodeUtilities.IsUiNode(x) && !string.IsNullOrEmpty(x.SceneFilePath));

        return collection.Scenes.Where(inboundScene => !currentNodes.Any(curr => curr.SceneFilePath == inboundScene.ResourcePath)).ToList();
    }

    private struct SceneActionCollection
    {
        public List<PackedScene> ScenesToLoad { get; set; }
        public List<Node> NodesToTransfer { get; set; }
        public List<Node> UntrackedNodes { get; set; }
    }
}
