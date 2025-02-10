using Godot;
using System;

public partial class NonPlayerCharacterControllerResource : CharacterControllerResource
{
    public enum EmotionalStateEnum
    {
        Explorative,
        Neutral,
        Defensive,
        Aggressive,
        Fearful
    }

    [Export] public EmotionalStateEnum InitialEmotionalState { get; set; } = EmotionalStateEnum.Neutral;
    [Export] public int WanderDistance { get; set; } = 100;
    [Export] public float DecisionInterval { get; set; } = 1.0f;
    [Export] public float AttackRange { get; set; } = 100.0f;
}