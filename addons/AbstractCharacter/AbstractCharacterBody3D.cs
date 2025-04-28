using System;
using Godot;

public partial class AbstractCharacterBody3D : CharacterBody3D, IAbstractCharacterBody
{
    public AbstractCharacter Character => GetNode<AbstractCharacter>("Character");
    public AnimatedSprite3D AnimatedSprite3D => GetNode<AnimatedSprite3D>("AnimatedSprite3D");
    public Area3D ScanArea => GetNode<Area3D>("ScanArea");
    public Area3D PickupArea => GetNode<Area3D>("PickupArea");

    public AbstractCharacterController CharacterController { get; private set; }
    public AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    public void PlayAnimation(string animationPrefix)
    {
        var animationName = animationPrefix + "_" + Orientation.ToString().ToLower();

        if (AnimatedSprite3D.SpriteFrames.HasAnimation(animationName))
            AnimatedSprite3D.Play(animationName);
    }

    public void OnLifeStateChanged(CharacterState state)
    {
        PlayAnimation(state.ID);
    }

    public void InitCharacter()
    {
        Character.Init();
        Character.StateManager.LifeStateChanged += OnLifeStateChanged;

        AnimatedSprite3D.SpriteFrames = Character.GetSpriteFrames();

        CharacterController = Character.GetNode<AbstractCharacterController>("CharacterController");
    }

    public void InitAreas()
    {
        foreach (var area in new[] { ScanArea, PickupArea })
        {
            var collisionShape = area.GetNode<CollisionShape3D>("CollisionShape3D");
            if (collisionShape.Shape is SphereShape3D sphereShape)
            {
                sphereShape.Radius = area == ScanArea
                    ? Character.CharacterResource.ScanRadius
                    : Character.CharacterResource.PickupRadius;
            }
        }
    }

    public void SetOrientation(Vector3 direction)
    {
        if (Math.Abs(direction.X) > Math.Abs(direction.Z) && Math.Abs(direction.X) > Math.Abs(direction.Y))
        {
            if (direction.X >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Right;
            else if (direction.X < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Left;
        }
        else if (Math.Abs(direction.Z) > Math.Abs(direction.Y))
        {
            if (direction.Z >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Down;
            else if (direction.Z < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Up;
        }
        else
        {
            if (direction.Y >= 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Forward;
            else if (direction.Y < 0)
                Orientation = AbstractCharacterResource.OrientationEnum.Backward;
        }
    }

    public virtual void TurnTowards(Vector3 targetPosition)
    {
        if (Character.CanMove())
        {
            Vector3 direction = Position.DirectionTo(targetPosition);
            SetOrientation(direction);
        }
    }
}
