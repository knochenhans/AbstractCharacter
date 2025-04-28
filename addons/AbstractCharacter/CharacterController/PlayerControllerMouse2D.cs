using Godot;
using System;

public partial class PlayerControllerMouse2D : AbstractCharacterController
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
			if (Input.IsMouseButtonPressed(MouseButton.Right))
			{
				// if ((ControlledCharacter as AbstractCharacter2D).StartShooting())
				// 	Input.SetCustomMouseCursor(CrossCursor, hotspot: new Vector2(24, 24));
			}
			else
			{
				// Input.SetCustomMouseCursor(ArrowCursor, hotspot: new Vector2(0, 0));
				// (ControlledCharacter as Character).StopShooting();
			}
		}
	}

	// public override void _PhysicsProcess(double delta)
	// {
	// 	base._PhysicsProcess(delta);

	// 	if (ControlledCharacter != null)
	// 	{
	// 		// Update player target position if left mouse button is pressed
	// 		// if (Input.IsMouseButtonPressed(MouseButton.Left))
	// 		// 	Character.MovementTarget = GetGlobalMousePosition();
	// 	}
	// }
}
