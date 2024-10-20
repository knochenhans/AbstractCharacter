using Godot;
using Godot.Collections;

public partial class AbstractCharacterResource : Resource
{
    public enum OrientationEnum
    {
        Idle,
        Up,
        Down,
        Left,
        Right
    }

    public enum TypeEnum
    {
        Player,
        Enemy,
        NPC
    }

    [Export] public TypeEnum CharacterType { get; set; } = TypeEnum.NPC;

    [Export] public ExplosionResource HitExplosionResource { get; set; }

    [Export] public OrientationEnum InitialOrientation { get; set; } = OrientationEnum.Down;

    [Export] public string ID { get; set; } = "";
    [Export] public int HealthMax { get; set; }
    [Export] public float MovementSpeed { get; set; }
    [Export] public int ScanRadius { get; set; }

    [ExportGroup("Sprite")]
    [Export] public SpriteFrames SpriteFrames { get; set; } = new();
    [Export] public string IdlePrefix { get; set; } = "idle";
    [Export] public string MovementPrefix { get; set; } = "move";

    [ExportGroup("Sounds")]
    [Export] public Array<AudioStream> MovementSounds { get; set; } = new();
    [Export] public Array<AudioStream> HitSounds { get; set; } = new();
    [Export] public Array<AudioStream> IdleSounds { get; set; } = new();
    [Export] public Array<AudioStream> DeathSounds { get; set; } = new();
    [Export] public Array<AudioStream> NoticeSounds { get; set; } = new();
    [Export] public Array<AudioStream> SpawnSounds { get; set; } = new();

    [Export] public float Pitch { get; set; } = 1.0f;
    [Export] public float RandomPitch { get; set; } = 0f;
}