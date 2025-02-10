using Godot;

public interface ICharacter
{
    AbstractCharacterResource CharacterResource { get; set; }
    PackedScene CharacterControllerScene { get; set; }

    CharacterStateManager StateManager { get; }
    CharacterStateManager.ActivityStateEnum ActivityState { get; set; }
    CharacterStateManager.MovementStateEnum MovementState { get; set; }
    AbstractCharacterResource.OrientationEnum Orientation { get; set; }

    int Health { get; set; }
    float MovementSpeed { get; set; }
    float Friction { get; set; }
    Vector2 MovementTarget2D { get; set; }
    Vector3 MovementTarget3D { get; set; }

    Timer LifeStateTimer { get; }

    void Hit(int damage);
    void SetOrientation(Vector2 direction);
    void SetOrientation(Vector3 direction);
    void TurnTowards(Vector2 targetPosition);
    void TurnTowards(Vector3 targetPosition);
    void PlayAnimation(string animationPrefix);
}