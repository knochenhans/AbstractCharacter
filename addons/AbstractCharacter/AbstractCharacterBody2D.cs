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

    public float Friction { get; set; }
    public float MovementSpeed { get; set; }

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

        Character.Init();
        Character.StateManager.LifeStateChanged += OnLifeStateChanged;

        AnimatedSprite2D.SpriteFrames = Character.GetSpriteFrames();

        InitAreas();

        Character.StateManager.SetLifeState(Character.CharacterResource.InitialLifeState);
        CharacterController = Character.GetNode<AbstractCharacterController>("CharacterController");
        Orientation = Character.CharacterResource.InitialOrientation;
        MovementSpeed = Character.CharacterResource.MovementSpeed;

        Friction = 5;
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
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (Character.CanMove())
        {
            Velocity = CharacterController.Velocity2D.Normalized() * MovementSpeed;

            MoveAndSlide();
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction);
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
}
