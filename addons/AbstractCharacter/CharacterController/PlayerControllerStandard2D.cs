using Godot;

public partial class PlayerControllerStandard2D : AbstractCharacterController
{
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (ActivityState == ActivityStateEnum.Active)
        {
            MovementDirection2D = Vector2.Zero;

            if (Input.IsActionPressed("ui_right"))
                MovementDirection2D.X = 1;
            else if (Input.IsActionPressed("ui_left"))
                MovementDirection2D.X = -1;

            if (Input.IsActionPressed("ui_up"))
                MovementDirection2D.Y = -1;
            else if (Input.IsActionPressed("ui_down"))
                MovementDirection2D.Y = 1;
        }
    }
}