using Godot;
using Godot.Collections;

public partial class AbstractCharacterResource : Resource
{
    public enum OrientationEnum
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    public enum CharacterTypeEnum
    {
        Player,
        Enemy,
        NPC
    }

    [Export] public CharacterTypeEnum CharacterType { get; set; } = CharacterTypeEnum.NPC;

    // [Export] public ExplosionResource HitExplosionResource { get; set; }

    [Export] public OrientationEnum InitialOrientation { get; set; } = OrientationEnum.Down;

    [Export] public string ID { get; set; } = "";
    [Export] public int HealthMax { get; set; }
    [Export] public float MovementSpeed { get; set; }
    [Export] public int ScanRadius { get; set; }

    [Export]
    public Dictionary<CharacterStateManager.LifeSubStateEnum, float> StateLengths { get; set; } = new Dictionary<CharacterStateManager.LifeSubStateEnum, float> {
        { CharacterStateManager.LifeSubStateEnum.Spawning, 1.0f },
        { CharacterStateManager.LifeSubStateEnum.Dying, 1.0f },
        { CharacterStateManager.LifeSubStateEnum.Hit, 1.0f },
    };

    [ExportGroup("Sprite")]
    [Export] public SpriteFrames SpriteFrames { get; set; } = new();
    [Export] public Array<string> AnimationPrefixes { get; set; } = new Array<string> { "idle", "move", "hit", "dying", "spawn" };

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