using Godot;
using System;

public partial class CharacterControllerResource : Resource
{
    public enum EmotionalStateEnum
	{
		Neutral,
		Fearful,
		Explorative
	}

    [Export] public float DecisionInterval { get; set; } = 1;
    [Export] public int WanderDistance { get; set; } = 30;
    [Export] public EmotionalStateEnum InitialEmotionalState { get; set; } = EmotionalStateEnum.Neutral;
}