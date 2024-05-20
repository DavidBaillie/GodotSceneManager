using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class SceneManager : Node
{
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
        // TODO - add loading screen

        if (CurrentCollection?.GameMode != null )
        {
            CurrentCollection.GameMode.Cleanup(CurrentBaseNode);
        }

        var nodeActions = GenerateSceneActions(collection);



        // TODO - unload actions

        // TODO - load actions

        // TODO - start new gamemode

        // TODO - remove loading screen
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
            NodesToUnload = existingActions.toUnload,
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
        public List<Node> NodesToUnload { get; set; }
        public List<Node> UntrackedNodes { get; set; }
    }
}
