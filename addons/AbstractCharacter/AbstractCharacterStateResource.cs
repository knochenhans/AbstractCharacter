using Godot;

public partial class AbstractCharacterStateResource : Resource
{
    [Export] public string ID { get; set; }
    [Export] public float Duration { get; set; }

    [Export] public string DefaultNextState { get; set; }
    [Export] public TimedAudioStreamPlayerResource TimedAudioStreamPlayerResource { get; set; }
}