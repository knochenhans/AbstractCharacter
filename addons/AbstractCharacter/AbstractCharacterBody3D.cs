using System;
using Godot;

public partial class AbstractCharacterBody3D : CharacterBody3D, IAbstractCharacterBody
{
    public AbstractCharacter Character => GetNode<AbstractCharacter>("Character");
    public AnimatedSprite3D AnimatedSprite3D => GetNode<AnimatedSprite3D>("AnimatedSprite3D");
    public AbstractCharacterController CharacterController { get; private set; }

    public void PlayAnimation(string animationName)
    {
        throw new NotImplementedException();
    }

    public void OnLifeStateChanged(CharacterState state)
    {
        throw new NotImplementedException();
    }

    public void InitAreas()
    {
        throw new NotImplementedException();
    }
}
