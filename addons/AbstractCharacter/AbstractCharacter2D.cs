using System;
using Godot;

public partial class AbstractCharacter2D : CharacterBody2D
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    [Signal] public delegate void RequestCurrentTileDataEventHandler(Character character);
    [Signal] public delegate void TargetPositionSetEventHandler(Vector2 position);
    [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void DiedEventHandler(Vector2 position);
    // [Signal] public delegate void HitEventHandler(Vector2 position, ExplosionResource explosionResource);
    [Signal] public delegate void HealthChangedEventHandler(int health, int healthMax);

    public enum ActivityStateEnum
    {
        Active,
        Inactive
    }

    public enum MovementStateEnum
    {
        Idle,
        Moving
    }

    public enum LifeStateEnum
    {
        Living,
        Hit,
        Dying
    }

    public ActivityStateEnum ActivityState
    {
        get => _activityState;
        set
        {
            OnActivityStateChange(value);
            _activityState = value;
        }
    }

    public MovementStateEnum MovementState
    {
        get => _movementState;
        set
        {
            if (_movementState != value)
            {
                OnMovementStateChange(value);
                _movementState = value;
            }
        }
    }

    public LifeStateEnum LifeState
    {
        get => _lifeState;
        set
        {
            if (_lifeState != value)
            {
                OnLifeStateChange(value);
                _lifeState = value;
            }
        }
    }

    private ActivityStateEnum _activityState = ActivityStateEnum.Active;
    private MovementStateEnum _movementState = MovementStateEnum.Idle;
    private LifeStateEnum _lifeState = LifeStateEnum.Living;

    protected virtual void OnActivityStateChange(ActivityStateEnum newState)
    {
        HitSoundPlayer.ActivityState = newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive;
        IdleSoundPlayer.ActivityState = newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive;
    }

    protected virtual void OnMovementStateChange(MovementStateEnum newState)
    {
        switch (newState)
        {
            case MovementStateEnum.Idle:
                // _currentAnimationPrefix = CharacterResource.IdlePrefix;
                MovementSoundPlayer.StopLoop();
                break;
            case MovementStateEnum.Moving:
                // _currentAnimationPrefix = CharacterResource.MovementPrefix;
                EmitSignal(SignalName.RequestCurrentTileData, this);
                MovementSoundPlayer.StartLoop();
                break;
        }
    }

    protected virtual void OnLifeStateChange(LifeStateEnum newState)
    {
        switch (newState)
        {
            case LifeStateEnum.Living:
                break;
            case LifeStateEnum.Hit:
                break;
            case LifeStateEnum.Dying:
                break;
        }
    }

    protected AbstractCharacterController2D CharacterController { get; set; }

    protected NavigationAgent2D NavigationAgent2D => GetNode<NavigationAgent2D>("NavigationAgent2D");

    protected TimedAudioStreamPlayer2D HitSoundPlayer => GetNode<TimedAudioStreamPlayer2D>("HitSoundPlayer");
    protected TimedAudioStreamPlayer2D IdleSoundPlayer => GetNode<TimedAudioStreamPlayer2D>("IdleSoundPlayer");
    protected AudioStreamPlayer2D DeathSoundPlayer => GetNode<AudioStreamPlayer2D>("DeathSoundPlayer");
    protected AudioStreamPlayer2D NoticedSoundPlayer => GetNode<AudioStreamPlayer2D>("NoticedSoundPlayer");
    protected TimedAudioStreamPlayer2D MovementSoundPlayer => GetNode<TimedAudioStreamPlayer2D>("MovementSoundPlayer");
    protected AudioStreamPlayer2D SpawnedSoundPlayer => GetNode<AudioStreamPlayer2D>("SpawnedSoundPlayer");

    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    public int Health { get; set; }
    protected int MovementSpeed { get; set; }

    public Vector2I TileSize { get; set; } = Vector2I.Zero;
    public Vector2I LastCheckedTile { get; set; } = Vector2I.Zero;

    public Vector2 MovementTarget
    {
        get => NavigationAgent2D.TargetPosition;
        set
        {
            NavigationAgent2D.TargetPosition = value;
            MovementState = MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, value);
        }
    }

    protected AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    protected Area2D ScanArea => GetNode<Area2D>("ScanArea");
    protected Area2D PickupArea => GetNode<Area2D>("PickupArea");

    public virtual AbstractCharacter2D WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
    {
        CharacterResource = characterResource;
        CharacterControllerScene = characterControllerScene;
        return this;
    }

    public override void _Ready()
    {
        Logger.Log($"Initializing character node {this.Name}", Logger.LogTypeEnum.Character);

        Health = CharacterResource.HealthMax;

        Orientation = CharacterResource.InitialOrientation;

        AnimatedSprite2D.SpriteFrames = CharacterResource.SpriteFrames;
        AnimatedSprite2D.SpriteFrames.ResourceLocalToScene = true;

        SetupSounds();

        CharacterController = CharacterControllerScene.Instantiate() as AbstractCharacterController2D;
        CharacterController.ControlledCharacter = this;
        CharacterController.CharacterNoticed += OnCharacterControllerCharacterNoticed;
        AddChild(CharacterController);

        Area2D scanArea = GetNode<Area2D>("ScanArea");
        scanArea.BodyEntered += (body) => CharacterController.OnScanArea2DBodyEntered(body);
        scanArea.BodyExited += (body) => CharacterController.OnScanArea2DBodyExited(body);

        var scanAreaShape = scanArea.GetNode<CollisionShape2D>("CollisionShape2D");
        (scanAreaShape.Shape as CircleShape2D).Radius = CharacterResource.ScanRadius;

        PickupArea.AreaEntered += OnPickupAreaAreaEntered;
    }

    public void Spawn() => SpawnedSoundPlayer.Play();

    private void SetupSounds()
    {
        MovementSoundPlayer.AddSoundSetsFromRaw(CharacterResource.MovementSounds);
        IdleSoundPlayer.AddSoundSet("idle", CharacterResource.IdleSounds);
        IdleSoundPlayer.CurrentSoundSet = "idle";
        HitSoundPlayer.SetStreams(CharacterResource.HitSounds);

        var DeathAudioStreamRandomizer = new AudioStreamRandomizer();
        var NoticedAudioStreamRandomizer = new AudioStreamRandomizer();
        var SpawnedAudioStreamRandomizer = new AudioStreamRandomizer();

        foreach (AudioStream audioStream in CharacterResource.NoticeSounds)
            NoticedAudioStreamRandomizer.AddStream(-1, audioStream);

        NoticedSoundPlayer.Stream = NoticedAudioStreamRandomizer;

        foreach (AudioStream audioStream in CharacterResource.SpawnSounds)
            SpawnedAudioStreamRandomizer.AddStream(-1, audioStream);

        SpawnedSoundPlayer.Stream = SpawnedAudioStreamRandomizer;

        foreach (AudioStream audioStream in CharacterResource.DeathSounds)
            DeathAudioStreamRandomizer.AddStream(-1, audioStream);

        DeathSoundPlayer.Stream = DeathAudioStreamRandomizer;

        // Set pitch for all audio players and add random pitch +/-
        var audioPlayers = new[]
        {
            IdleSoundPlayer,
            MovementSoundPlayer,
            NoticedSoundPlayer,
            SpawnedSoundPlayer,
            DeathSoundPlayer
        };

        foreach (var player in audioPlayers)
            player.PitchScale = CharacterResource.Pitch + (float)GD.RandRange(-CharacterResource.RandomPitch, CharacterResource.RandomPitch);
    }

    public void SetOrientation(Vector2 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            if (direction.X >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Right;
            else if (direction.X < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Left;
        }
        else
        {
            if (direction.Y >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Down;
            else if (direction.Y < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Up;
        }
    }

    public virtual void TurnTowards(Vector2 targetPosition)
    {
        if (ActivityState == ActivityStateEnum.Active && (LifeState == LifeStateEnum.Living || LifeState == LifeStateEnum.Hit))
        {
            Vector2 direction = Position.DirectionTo(targetPosition);
            SetOrientation(direction);
        }
    }

    public async void Die()
    {
        LifeState = LifeStateEnum.Dying;
        DeathSoundPlayer.Play();
        AnimatedSprite2D.Play("death");

        await ToSignal(AnimatedSprite2D, "animation_finished");
        EmitSignal(SignalName.Died, Position);
    }

    public virtual void OnPickupAreaAreaEntered(Area2D area)
    {
    }

    public virtual void OnCharacterControllerCharacterNoticed(Character player)
    {
        NoticedSoundPlayer.Play();
    }

    public void SetCurrentTileData(TileData tileData)
    {
        if (tileData != null)
            MovementSoundPlayer.CurrentSoundSet = tileData.GetCustomData("surface").ToString();
    }

    public virtual void OnHit(int damage)
    {
        if (ActivityState == ActivityStateEnum.Active && LifeState == LifeStateEnum.Living)
        {
            Health -= damage;

            if (Health <= 0)
                Die();
            else
            {
                HitSoundPlayer.Play();

                // Flash character
                LifeState = LifeStateEnum.Hit;
            }
            // EmitSignal(SignalName.Hit, Position, CharacterResource.HitExplosionResource);
            EmitSignal(SignalName.HealthChanged, Health, CharacterResource.HealthMax);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (ActivityState == ActivityStateEnum.Active && MovementState == MovementStateEnum.Moving)
        {
            if (LifeState == LifeStateEnum.Living || LifeState == LifeStateEnum.Hit)
            {
                // Check if character is close to target position
                // if (Position.DistanceTo(NavigationAgent2D.TargetPosition) < 5)
                // 	EmitSignal(SignalName.TargetPositionReached);

                if (NavigationAgent2D.IsNavigationFinished())
                {
                    AnimatedSprite2D.Play("idle_" + Orientation.ToString().ToLower());
                    MovementState = MovementStateEnum.Idle;
                    EmitSignal(SignalName.TargetPositionReached);
                    Logger.Log($"Character {Name} reached target position", Logger.LogTypeEnum.Character);
                }
                else
                {
                    Vector2 currentAgentPosition = Position;
                    Vector2 nextPathPosition = NavigationAgent2D.GetNextPathPosition();

                    Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed;

                    // SetOrientation(Velocity);

                    AnimatedSprite2D.Play("walk_" + Orientation.ToString().ToLower());
                    MoveAndSlide();

                    if (Velocity.Length() > 0)
                        MovementState = MovementStateEnum.Moving;
                    else
                        MovementState = MovementStateEnum.Idle;
                }

                // Check if character is on a different tile than before
                Vector2I currentTile = (Vector2I)(Position / TileSize);

                if (currentTile != LastCheckedTile)
                {
                    LastCheckedTile = currentTile;
                    EmitSignal(SignalName.RequestCurrentTileData, this);
                    // GD.Print("New tile entered");
                }
            }
        }
    }
}
