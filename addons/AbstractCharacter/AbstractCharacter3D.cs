using System;
using Godot;

public partial class AbstractCharacter3D : CharacterBody3D
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    [Signal] public delegate void RequestCurrentTileDataEventHandler(AbstractCharacter3D character);
    [Signal] public delegate void TargetPositionSetEventHandler(Vector3 position);
    [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void DiedEventHandler(Vector3 position);
    // [Signal] public delegate void HitEventHandler(Vector3 position, ExplosionResource explosionResource);
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
    private string _currentAnimationPrefix;

    protected virtual void OnActivityStateChange(ActivityStateEnum newState)
    {
        HitSound.ActivityState = newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer3D.ActivityStateEnum.Active : TimedAudioStreamPlayer3D.ActivityStateEnum.Inactive;
        IdleSound.ActivityState = newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer3D.ActivityStateEnum.Active : TimedAudioStreamPlayer3D.ActivityStateEnum.Inactive;
    }

    protected virtual void OnMovementStateChange(MovementStateEnum newState)
    {
        switch (newState)
        {
            case MovementStateEnum.Idle:
                // _currentAnimationPrefix = CharacterResource.IdlePrefix;
                MovementSound.StopLoop();
                break;
            case MovementStateEnum.Moving:
                // _currentAnimationPrefix = CharacterResource.MovementPrefix;
                EmitSignal(SignalName.RequestCurrentTileData, this);
                MovementSound.StartLoop();
                break;
        }
        PlayCurrentAnimation();
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

    protected AbstractCharacterController3D CharacterController { get; set; }

    protected NavigationAgent3D NavigationAgent3D => GetNode<NavigationAgent3D>("NavigationAgent3D");

    protected TimedAudioStreamPlayer3D HitSound => GetNode<TimedAudioStreamPlayer3D>("HitSound");
    protected TimedAudioStreamPlayer3D IdleSound => GetNode<TimedAudioStreamPlayer3D>("IdleSound");
    protected AudioStreamPlayer3D DeathSound => GetNode<AudioStreamPlayer3D>("DeathSound");
    protected AudioStreamPlayer3D NoticedSound => GetNode<AudioStreamPlayer3D>("NoticedSound");
    protected TimedAudioStreamPlayer3D MovementSound => GetNode<TimedAudioStreamPlayer3D>("MovementSound");
    protected AudioStreamPlayer3D SpawnedSound => GetNode<AudioStreamPlayer3D>("SpawnedSound");

    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    public int Health { get; set; }
    protected int MovementSpeed { get; set; }

    public Vector3I TileSize { get; set; } = Vector3I.Zero;
    public Vector3I LastCheckedTile { get; set; } = Vector3I.Zero;

    public Vector3 MovementTarget
    {
        get => NavigationAgent3D.TargetPosition;
        set
        {
            NavigationAgent3D.TargetPosition = value;
            MovementState = MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, value);
        }
    }

    protected AnimatedSprite3D AnimatedSprite3D => GetNode<AnimatedSprite3D>("AnimatedSprite3D");

    protected Area3D ScanArea => GetNode<Area3D>("ScanArea");
    protected Area3D PickupArea => GetNode<Area3D>("PickupArea");

    public virtual AbstractCharacter3D WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
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

        AnimatedSprite3D.SpriteFrames = CharacterResource.SpriteFrames;
        AnimatedSprite3D.SpriteFrames.ResourceLocalToScene = true;

        SetupSounds();

        CharacterController = CharacterControllerScene.Instantiate() as AbstractCharacterController3D;
        CharacterController.ControlledCharacter = this;
        CharacterController.CharacterNoticed += OnCharacterControllerCharacterNoticed;
        AddChild(CharacterController);

        Area3D scanArea = GetNode<Area3D>("ScanArea");
        scanArea.BodyEntered += (body) => CharacterController.OnScanArea3DBodyEntered(body);
        scanArea.BodyExited += (body) => CharacterController.OnScanArea3DBodyExited(body);

        var scanAreaShape = scanArea.GetNode<CollisionShape3D>("CollisionShape3D");
        (scanAreaShape.Shape as SphereShape3D).Radius = CharacterResource.ScanRadius;

        PickupArea.AreaEntered += OnPickupAreaAreaEntered;
        // _currentAnimationPrefix = CharacterResource.IdlePrefix;
        MovementState = MovementStateEnum.Idle;
    }

    public void Spawn() => SpawnedSound.Play();

    private void SetupSounds()
    {
        MovementSound.AddSoundSetsFromRaw(CharacterResource.MovementSounds);
        IdleSound.AddSoundSet("idle", CharacterResource.IdleSounds);
        IdleSound.CurrentSoundSet = "idle";
        HitSound.SetStreams(CharacterResource.HitSounds);

        var DeathAudioStreamRandomizer = new AudioStreamRandomizer();
        var NoticedAudioStreamRandomizer = new AudioStreamRandomizer();
        var SpawnedAudioStreamRandomizer = new AudioStreamRandomizer();

        foreach (AudioStream audioStream in CharacterResource.NoticeSounds)
            NoticedAudioStreamRandomizer.AddStream(-1, audioStream);

        NoticedSound.Stream = NoticedAudioStreamRandomizer;

        foreach (AudioStream audioStream in CharacterResource.SpawnSounds)
            SpawnedAudioStreamRandomizer.AddStream(-1, audioStream);

        SpawnedSound.Stream = SpawnedAudioStreamRandomizer;

        foreach (AudioStream audioStream in CharacterResource.DeathSounds)
            DeathAudioStreamRandomizer.AddStream(-1, audioStream);

        DeathSound.Stream = DeathAudioStreamRandomizer;

        // Set pitch for all audio players and add random pitch +/-
        var audioPlayers = new[]
        {
            IdleSound,
            MovementSound,
            NoticedSound,
            SpawnedSound,
            DeathSound
        };

        foreach (var player in audioPlayers)
            player.PitchScale = CharacterResource.Pitch + (float)GD.RandRange(-CharacterResource.RandomPitch, CharacterResource.RandomPitch);
    }

    public void SetOrientation2D(Vector2 direction)
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
        PlayCurrentAnimation();
    }

    public void PlayCurrentAnimation()
    {
        AnimatedSprite3D.Play(_currentAnimationPrefix + "_" + Orientation.ToString().ToLower());
    }

    public virtual void TurnTowards(Vector3 targetPosition)
    {
        if (ActivityState == ActivityStateEnum.Active && (LifeState == LifeStateEnum.Living || LifeState == LifeStateEnum.Hit))
        {
            Vector3 direction = Position.DirectionTo(targetPosition);
            // SetOrientation2D(direction);
        }
    }

    public async void Die()
    {
        LifeState = LifeStateEnum.Dying;
        DeathSound.Play();
        AnimatedSprite3D.Play("death");

        await ToSignal(AnimatedSprite3D, "animation_finished");
        EmitSignal(SignalName.Died, Position);
    }

    public virtual void OnPickupAreaAreaEntered(Area3D area)
    {
    }

    public virtual void OnCharacterControllerCharacterNoticed(AbstractCharacter3D player)
    {
        NoticedSound.Play();
    }

    public void SetCurrentTileData(TileData tileData)
    {
        if (tileData != null)
            MovementSound.CurrentSoundSet = tileData.GetCustomData("surface").ToString();
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
                HitSound.Play();

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
                // if (Position.DistanceTo(NavigationAgent3D.TargetPosition) < 5)
                // 	EmitSignal(SignalName.TargetPositionReached);

                // if (NavigationAgent3D.IsNavigationFinished())
                // {
                //     AnimatedSprite3D.Play("idle_" + Orientation.ToString().ToLower());
                //     MovementState = MovementStateEnum.Idle;
                //     EmitSignal(SignalName.TargetPositionReached);
                //     Logger.Log($"Character {Name} reached target position", Logger.LogTypeEnum.Character);
                // }
                // else
                // {
                // Vector3 currentAgentPosition = Position;
                // Vector3 nextPathPosition = NavigationAgent3D.GetNextPathPosition();

                // Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed;

                // SetOrientation(Velocity);

                // AnimatedSprite3D.Play("walk_" + Orientation.ToString().ToLower());
                MoveAndSlide();

                if (Velocity.Length() > 0)
                    MovementState = MovementStateEnum.Moving;
                else
                    MovementState = MovementStateEnum.Idle;
                // }

                // Check if character is on a different tile than before
                Vector3I currentTile = (Vector3I)(Position / TileSize);

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
