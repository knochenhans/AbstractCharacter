using System;
using Godot;

public class CharacterStateManager
{
    private AbstractCharacter2D _character;

    public CharacterStateManager(AbstractCharacter2D character) => _character = character;

    public enum ActivityStateEnum { Active, Inactive }
    public enum MovementStateEnum { Idle, Moving }
    public enum LifeStateEnum { None, Living, Dead }
    public enum LifeSubStateEnum { None, Spawning, Dying, Hit }

    private ActivityStateEnum _activityState = ActivityStateEnum.Active;
    private MovementStateEnum _movementState = MovementStateEnum.Idle;
    private LifeStateEnum _lifeState = LifeStateEnum.Living;
    private LifeSubStateEnum _lifeSubState = LifeSubStateEnum.None;

    public ActivityStateEnum ActivityState
    {
        get => _activityState;
        set
        {
            if (_activityState != value)
            {
                _activityState = value;
                OnActivityStateChange(value);
            }
        }
    }

    public MovementStateEnum MovementState
    {
        get => _movementState;
        set
        {
            if (_movementState != value)
            {
                _movementState = value;
                OnMovementStateChange(value);
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
                _lifeState = value;
                OnLifeStateChange(value);
            }
        }
    }

    public LifeSubStateEnum LifeSubState
    {
        get => _lifeSubState;
        set
        {
            if (_lifeSubState != value)
            {
                _lifeSubState = value;
                OnSubLifeStateChange(value);
            }
        }
    }

    protected virtual void OnActivityStateChange(ActivityStateEnum newState)
    {
        _character.SoundManager.SetActivityState(CharacterSoundManager.SoundType.Hit, newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive);
        _character.SoundManager.SetActivityState(CharacterSoundManager.SoundType.Idle, newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive);
    }

    protected virtual void OnMovementStateChange(MovementStateEnum newState)
    {
        switch (newState)
        {
            case MovementStateEnum.Idle:
                // _currentAnimationPrefix = CharacterResource.IdlePrefix;
                _character.SoundManager.StopLoop(CharacterSoundManager.SoundType.Movement);
                break;
            case MovementStateEnum.Moving:
                // _currentAnimationPrefix = CharacterResource.MovementPrefix;
                _character.EmitSignal(AbstractCharacter2D.SignalName.RequestCurrentTileData, _character);
                _character.SoundManager.StartLoop(CharacterSoundManager.SoundType.Movement);
                break;
        }
    }

    protected virtual void OnLifeStateChange(LifeStateEnum newState)
    {
        switch (newState)
        {
            case LifeStateEnum.Living:
                Logger.Log($"Character {_character.Name} is living", Logger.LogTypeEnum.Character);
                break;
            case LifeStateEnum.Dead:
                Logger.Log($"Character {_character.Name} is dead", Logger.LogTypeEnum.Character);
                _character.OnDead();
                break;
        }
    }

    protected virtual void OnSubLifeStateChange(LifeSubStateEnum newState)
    {
        switch (newState)
        {
            case LifeSubStateEnum.Spawning:
                Logger.Log($"Character {_character.Name} is spawning", Logger.LogTypeEnum.Character);
                _character.OnSpawning();
                break;
            case LifeSubStateEnum.Dying:
                Logger.Log($"Character {_character.Name} is dying", Logger.LogTypeEnum.Character);
                _character.OnDying();
                break;
            case LifeSubStateEnum.Hit:
                Logger.Log($"Character {_character.Name} is hit", Logger.LogTypeEnum.Character);
                _character.OnHit();
                break;
        }
    }
}

public class CharacterTileManager
{
    private AbstractCharacter2D _character;

    public CharacterTileManager(AbstractCharacter2D character) => _character = character;

    public Vector2I TileSize { get; set; } = Vector2I.Zero;
    public Vector2I LastCheckedTile { get; set; } = Vector2I.Zero;

    public void SetCurrentTileData(TileData tileData)
    {
        if (tileData != null)
            _character.SoundManager.SetCurrentSoundSet(CharacterSoundManager.SoundType.Movement, tileData.GetCustomData("surface").ToString());
    }
}

public class CharacterSoundManager
{
    private AbstractCharacter2D _character;

    public enum SoundType
    {
        Movement,
        Idle,
        Death,
        Noticed,
        Hit,
        Spawn
    }

