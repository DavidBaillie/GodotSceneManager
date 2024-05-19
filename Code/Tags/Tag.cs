using Godot;
using System;

[GlobalClass]
public partial class Tag : Resource
{
    [Export(PropertyHint.ObjectId)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
}
