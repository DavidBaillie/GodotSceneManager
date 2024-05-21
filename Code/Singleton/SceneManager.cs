using Godot;
using System.Collections.Generic;
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
    public void LoadGameMode(SceneCollectionTag collection, bool freeUntrackedScenes = true) 
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
        Task.Run(async () =>
        {
            //Spin up the loading screen
            var loadingScreen = collection.LoadingScreenOverride ?? DefaultLoadingScreen;
            var loadingScreenNode = ResourceLoader.Load<PackedScene>(loadingScreen.ResourcePath).Instantiate<Node>();
            GetTree().Root.AddChild(loadingScreenNode);

            await Task.Delay(1);

            //End previous game mode and decide what we're doing
            CurrentCollection?.GameMode?.Cleanup(CurrentBaseNode);
            var nodeActions = GenerateSceneActions(collection);

            await Task.Delay(1);

            //Create new node to hold everything
            Node newNode = new Node();
            newNode.Name = string.IsNullOrWhiteSpace(collection.EditorDisplayName) ? collection.ResourceName : collection.EditorDisplayName;

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

            await Task.Delay(1);

            //Load the new nodes
            CurrentBaseNode = newNode;
            foreach(var node in nodeActions.ScenesToLoad)
            {
                CurrentBaseNode.AddChild(GD.Load<PackedScene>(node.ResourcePath).Instantiate());
            }

            await Task.Delay(1);

            //Start new mode and remove loading screen
            CurrentCollection = collection;
            collection.GameMode?.Setup(CurrentBaseNode);
            loadingScreenNode.QueueFree();
        });
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
    private (List<Node> toUnload, List<Node> toMove, List<Node> unknown) GenerateActionsForExistingScenes(SceneCollectionTag collection)
    {
        return default;
    }

    /// <summary>
    /// Given a collection, determines whick of the nodes need to be loaded into the game
    /// </summary>
    /// <param name="collection">Collection being loaded</param>
    private List<PackedScene> GenerateActionsForNewCollectionScenes(SceneCollectionTag collection)
    {
        return default;
    }

    private struct SceneActionCollection
    {
        public List<PackedScene> ScenesToLoad { get; set; }
        public List<Node> NodesToTransfer { get; set; }
        public List<Node> UntrackedNodes { get; set; }
    }
}
