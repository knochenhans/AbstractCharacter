using Godot;

public partial class ExamplePlayerController : AbstractCharacterController
{
    public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (ActivityState == ActivityStateEnum.Active)
		{
            if (@event is InputEventMouseMotion)
				LookAtTarget2D = GetGlobalMousePosition();

			if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
				MovementTarget2D = GetGlobalMousePosition();
		}
	}

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