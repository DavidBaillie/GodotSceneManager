using Godot;

[GlobalClass]
public partial class SceneManager : Node
{
    public SceneCollectionTag CurrentCollection { get; set; } = null;
    public Node CurrentBaseNode { get; set; } = null;


    /// <summary>
    /// Loads the provided game mode and scenes for the mode
    /// </summary>
    /// <param name="gameMode">Gamemode to load</param>
    public void LoadGameMode(SceneCollectionTag collection) 
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

        CallDeferred(MethodName.LoadSceneCollection_Deferred, collection);
    }
    private void LoadSceneCollection_Deferred(SceneCollectionTag collection)
    {
        if (CurrentCollection?.GameMode != null )
        {
            CurrentCollection.GameMode.Cleanup(CurrentBaseNode);
        }

        
    }
}
