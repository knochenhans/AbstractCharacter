using Godot;
using System;

public partial class NonPlayerController2D : AbstractCharacterController2D
{
	[Export] public NonPlayerCharacterControllerResource NonPlayerControllerResource { get; set; }

	public enum FollowingStateEnum
	{
		Idle,
		Following
	}

	protected FollowingStateEnum FollowingState = FollowingStateEnum.Idle;

	protected AbstractCharacter2D targetCharacter;

	public NonPlayerCharacterControllerResource.EmotionalStateEnum EmotionalState { get; set; }

	protected Timer DecisionTimer => GetNode<Timer>("DecisionTimer");

	public Vector2 PreferredPosition { get; set; }

	public override void _Ready()
	{
		base._Ready();

		DecisionTimer.WaitTime = NonPlayerControllerResource.DecisionInterval;
		DecisionTimer.Timeout += OnDecisionTimerTimeout;

		EmotionalState = NonPlayerControllerResource.InitialEmotionalState;

		PreferredPosition = ControlledCharacter.Position;
	}

	protected void OnDecisionTimerTimeout()
	{
		if (ActivityState == ActivityStateEnum.Active)
		{
			switch (FollowingState)
			{
				case FollowingStateEnum.Idle:
					bool randomWait = new Random().Next(0, 2) == 1;

					if (!randomWait)
					{
						// switch (EmotionalState)
						// {
						// 	case NonPlayerCharacterControllerResource.EmotionalStateEnum.Explorative:
						// 		ControlledCharacter.MovementTarget = new Vector2(
						// 			ControlledCharacter.Position.X + new Random().Next(-NonPlayerControllerResource.WanderDistance, NonPlayerControllerResource.WanderDistance),
						// 			ControlledCharacter.Position.Y + new Random().Next(-NonPlayerControllerResource.WanderDistance, NonPlayerControllerResource.WanderDistance)
						// 		);
						// 		break;
						// 	case NonPlayerCharacterControllerResource.EmotionalStateEnum.Neutral:
						// 		ControlledCharacter.MovementTarget = new Vector2(
						// 			PreferredPosition.X + new Random().Next(-NonPlayerControllerResource.WanderDistance, NonPlayerControllerResource.WanderDistance),
						// 			PreferredPosition.Y + new Random().Next(-NonPlayerControllerResource.WanderDistance, NonPlayerControllerResource.WanderDistance)
						// 		);
						// 		break;
						// 	case NonPlayerCharacterControllerResource.EmotionalStateEnum.Fearful:
						// 		if (ControlledCharacter.Position != PreferredPosition)
						// 		{
						// 			ControlledCharacter.MovementTarget = PreferredPosition;
						// 		}
						// 		break;
						// }
						// ControlledCharacter.TurnTowards(ControlledCharacter.MovementTarget);
					}
					break;

				case FollowingStateEnum.Following:
					// ControlledCharacter.MovementTarget = targetCharacter.GlobalPosition;
					ControlledCharacter.TurnTowards(targetCharacter.GlobalPosition);
					break;
			}
			DecisionTimer.Start();
		}
	}

	protected void WaitIdle(float seconds)
	{
		FollowingState = FollowingStateEnum.Idle;
		DecisionTimer.Start(seconds);
	}
}