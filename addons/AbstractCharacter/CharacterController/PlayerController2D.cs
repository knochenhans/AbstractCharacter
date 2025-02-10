using Godot;
using System;

public partial class PlayerController2D : AbstractCharacterController2D
{
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void OnScanArea2DBodyEntered(Node body)
	{
	}

	public override void OnScanArea2DBodyExited(Node body)
	{
	}

	public override void OnWeaponRangeEntered(Node2D body)
	{
	}

	public override void OnWeaponRangeExited(Node2D body)
	{
	}
}
