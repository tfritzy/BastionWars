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
		foreach (var keep in Game.Map.Keeps.Values)
		{
			GD.Print(keep.Name);
		}

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

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode >= Key.A && eventKey.Keycode <= Key.Z)
			{
				Game.HandleKeystroke((char)('a' + eventKey.Keycode - Key.A), 1);
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