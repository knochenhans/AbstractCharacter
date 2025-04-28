using Godot;

public partial class AbstractCharacterStateResource : Resource
{
    [Export] public string ID { get; set; }
    [Export] public float Duration { get; set; }

    [Export] public string DefaultNextState { get; set; }
    [Export] public TimedAudioStreamPlayerResource TimedAudioStreamPlayerResource { get; set; }

    [Export] public bool InputActive { get; set; } = true;
    [Export] public bool MovementActive { get; set; } = true;
    [Export] public bool DamageActive { get; set; } = true;
}