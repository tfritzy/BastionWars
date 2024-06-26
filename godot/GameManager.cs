using Godot;
using KeepLordWarriors;
using System;
using System.Collections.Generic;

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
}
