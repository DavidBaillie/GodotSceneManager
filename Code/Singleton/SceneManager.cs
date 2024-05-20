using Godot;

[GlobalClass]
public partial class SceneManager : Node
{
    public GameModeTag CurrentGameMode { get; set; } = null;
    public Node CurrentScene { get; set; } = null;

    /// <summary>
    /// When when Node is ready
    /// </summary>
    public override void _Ready()
    {
        var root = GetTree().Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    /// <summary>
    /// Loads the provided game mode and scenes for the mode
    /// </summary>
    /// <param name="gameMode">Gamemode to load</param>
    public void LoadGameMode(GameModeTag gameMode) 
    {
        if (gameMode == null)
        {
            GD.PrintErr($"Cannot load a null Game Mode!");
            return;
        }

        if (CurrentGameMode == gameMode)
        {
            GD.PrintErr($"Cannot load the Game Mode {gameMode.ResourceName} because it is already live!");
            return;
        }

        CallDeferred(MethodName.LoadGameMode_Deferred, gameMode);
    }
    private void LoadGameMode_Deferred(GameModeTag gameMode)
    {
        if (CurrentGameMode != null )
        {
            CurrentGameMode.Cleanup();
        }

        CurrentScene.Free();

        var nextScene = GD.Load<PackedScene>(gameMode.RootGameModeScene.ResourcePath);
        CurrentScene = nextScene.Instantiate();
        
        GetTree().Root.AddChild(CurrentScene);
        GetTree().CurrentScene = CurrentScene;
    }
}
