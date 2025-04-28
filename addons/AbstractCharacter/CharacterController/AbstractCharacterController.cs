using Godot;

public partial class AbstractCharacterController : Node2D
{      
    public Vector2 MovementDirection2D;
    public Vector3 MovementDirection3D;

    public Vector2 MovementTarget2D;
    public Vector3 MovementTarget3D;

    public Vector2 LookAtDirection2D;
    public Vector3 LookAtDirection3D;

    public Vector2 LookAtTarget2D;
    public Vector3 LookAtTarget3D;
    
    public ActivityStateEnum ActivityState { get; set; } = ActivityStateEnum.Active;
}