using Godot;

public partial class PlayerControllerStandard2D : AbstractCharacterController
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Input.IsActionPressed("ui_right"))
        {
            Velocity2D.X = 1;
        }
        else if (Input.IsActionPressed("ui_left"))
        {
            Velocity2D.X = -1;
        }
        else if (Input.IsActionPressed("ui_up"))
        {
            Velocity2D.Y = -1;
        }
        else if (Input.IsActionPressed("ui_down"))
        {
            Velocity2D.Y = 1;
        }
    }
}