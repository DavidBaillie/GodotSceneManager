using Godot;
using System;

[GlobalClass]
public partial class SceneReferenceTag : Tag
{
    [Export(PropertyHint.File, "*.tscn")]
    public string ScenePath { get; set; }
    
    [Export(PropertyHint.EnumSuggestion)]
    public LoadOption AlwaysLoad { get; set; }
}

public enum LoadOption { Standard, AlwaysLoad, Persistent }