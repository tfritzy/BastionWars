using Godot;
using KeepLordWarriors;
using System.Collections.Generic;

public partial class GameMono : Node
{
	public Game Game;
	private Dictionary<ulong, SoldierMono> soldiers = new();
	private MapMono mapMono;

	public override void _Ready()
	{
		using var file = FileAccess.Open("res://Maps/map.txt", FileAccess.ModeFlags.Read);
		string map = file.GetAsText();

		Game = new Game(new GameSettings(
			mode: GenerationMode.Word,
			map: map
		));

		mapMono = new MapMono(Game.Map);
		AddChild(mapMono);
		AddChild(new InteractiveCamera(mapMono.Center));
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

	private Dictionary<Key, char> keys = new() {
		{ Key.A, 'a'},
		{ Key.B, 'b'},
		{ Key.C, 'c'},
		{ Key.D, 'd'},
		{ Key.E, 'e'},
		{ Key.F, 'f'},
		{ Key.G, 'g'},
		{ Key.H, 'h'},
		{ Key.I, 'i'},
		{ Key.J, 'j'},
		{ Key.K, 'k'},
		{ Key.L, 'l'},
		{ Key.M, 'm'},
		{ Key.N, 'n'},
		{ Key.O, 'o'},
		{ Key.P, 'p'},
		{ Key.Q, 'q'},
		{ Key.R, 'r'},
		{ Key.S, 's'},
		{ Key.T, 't'},
		{ Key.U, 'u'},
		{ Key.V, 'v'},
		{ Key.W, 'w'},
		{ Key.X, 'x'},
		{ Key.Y, 'y'},
		{ Key.Z, 'z'},
	};
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && keys.TryGetValue(eventKey.Keycode, out char key))
			{
				Game.HandleKeystroke(key);
				UpdateWords();
			}
		}
	}

	private void UpdateWords()
	{
		foreach (var word in Game.Map.Words)
		{
			if (word.Value == null)
			{
				continue;
			}

			if (mapMono.Words.TryGetValue(word.Key, out Typeable value))
			{
				value.UpdateProgress(word.Value.TypedIndex);
			}
		}
	}
}