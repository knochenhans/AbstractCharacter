using Godot;
using System;

public partial class EnemyController2D : NonPlayerController2D
{
	public override void OnScanArea2DBodyEntered(Node body)
	{
		if (body.IsInGroup("player"))
		{
			var player = body as AbstractCharacter2D;
			targetCharacter = player;
			FollowingState = FollowingStateEnum.Following;
			Logger.Log($"EnemyController: {ControlledCharacter.Name} is chasing {player.Name}", Logger.LogTypeEnum.Character);
			EmitSignal(SignalName.CharacterNoticed, player);
		}
	}

	public override void OnScanArea2DBodyExited(Node body)
	{
		if (body.IsInGroup("player"))
		{
			FollowingState = FollowingStateEnum.Idle;
		}
	}

	public override void OnWeaponRangeEntered(Node2D body)
	{
		if (body is AbstractCharacter2D character)
		{
			if (character != ControlledCharacter && character.IsInGroup("player"))
			{
				Logger.Log($"EnemyController: {ControlledCharacter.Name} is attacking {character.Name}", Logger.LogTypeEnum.Character);
				// (ControlledCharacter as Character).StartShooting();
			}
		}
	}

	public override void OnWeaponRangeExited(Node2D body)
	{
		if (body is AbstractCharacter2D character)
		{
			if (character != ControlledCharacter)
			{
				// (ControlledCharacter as Character).StopShooting();
			}
		}
	}
}
