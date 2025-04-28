using System;
using Godot;

public partial class AbstractCharacter : Node
{
    [Export] public AbstractCharacterResource CharacterResource { get; set; }
    [Export] public PackedScene CharacterControllerScene { get; set; }

    Node CharacterBody => GetParent<Node>();

    // [Signal] public delegate void RequestCurrentTileDataEventHandler(AbstractCharacter2D character);
    // [Signal] public delegate void TargetPositionSetEventHandler(Vector2 position);
    // [Signal] public delegate void TargetPositionReachedEventHandler();
    [Signal] public delegate void HealthChangedEventHandler(int health, int healthMax);
    [Signal] public delegate void DiedEventHandler();

    // public CharacterSoundManager2D SoundManager;
    // protected NavigationAgent2D NavigationAgent2D;

    public CharacterStateManager StateManager;
    // public CharacterTileManager TileManager;

    private int _health;
    public int Health { get; set; }

    public ActivityStateEnum ActivityState { get; set; }
    public MovementStateEnum MovementState { get; set; }

    public Timer LifeStateTimer { get; private set; }

    public SpriteFrames GetSpriteFrames()
    {
        if (CharacterResource == null)
            throw new Exception("CharacterResource is not set.");

        CharacterResource.SpriteFrames.ResourceLocalToScene = true;
        return CharacterResource.SpriteFrames;
    }

    public void Init()
    {
        if (CharacterResource == null)
            throw new Exception("CharacterResource is not set.");

        Logger.Log($"Initializing character node {CharacterResource.ID}", Logger.LogTypeEnum.Character);

        LifeStateTimer = new Timer();
        AddChild(LifeStateTimer);
        LifeStateTimer.Timeout += OnLifeStateTimeout;

        // SoundManager = new CharacterSoundManager2D(this);

        var characterController = CharacterControllerScene.Instantiate() as AbstractCharacterController;
        AddChild(characterController);

        StateManager = new CharacterStateManager(this);

        // NavigationAgent2D = new NavigationAgent2D();
        // AddChild(NavigationAgent2D);

        Health = CharacterResource.HealthMax;

        StateManager.LifeStateChanged += OnLifeStateChanged;
        StateManager.SetLifeState(CharacterResource.InitialLifeState);
    }

    public virtual AbstractCharacter WithData(AbstractCharacterResource characterResource, PackedScene characterControllerScene)
    {
        CharacterResource = characterResource;
        CharacterControllerScene = characterControllerScene;
        return this;
    }

    public virtual void Hit(int damage)
    {
        if (ActivityState != ActivityStateEnum.Active)
            return;

        if (!StateManager.LifeState.DamageActive)
            return;

        Health -= damage;

        if (Health <= 0)
            StateManager.SetLifeState("dying");
        else if (Health < CharacterResource.HealthMax)
            StateManager.SetLifeState(CharacterResource.StateAfterHit);
        else
            StateManager.SetLifeState(CharacterResource.IdleState);

        EmitSignal(SignalName.HealthChanged, Health, CharacterResource.HealthMax);
    }

    private void OnLifeStateTimeout() => StateManager.TransitionToNextState();

    public bool CanMove()
    {
        if (ActivityState != ActivityStateEnum.Active)
            return false;

        if (!StateManager.LifeState.MovementActive)
            return false;

        return true;
    }

    public void OnLifeStateChanged(CharacterState state)
    {
        if (state.ID == CharacterResource.DeadState)
            EmitSignal(SignalName.Died);

        Logger.Log($"Character {Name} changed life state to {state.ID}", Logger.LogTypeEnum.Character);
    }
}
