using Godot;
using KeepLordWarriors;
using System.Collections.Generic;

public partial class GameMono : Node
{
	public Game Game;
	private Dictionary<ulong, SoldierMono> soldiers = new();
	private MapMono mapMono;
	private ulong? selectedKeepId;
	private Label instructionLabel;

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

		ConfigureCamera();
		ConfigureMap();
		ConfigureFog();
		ConfigureScene();
		ConfigureKeeps();
		SpawnInstructionLabel();
	}

	void SyncSoldiers()
	{
		foreach (var soldier in Game.Map.Soldiers)
		{
			if (!soldiers.ContainsKey(soldier.Id))
			{
				SoldierMono soldierMono = new SoldierMono(Game, soldier.Id);
				soldiers[soldier.Id] = soldierMono;
				AddChild(soldierMono);
			}
		}
	}

	void SpawnInstructionLabel()
	{
		instructionLabel = new Label
		{
			Text = "",
			Position = new Vector2(-200, -200),
			LabelSettings = new LabelSettings()
			{
				FontSize = 60,
				FontColor = new Color(0, 0, 0, 1)
			}
		};
		AddChild(instructionLabel);
	}

	public override void _Process(double delta)
	{
		Game.Update(delta);
		SyncSoldiers();
	}

	void ConfigureCamera()
	{
		var cam = new InteractiveCamera();
		cam.Position = new Vector3(Game.Map.Width / 2, 17, Game.Map.Height);
		AddChild(cam);
		cam.LookAt(new Vector3(Game.Map.Width / 2f, 0, (Game.Map.Height + 5) / 2f));
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
				char key = (char)('a' + eventKey.Keycode - Key.A);
				Game.HandleKeystroke(key, 1);
				UpdateWords();
				UpdateKeepNames(key);
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

	private void UpdateKeepNames(char key)
	{
		foreach (KeepMono keep in mapMono.KeepMonos.Values)
		{
			if (selectedKeepId == null && keep.Keep.Alliance != 1)
			{
				continue;
			}

			keep.NameLabel.HandleKeystroke(key);
		}
	}

	private void ConfigureMap()
	{
		mapMono = new MapMono(Game.Map);
		AddChild(mapMono);
	}

	private void ConfigureFog()
	{
		for (float y = 1; y <= 2; y += .5f)
		{
			MeshInstance3D fog = new MeshInstance3D();
			fog.Mesh = new QuadMesh();
			fog.Mesh.SurfaceSetMaterial(
				0,
				GD.Load<Material>("res://Rendering/Materials/fog.tres")
			);
			fog.Scale = new Vector3(Game.Map.Width + 6, Game.Map.Height + 6, 1);
			fog.Position = new Vector3(Game.Map.Width / 2, y, Game.Map.Height / 2);
			fog.RotateX(-Mathf.Pi / 2);
			AddChild(fog);
		}

		MeshInstance3D wordBound = new MeshInstance3D();
		wordBound.Mesh = new QuadMesh();
		wordBound.Scale = new Vector3(100, 100, 1);
		wordBound.RotateX(-Mathf.Pi / 2);
		// wordBound.Position = new Vector3(Game.Map.Width / 2, 1, Game.Map.Height + 100);
		wordBound.Position = new Vector3(0, 0, -50 - 6);
		AddChild(wordBound);
	}

	private void ConfigureKeeps()
	{
		foreach (var keep in mapMono.KeepMonos.Values)
		{
			keep.OnSelect = SelectKeep;
		}
	}

	private bool IsValidSelection(Keep keep)
	{
		if (selectedKeepId == null)
		{
			// Don't select enemy keeps
			if (keep.Alliance != 1)
			{
				return false;
			}
		}

		return true;
	}

	private void SelectKeep(Keep keep)
	{
		if (!IsValidSelection(keep))
		{
			return;
		}

		if (selectedKeepId != null)
		{
			Game.AttackBastion(selectedKeepId.Value, keep.Id);
			selectedKeepId = null;
			instructionLabel.Text = "";
		}
		else
		{
			selectedKeepId = keep.Id;
			instructionLabel.Text = $"Selected {selectedKeepId}. Select a target.";
		}
	}
}