    public CharacterSoundManager(AbstractCharacter2D character)
    {
        _character = character;

        MovementSound = _character.GetNode<TimedAudioStreamPlayer2D>("MovementSound");
        IdleSound = _character.GetNode<TimedAudioStreamPlayer2D>("IdleSound");
        DeathSound = _character.GetNode<AudioStreamPlayer2D>("DeathSound");
        NoticedSound = _character.GetNode<AudioStreamPlayer2D>("NoticedSound");
        HitSound = _character.GetNode<TimedAudioStreamPlayer2D>("HitSound");
        SpawnedSound = _character.GetNode<AudioStreamPlayer2D>("SpawnedSound");

        SetupSounds();
    }

    private TimedAudioStreamPlayer2D MovementSound;
    private TimedAudioStreamPlayer2D IdleSound;
    private AudioStreamPlayer2D DeathSound;
    private AudioStreamPlayer2D NoticedSound;
    private TimedAudioStreamPlayer2D HitSound;
    private AudioStreamPlayer2D SpawnedSound;

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Movement:
                if (MovementSound.CurrentSoundSet != null)
                    MovementSound.Play();
                break;
            case SoundType.Idle:
                IdleSound.Play();
                break;
            case SoundType.Death:
                DeathSound.Play();
                break;
            case SoundType.Noticed:
                NoticedSound.Play();
                break;
            case SoundType.Hit:
                HitSound.Play();
                break;
            case SoundType.Spawn:
                SpawnedSound.Play();
                break;
        }
    }

    public void SetCurrentSoundSet(SoundType soundType, string soundSet)
    {
        switch (soundType)
        {
            case SoundType.Movement:
                MovementSound.CurrentSoundSet = soundSet;
                break;
            case SoundType.Idle:
                IdleSound.CurrentSoundSet = soundSet;
                break;
        }
    }

    public void SetActivityState(SoundType soundType, TimedAudioStreamPlayer2D.ActivityStateEnum activityState)
    {
        switch (soundType)
        {
            case SoundType.Movement:
                MovementSound.ActivityState = activityState;
                break;
            case SoundType.Idle:
                IdleSound.ActivityState = activityState;
                break;
        }
    }

    public void StartLoop(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Movement:
                MovementSound.StartLoop();
                break;
        }
    }

    public void StopLoop(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Movement:
                MovementSound.StopLoop();
                break;
        }
    }

    public void SetupSounds()
    {
        MovementSound.AddSoundSetsFromRaw(_character.CharacterResource.MovementSounds);
        IdleSound.AddSoundSet("idle", _character.CharacterResource.IdleSounds);
        IdleSound.CurrentSoundSet = "idle";
        HitSound.SetStreams(_character.CharacterResource.HitSounds);

        var DeathAudioStreamRandomizer = new AudioStreamRandomizer();
        var NoticedAudioStreamRandomizer = new AudioStreamRandomizer();
        var SpawnedAudioStreamRandomizer = new AudioStreamRandomizer();

        foreach (AudioStream audioStream in _character.CharacterResource.NoticeSounds)
            NoticedAudioStreamRandomizer.AddStream(-1, audioStream);

        NoticedSound.Stream = NoticedAudioStreamRandomizer;

        foreach (AudioStream audioStream in _character.CharacterResource.SpawnSounds)
            SpawnedAudioStreamRandomizer.AddStream(-1, audioStream);

        SpawnedSound.Stream = SpawnedAudioStreamRandomizer;

        foreach (AudioStream audioStream in _character.CharacterResource.DeathSounds)
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
            player.PitchScale = _character.CharacterResource.Pitch + (float)GD.RandRange(-_character.CharacterResource.RandomPitch, _character.CharacterResource.RandomPitch);
    }
}

public class CharacterControllerManager
{
    private AbstractCharacter2D _character;

    public AbstractCharacterController2D CharacterController { get; set; }

    public CharacterControllerManager(AbstractCharacter2D character)
    {
        _character = character;

        CharacterController = _character.CharacterControllerScene.Instantiate() as AbstractCharacterController2D;
        CharacterController.ControlledCharacter = _character;
        CharacterController.CharacterNoticed += OnCharacterControllerCharacterNoticed;
        _character.AddChild(CharacterController);
    }

    public void OnCharacterControllerCharacterNoticed(AbstractCharacter2D player) => _character.OnCharacterControllerCharacterNoticed(player);
    public void OnScanArea2DBodyEntered(Node2D body) => CharacterController.OnScanArea2DBodyEntered(body);
    public void OnScanArea2DBodyExited(Node2D body) => CharacterController.OnScanArea2DBodyExited(body);
}

public class CharacterAreaManager
{
    private AbstractCharacter2D _character;

