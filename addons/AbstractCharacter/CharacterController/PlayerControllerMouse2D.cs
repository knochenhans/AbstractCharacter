using Godot;
using System;

public partial class PlayerControllerMouse2D : PlayerController2D
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (ActivityState == ActivityStateEnum.Active && ControlledCharacter != null)
		{
			// Set character orientation based on mouse position in a circle around the character
			if (@event is InputEventMouseMotion mouseMotion)
			{
				Vector2 mousePosition = GetGlobalMousePosition();
				ControlledCharacter.TurnTowards(mousePosition);
			}

			// Move character to mouse position
			// if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
			// 	ControlledCharacter.MovementTarget = GetGlobalMousePosition();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (ActivityState == ActivityStateEnum.Active && ControlledCharacter != null)
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

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (ControlledCharacter != null)
		{
			// Update player target position if left mouse button is pressed
			// if (Input.IsMouseButtonPressed(MouseButton.Left))
			// 	Character.MovementTarget = GetGlobalMousePosition();
		}
	}
}
