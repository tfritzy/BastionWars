using Godot;
using KeepLordWarriors;
using System;

public partial class GameManager : Node
{
	public Game Game;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		using var file = FileAccess.Open("res://map.txt", FileAccess.ModeFlags.Read);
		string map = file.GetAsText();

		Game = new Game(new GameSettings(
			mode: GenerationMode.AutoAccrue,
			map: map
		));
		GD.Print("Num bastions: " + Game.Map.Bastions.Count);
		foreach (var bastion in Game.Map.Bastions)
		{
			BuildBastion(bastion);
		}

		AddChild(new Terminal());
	}

	void BuildBastion(Bastion bastion)
	{
		BastionMono bastionMono = new(bastion);
		AddChild(bastionMono);
	}

	public override void _Process(double delta)
	{
		Game.Update(delta);
	}
}
