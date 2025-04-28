using Godot;

public interface IAbstractCharacterBody
{
    AbstractCharacter Character { get; }
    public AbstractCharacterController CharacterController { get; }

    public void PlayAnimation(string animationName);
    public void OnLifeStateChanged(CharacterState state);
    public void InitAreas();
    public void InitCharacter();
}