    public Area2D ScanArea => _character.GetNode<Area2D>("ScanArea");
    public Area2D PickupArea => _character.GetNode<Area2D>("PickupArea");

    public CharacterAreaManager(AbstractCharacter2D character)
    {
        _character = character;

        Area2D scanArea = _character.GetNode<Area2D>("ScanArea");
        scanArea.BodyEntered += (body) => _character.ControllerManager.OnScanArea2DBodyEntered(body);
        scanArea.BodyExited += (body) => _character.ControllerManager.OnScanArea2DBodyExited(body);

        var scanAreaShape = scanArea.GetNode<CollisionShape2D>("CollisionShape2D");
        (scanAreaShape.Shape as CircleShape2D).Radius = _character.CharacterResource.ScanRadius;

        PickupArea.AreaEntered += OnPickupAreaAreaEntered;
    }

    public void OnPickupAreaAreaEntered(Area2D area) => _character.OnPickupAreaAreaEntered(area);
}

public partial class AbstractCharacter2D : Node2D
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    [Signal] public delegate void RequestCurrentTileDataEventHandler(AbstractCharacter2D character);
    [Signal] public delegate void TargetPositionSetEventHandler(Vector2 position);
    [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void DiedEventHandler(Vector2 position);
    // [Signal] public delegate void HitEventHandler(Vector2 position, ExplosionResource explosionResource);
    [Signal] public delegate void HealthChangedEventHandler(int health, int healthMax);

    public CharacterStateManager StateManager;
    public CharacterTileManager TileManager;
    public CharacterSoundManager SoundManager;
    public CharacterControllerManager ControllerManager;
    public CharacterAreaManager AreaManager;
    private int health;

    public CharacterStateManager.ActivityStateEnum ActivityState
    {
        get => StateManager.ActivityState;
        set => StateManager.ActivityState = value;
    }

    public CharacterStateManager.MovementStateEnum MovementState
    {
        get => StateManager.MovementState;
        set => StateManager.MovementState = value;
    }

    public CharacterStateManager.LifeStateEnum LifeState
    {
        get => StateManager.LifeState;
        set => StateManager.LifeState = value;
    }

    protected NavigationAgent2D NavigationAgent2D => GetNode<NavigationAgent2D>("NavigationAgent2D");

    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    private int _health;
    public int Health
    {
        get => _health;
        private set
        {
            if (_health != value)
            {
                _health = value;
                OnHealthChanged();
            }
        }
    }

    protected int MovementSpeed { get; set; }

    public Vector2 MovementTarget
    {
        get => NavigationAgent2D.TargetPosition;
        set
        {
            NavigationAgent2D.TargetPosition = value;
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, value);
        }
    }

    protected AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public virtual AbstractCharacter2D WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
    {
        CharacterResource = characterResource;
        CharacterControllerScene = characterControllerScene;
        return this;
    }

    public override void _Ready()
    {
        Logger.Log($"Initializing character node {Name}", Logger.LogTypeEnum.Character);

        Orientation = CharacterResource.InitialOrientation;

        AnimatedSprite2D.SpriteFrames = CharacterResource.SpriteFrames;
        AnimatedSprite2D.SpriteFrames.ResourceLocalToScene = true;

        StateManager = new CharacterStateManager(this);
        SoundManager = new CharacterSoundManager(this);
        ControllerManager = new CharacterControllerManager(this);
        AreaManager = new CharacterAreaManager(this);

        Health = CharacterResource.HealthMax;

        LifeState = CharacterStateManager.LifeStateEnum.None;
        StateManager.LifeSubState = CharacterStateManager.LifeSubStateEnum.Spawning;
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
        if (ActivityState == CharacterStateManager.ActivityStateEnum.Active && (LifeState == CharacterStateManager.LifeStateEnum.Living || StateManager.LifeSubState == CharacterStateManager.LifeSubStateEnum.Hit))
        {
            Vector2 direction = Position.DirectionTo(targetPosition);
            SetOrientation(direction);
        }
    }

    public virtual void OnPickupAreaAreaEntered(Area2D area)
    { }

    public virtual void OnCharacterControllerCharacterNoticed(AbstractCharacter2D player) => SoundManager.PlaySound(CharacterSoundManager.SoundType.Noticed);

    public virtual void Hit(int damage)
    {
        if (ActivityState == CharacterStateManager.ActivityStateEnum.Active && LifeState == CharacterStateManager.LifeStateEnum.Living)
            Health -= damage;
    }

    private void OnHealthChanged()
    {
        if (Health <= 0)
            StateManager.LifeSubState = CharacterStateManager.LifeSubStateEnum.Dying;
        else if (Health < CharacterResource.HealthMax)
            StateManager.LifeSubState = CharacterStateManager.LifeSubStateEnum.Hit;
        else
            LifeState = CharacterStateManager.LifeStateEnum.Living;

        // EmitSignal(SignalName.Hit, Position, CharacterResource.HitExplosionResource);
        EmitSignal(SignalName.HealthChanged, Health, CharacterResource.HealthMax);
    }

    public async void OnSpawning()
    {
        SoundManager.PlaySound(CharacterSoundManager.SoundType.Spawn);
        PlayAnimation("spawn");

        var waitTime = CharacterResource.StateLengths[CharacterStateManager.LifeSubStateEnum.Spawning];
        await ToSignal(GetTree().CreateTimer(waitTime), SceneTreeTimer.SignalName.Timeout);

        LifeState = CharacterStateManager.LifeStateEnum.Living;
    }

    public async virtual void OnHit()
    {
        SoundManager.PlaySound(CharacterSoundManager.SoundType.Hit);
        PlayAnimation("hit");

        var waitTime = CharacterResource.StateLengths[CharacterStateManager.LifeSubStateEnum.Hit];
        await ToSignal(GetTree().CreateTimer(waitTime), SceneTreeTimer.SignalName.Timeout);

        LifeState = CharacterStateManager.LifeStateEnum.Living;
    }

    public async virtual void OnDying()
    {
        SoundManager.PlaySound(CharacterSoundManager.SoundType.Death);
        PlayAnimation("dying");

        var waitTime = CharacterResource.StateLengths[CharacterStateManager.LifeSubStateEnum.Dying];
        await ToSignal(GetTree().CreateTimer(waitTime), SceneTreeTimer.SignalName.Timeout);

        LifeState = CharacterStateManager.LifeStateEnum.Dead;
    }

    public void OnDead()
    {
        PlayAnimation("dead");
        EmitSignal(SignalName.Died, Position);
    }

    public void PlayAnimation(string animationPrefix)
    {
        var animationName_ = animationPrefix + "_" + Orientation.ToString().ToLower();
        if (AnimatedSprite2D.SpriteFrames.HasAnimation(animationName_))
            AnimatedSprite2D.Play(animationName_);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (ActivityState == CharacterStateManager.ActivityStateEnum.Inactive)
            return;

        if (LifeState == CharacterStateManager.LifeStateEnum.Dead)
            return;

        if (StateManager.LifeSubState == CharacterStateManager.LifeSubStateEnum.Spawning
            || StateManager.LifeSubState == CharacterStateManager.LifeSubStateEnum.Dying
            || StateManager.LifeSubState == CharacterStateManager.LifeSubStateEnum.Hit)
            return;

        if (LifeState == CharacterStateManager.LifeStateEnum.Living)
        {
            if (MovementState == CharacterStateManager.MovementStateEnum.Idle)
            {
                PlayAnimation("idle");
            }
            else
            {
                //         // Check if character is close to target position
                //         // if (Position.DistanceTo(NavigationAgent2D.TargetPosition) < 5)
                //         // 	EmitSignal(SignalName.TargetPositionReached);

                //         if (NavigationAgent2D.IsNavigationFinished())
                //         {
                // AnimatedSprite2D.Play("idle_" + Orientation.ToString().ToLower());
                // MovementState = MovementStateEnum.Idle;
                //             EmitSignal(SignalName.TargetPositionReached);
                //             Logger.Log($"Character {Name} reached target position", Logger.LogTypeEnum.Character);
                //         }
                //         else
                //         {
                //             Vector2 currentAgentPosition = Position;
                //             Vector2 nextPathPosition = NavigationAgent2D.GetNextPathPosition();

                //             Velocity = currentAgentPosition.DirectionTo(nextPathPosition) * MovementSpeed;

                //             // SetOrientation(Velocity);

                //             AnimatedSprite2D.Play("walk_" + Orientation.ToString().ToLower());
                //             MoveAndSlide();

                //             if (Velocity.Length() > 0)
                //                 MovementState = MovementStateEnum.Moving;
                //             else
                //                 MovementState = MovementStateEnum.Idle;
                //         }

                //         // Check if character is on a different tile than before
                //         Vector2I currentTile = (Vector2I)(Position / TileSize);

                //         if (currentTile != LastCheckedTile)
                //         {
                //             LastCheckedTile = currentTile;
                //             EmitSignal(SignalName.RequestCurrentTileData, this);
                //             // GD.Print("New tile entered");
                //         }
            }
        }
    }
}
