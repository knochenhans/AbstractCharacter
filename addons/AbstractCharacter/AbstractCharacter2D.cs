using System;
using Godot;

public partial class AbstractCharacter2D : CharacterBody2D, ICharacter
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    [Signal] public delegate void RequestCurrentTileDataEventHandler(AbstractCharacter2D character);
    [Signal] public delegate void DiedEventHandler(Vector2 position);
    [Signal] public delegate void TargetPositionSetEventHandler(Vector2 position);
    [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int health, int healthMax);

    public CharacterSoundManager2D SoundManager;
    protected NavigationAgent2D NavigationAgent2D;
    protected AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("AnimatedSprite2D");

    public CharacterStateManager StateManager;
    public CharacterTileManager TileManager;
    public CharacterControllerManager2D ControllerManager;
    public CharacterAreaManager2D AreaManager;

    private int _health;
    public int Health
    {
        get => _health;
        set => _health = value;
    }

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

    CharacterStateManager ICharacter.StateManager => StateManager;

    public Timer LifeStateTimer => GetNode<Timer>("LifeStateTimer");

    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    public float MovementSpeed { get; set; }
    public float Friction { get; set; }

    public Vector2 MovementTarget2D
    {
        get => NavigationAgent2D.TargetPosition;
        set
        {
            NavigationAgent2D.TargetPosition = value;
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, value);
        }
    }

    public Vector3 MovementTarget3D
    {
        get => new Vector3(NavigationAgent2D.TargetPosition.X, NavigationAgent2D.TargetPosition.Y, 0);
        set
        {
            NavigationAgent2D.TargetPosition = new Vector2(value.X, value.Y);
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, new Vector2(value.X, value.Y));
        }
    }

    public override void _Ready()
    {
        Logger.Log($"Initializing character node {Name}", Logger.LogTypeEnum.Character);

        Orientation = CharacterResource.InitialOrientation;

        AnimatedSprite2D.SpriteFrames = CharacterResource.SpriteFrames;
        AnimatedSprite2D.SpriteFrames.ResourceLocalToScene = true;

        LifeStateTimer.Timeout += OnLifeStateTimeout;

        StateManager = new CharacterStateManager(this);
        StateManager.SetLifeState("spawn");
        StateManager.LifeStateChanged += OnLifeStateChanged;

        SoundManager = new CharacterSoundManager2D(this);
        ControllerManager = new CharacterControllerManager2D(this);
        AreaManager = new CharacterAreaManager2D(this);

        NavigationAgent2D = new NavigationAgent2D();
        AddChild(NavigationAgent2D);

        Health = CharacterResource.HealthMax;
        MovementSpeed = CharacterResource.MovementSpeed;
        Friction = 5;
    }

    public virtual AbstractCharacter2D WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
    {
        CharacterResource = characterResource;
        CharacterControllerScene = characterControllerScene;
        return this;
    }

    private void OnLifeStateChanged(CharacterState newState)
    {
        switch (newState.ID)
        {
            case "dead":
                EmitSignal(SignalName.Died, Position);
                break;
        }

        SoundManager.PlaySound(newState.ID);
        PlayAnimation(newState.ID);

        Logger.Log($"Character {Name} changed life state to {newState.ID}", Logger.LogTypeEnum.Character);
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

    public void SetOrientation(Vector3 direction)
    {
        SetOrientation(new Vector2(direction.X, direction.Y));
    }

    public virtual void TurnTowards(Vector2 targetPosition)
    {
        if (ActivityState != CharacterStateManager.ActivityStateEnum.Active)
            return;

        if (StateManager.LifeState.ID == "dead" || StateManager.LifeState.ID == "dying" || StateManager.LifeState.ID == "spawn")
            return;

        Vector2 direction = Position.DirectionTo(targetPosition);
        SetOrientation(direction);
    }

    public virtual void TurnTowards(Vector3 targetPosition)
    {
        TurnTowards(new Vector2(targetPosition.X, targetPosition.Y));
    }

    public virtual void OnPickupAreaAreaEntered(Area2D area)
    { }

    public virtual void OnCharacterControllerCharacterNoticed(AbstractCharacter2D player) => SoundManager.PlaySound("noticed");

    public virtual void Hit(int damage)
    {
        if (ActivityState != CharacterStateManager.ActivityStateEnum.Active)
            return;

        if (StateManager.LifeState.ID != "idle")
            return;

        Health -= damage;

        if (Health <= 0)
            StateManager.SetLifeState("dying");
        else if (Health < CharacterResource.HealthMax)
            StateManager.SetLifeState("hit");
        else
            StateManager.SetLifeState("idle");

        EmitSignal(SignalName.HealthChanged, Health, CharacterResource.HealthMax);
    }

    private void OnLifeStateTimeout() => StateManager.TransitionToNextState();

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

        if (StateManager.LifeState.ID == "dead" || StateManager.LifeState.ID == "dying" || StateManager.LifeState.ID == "spawn")
            return;

        MoveAndSlide();
        Velocity = Velocity.MoveToward(Vector2.Zero, Friction);

        if (Velocity.Length() > 0)
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
        else
            MovementState = CharacterStateManager.MovementStateEnum.Idle;
    }
}
