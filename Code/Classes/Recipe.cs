using Godot;

[GlobalClass]
public partial class Recipe : Resource
{
    [Export] public int Id { get; set; }
    [Export] public string Name { get; set; }
    [Export] public Texture2D Icon { get; set; }
    [Export] public float CraftingTime { get; set; }
    [Export] public Godot.Collections.Array<RecipeEntry> Inputs = [];
    [Export] public Godot.Collections.Array<RecipeEntry> Outputs = [];
}

