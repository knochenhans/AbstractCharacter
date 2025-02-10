using System;
using Godot;

public partial class AbstractCharacter3D : CharacterBody3D, ICharacter
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    [Signal] public delegate void RequestCurrentTileDataEventHandler(AbstractCharacter3D character);
    [Signal] public delegate void DiedEventHandler(Vector3 position);
    [Signal] public delegate void TargetPositionSetEventHandler(Vector3 position);
    [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int health, int healthMax);

    public CharacterSoundManager3D SoundManager;
    protected NavigationAgent3D NavigationAgent3D;
    protected AnimatedSprite3D AnimatedSprite3D => GetNode<AnimatedSprite3D>("AnimatedSprite3D");

    public CharacterStateManager StateManager;
    public CharacterTileManager TileManager;
    public CharacterControllerManager3D ControllerManager;
    public CharacterAreaManager3D AreaManager;

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

    public Vector2 MovementTarget2D { get; set; }

    public Vector3 MovementTarget3D
    {
        get => NavigationAgent3D.TargetPosition;
        set
        {
            NavigationAgent3D.TargetPosition = value;
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
            EmitSignal(SignalName.TargetPositionSet, value);
        }
    }

    public override void _Ready()
    {
        Logger.Log($"Initializing character node {Name}", Logger.LogTypeEnum.Character);

        Orientation = CharacterResource.InitialOrientation;

        AnimatedSprite3D.SpriteFrames = CharacterResource.SpriteFrames;
        AnimatedSprite3D.SpriteFrames.ResourceLocalToScene = true;

        LifeStateTimer.Timeout += OnLifeStateTimeout;

        StateManager = new CharacterStateManager(this);
        StateManager.SetLifeState("spawn");
        StateManager.LifeStateChanged += OnLifeStateChanged;

        SoundManager = new CharacterSoundManager3D(this);
        ControllerManager = new CharacterControllerManager3D(this);
        AreaManager = new CharacterAreaManager3D(this);

        NavigationAgent3D = new NavigationAgent3D();
        AddChild(NavigationAgent3D);

        Health = CharacterResource.HealthMax;
        MovementSpeed = CharacterResource.MovementSpeed;
        Friction = 5;
    }

    public virtual AbstractCharacter3D WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
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
                EmitSignal(SignalName.Died, GlobalTransform.Origin);
                break;
        }

        SoundManager.PlaySound(newState.ID);
        PlayAnimation(newState.ID);

        Logger.Log($"Character {Name} changed life state to {newState.ID}", Logger.LogTypeEnum.Character);
    }

    public void SetOrientation(Vector3 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Z))
        {
            if (direction.X >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Right;
            else if (direction.X < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Left;
        }
        else
        {
            if (direction.Z >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Down;
            else if (direction.Z < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Up;
        }
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

    public virtual void TurnTowards(Vector3 targetPosition)
    {
        if (ActivityState != CharacterStateManager.ActivityStateEnum.Active)
            return;

        if (StateManager.LifeState.ID == "dead" || StateManager.LifeState.ID == "dying" || StateManager.LifeState.ID == "spawn")
            return;

        Vector3 direction = GlobalTransform.Origin.DirectionTo(targetPosition);
        SetOrientation(direction);
    }

    public virtual void TurnTowards(Vector2 targetPosition)
    {
        if (ActivityState != CharacterStateManager.ActivityStateEnum.Active)
            return;

        if (StateManager.LifeState.ID == "dead" || StateManager.LifeState.ID == "dying" || StateManager.LifeState.ID == "spawn")
            return;

        Vector3 direction = GlobalTransform.Origin.DirectionTo(new Vector3(targetPosition.X, GlobalTransform.Origin.Y, targetPosition.Y));
        SetOrientation(direction);
    }

    public virtual void OnPickupAreaAreaEntered(Area3D area)
    { }

    public virtual void OnCharacterControllerCharacterNoticed(AbstractCharacter3D player) => SoundManager.PlaySound("noticed");

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
        if (AnimatedSprite3D.SpriteFrames.HasAnimation(animationName_))
            AnimatedSprite3D.Play(animationName_);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (ActivityState == CharacterStateManager.ActivityStateEnum.Inactive)
            return;

        if (StateManager.LifeState.ID == "dead" || StateManager.LifeState.ID == "dying" || StateManager.LifeState.ID == "spawn")
            return;

        MoveAndSlide();
        Velocity = Velocity.MoveToward(Vector3.Zero, Friction);

        if (Velocity.Length() > 0)
            MovementState = CharacterStateManager.MovementStateEnum.Moving;
        else
            MovementState = CharacterStateManager.MovementStateEnum.Idle;
    }
}
