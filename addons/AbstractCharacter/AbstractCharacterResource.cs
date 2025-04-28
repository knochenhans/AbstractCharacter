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
        Right,
        Forward,
        Backward
    }

    public enum CharacterTypeEnum
    {
        Generic,
        Player,
        Enemy,
        NPC
    }

    [Export] public CharacterTypeEnum CharacterType { get; set; } = CharacterTypeEnum.Generic;

    // [Export] public ExplosionResource HitExplosionResource { get; set; }

    [Export] public OrientationEnum InitialOrientation { get; set; } = OrientationEnum.Down;

    [Export] public string ID { get; set; } = "";
    [Export] public int HealthMax { get; set; }
    [Export] public float MovementSpeed { get; set; }
    [Export] public float Friction { get; set; } = 0.5f;

    [ExportGroup("Areas")]
    [Export] public int ScanRadius { get; set; } = 10;
    [Export] public int PickupRadius { get; set; } = 10;

    [ExportGroup("State")]
    [Export] public Array<AbstractCharacterStateResource> States { get; set; } = [];
    [Export] public string InitialLifeState { get; set; } = "spawning";
    [Export] public string StateAfterHit { get; set; } = "hit";
    [Export] public string IdleState { get; set; } = "idle";
    [Export] public string DeadState { get; set; } = "dead";

    [ExportGroup("Sprite")]
    [Export] public SpriteFrames SpriteFrames { get; set; } = new();
    [Export] public Array<string> AnimationPrefixes { get; set; } = ["idle", "move", "hit", "dying", "spawning"];

    [ExportGroup("Sounds")]
    [Export] public Array<AudioStream> MovementSounds { get; set; } = [];
    [Export] public Array<AudioStream> NoticeSounds { get; set; } = [];

    [Export] public float Pitch { get; set; } = 1.0f;
    [Export] public float RandomPitch { get; set; } = 0f;

}