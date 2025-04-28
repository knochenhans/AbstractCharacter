using System;
using Godot;

public partial class AbstractCharacterBody2D : CharacterBody2D, IAbstractCharacterBody
{
    public AbstractCharacter Character => GetNode<AbstractCharacter>("Character");
    public AnimatedSprite2D AnimatedSprite2D => GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    public Area2D ScanArea => GetNode<Area2D>("ScanArea");
    public Area2D PickupArea => GetNode<Area2D>("PickupArea");

    public AbstractCharacterController CharacterController { get; private set; }
    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    // public Vector2 MovementTarget2D
    // {
    // get => NavigationAgent2D.TargetPosition;
    // set
    // {
    //     NavigationAgent2D.TargetPosition = value;
    //     MovementState = MovementStateEnum.Moving;
    //     EmitSignal(SignalName.TargetPositionSet, value);
    // }
    // }

    public override void _Ready()
    {
        base._Ready();

        InitCharacter();
        InitAreas();
    }

    public void InitCharacter()
    {
        Character.Init();
        Character.StateManager.LifeStateChanged += OnLifeStateChanged;

        AnimatedSprite2D.SpriteFrames = Character.GetSpriteFrames();

        CharacterController = Character.GetNode<AbstractCharacterController>("CharacterController");
        Orientation = Character.CharacterResource.InitialOrientation;
    }

    public void InitAreas()
    {
        foreach (var area in new[] { ScanArea, PickupArea })
        {
            var collisionShape = area.GetNode<CollisionShape2D>("CollisionShape2D");
            if (collisionShape.Shape is CircleShape2D circleShape)
            {
                circleShape.Radius = area == ScanArea
                    ? Character.CharacterResource.ScanRadius
                    : Character.CharacterResource.PickupRadius;
            }
            area.AreaEntered += area == ScanArea ? OnScanAreaEntered : OnPickupAreaEntered;
            area.AreaExited += area == ScanArea ? OnScanAreaExited : OnPickupAreaExited;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Character.CanMove())
        {
            Velocity = CharacterController.Velocity2D.Normalized() * Character.CharacterResource.MovementSpeed; ;

            MoveAndSlide();
            Velocity = Velocity.MoveToward(Vector2.Zero, Character.CharacterResource.Friction);
            CharacterController.Velocity2D = Vector2.Zero;

            if (Velocity.Length() > 0)
                Character.MovementState = MovementStateEnum.Moving;
            else
                Character.MovementState = MovementStateEnum.Idle;
        }
    }

    public void PlayAnimation(string animationPrefix)
    {
        var animationName = animationPrefix + "_" + Orientation.ToString().ToLower();

        if (AnimatedSprite2D.SpriteFrames.HasAnimation(animationName))
            AnimatedSprite2D.Play(animationName);
    }

    public void OnLifeStateChanged(CharacterState state)
    {
        PlayAnimation(state.ID);

        ScanArea.Monitoring = state.ScanActive;
        PickupArea.Monitoring = state.PickupActive;
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
        if (Character.CanMove())
        {
            Vector2 direction = Position.DirectionTo(targetPosition);
            SetOrientation(direction);
        }
    }

    public void OnScanAreaEntered(Area2D area)
    {

    }

    public void OnPickupAreaEntered(Area2D area)
    {

    }

    public void OnScanAreaExited(Area2D area)
    {

    }

    public void OnPickupAreaExited(Area2D area)
    {

    }
}
