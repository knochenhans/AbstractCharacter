using Godot;
using System;

public partial class PlayerControllerStandard2D : PlayerController2D
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (ControlledCharacter is not AbstractCharacter2D character)
			return;

		// if (ActivityState == ActivityStateEnum.Active && ControlledCharacter != null)
		// {
		// 	if (@event is InputEventKey eventKey)
		// 	{
		// 		if (eventKey.Pressed && eventKey.Keycode == Key.W)
		// 		{
		// 			ControlledCharacter.Velocity = new Vector2(ControlledCharacter.Velocity.X, -1);
		// 		}
		// 		else if (eventKey.Pressed && eventKey.Keycode == Key.S)
		// 		{
		// 			ControlledCharacter.Velocity = new Vector2(ControlledCharacter.Velocity.X, 1);
		// 		}
		// 		else if (eventKey.Pressed && eventKey.Keycode == Key.A)
		// 		{
		// 			ControlledCharacter.Velocity = new Vector2(-1, ControlledCharacter.Velocity.Y);
		// 		}
		// 		else if (eventKey.Pressed && eventKey.Keycode == Key.D)
		// 		{
		// 			ControlledCharacter.Velocity = new Vector2(1, ControlledCharacter.Velocity.Y);
		// 		}
		// 		ControlledCharacter.Velocity = ControlledCharacter.Velocity.Normalized() * character.MovementSpeed;
		// 	}
		// }
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (ControlledCharacter == null)
			return;

		if (ActivityState == ActivityStateEnum.Active)
		{
			ControlledCharacter.Velocity = Vector2.Zero;
			if (Input.IsKeyPressed(Key.W))
			{
				ControlledCharacter.Velocity += new Vector2(0, -1).Normalized() * ControlledCharacter.MovementSpeed;
			}
			if (Input.IsKeyPressed(Key.S))
			{
				ControlledCharacter.Velocity += new Vector2(0, 1).Normalized() * ControlledCharacter.MovementSpeed;
			}
			if (Input.IsKeyPressed(Key.A))
			{
				ControlledCharacter.Velocity += new Vector2(-1, 0).Normalized() * ControlledCharacter.MovementSpeed;
			}
			if (Input.IsKeyPressed(Key.D))
			{
				ControlledCharacter.Velocity += new Vector2(1, 0).Normalized() * ControlledCharacter.MovementSpeed;
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}
}
