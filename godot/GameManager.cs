using Godot;
using KeepLordWarriors;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameManager : Node
{
	public Game Game;
	private Dictionary<ulong, SoldierMono> soldiers = new();

	public override void _Ready()
	{
		using var file = FileAccess.Open("res://map.txt", FileAccess.ModeFlags.Read);
		string map = file.GetAsText();

		Game = new Game(new GameSettings(
			mode: GenerationMode.AutoAccrue,
			map: map
		));
		foreach (var bastion in Game.Map.Bastions)
		{
			BuildBastion(bastion);
		}

		AddChild(new Terminal(Game));

		ConfigureScene();
		// SpawnTiles();
	}

	void BuildBastion(Bastion bastion)
	{
		BastionMono bastionMono = new(bastion);
		AddChild(bastionMono);
	}

	void SyncSoldiers()
	{
		foreach (var soldier in Game.Map.Soldiers)
		{
			if (!soldiers.ContainsKey(soldier.Id))
			{
				SoldierMono soldierMono = new(Game, soldier.Id);
				soldiers[soldier.Id] = soldierMono;
				AddChild(soldierMono);
			}
		}
	}

	public override void _Process(double delta)
	{
		Game.Update(delta);
		SyncSoldiers();
	}

	void ConfigureScene()
	{
		RenderingServer.SetDefaultClearColor(new Color(1, 1, 1, 1));
	}

	void SpawnTiles()
	{
		var texture = (Texture2D)GD.Load<Texture>("res://Sprites/tile.png");
		for (int x = 0; x < Game.Map.Width; x++)
		{
			for (int y = 0; y < Game.Map.Height; y++)
			{
				Sprite2D tileSprite = new();
				tileSprite.Texture = texture;
				tileSprite.Position = new Vector2(x, y) * Constants.WorldSpaceToScreenSpace;
				AddChild(tileSprite);
			}
		}
	}
}
