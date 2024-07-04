using Godot;
using KeepLordWarriors;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameMono : Node
{
	public Game Game;
	private Dictionary<ulong, SoldierMono> soldiers = new();
	private MapMono mapMono;

	public override void _Ready()
	{
		using var file = FileAccess.Open("res://map.txt", FileAccess.ModeFlags.Read);
		string map = file.GetAsText();

		Game = new Game(new GameSettings(
			mode: GenerationMode.Word,
			map: map
		));

		mapMono = new MapMono(Game.Map);
		AddChild(mapMono);
		AddChild(new InteractiveCamera(mapMono.Center));
		AddChild(new Terminal(Game));
		ConfigureScene();
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
}
