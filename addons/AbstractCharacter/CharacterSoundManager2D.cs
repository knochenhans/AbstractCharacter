using System.Linq;
using System.Xml.XPath;
using Godot;
using Godot.Collections;

public class CharacterSoundManager2D
{
    private AbstractCharacter2D _character;

    public Dictionary<string, TimedAudioStreamPlayer2D> AudioPlayers { get; set; }

    public CharacterSoundManager2D(AbstractCharacter2D character)
    {
        _character = character;

        // var stateKeys = _stateSoundResources.Keys.ToArray();

        var audioPlayerPath = "res://addons/TimedAudioPlayer/TimedAudioStreamPlayer2D.tscn";
        Array<string> audioPlayerKeys = new() { "movement", "noticed" };

        var states = _character.CharacterResource.States;

        foreach (var state in states)
            audioPlayerKeys.Add(state.ID);

        AudioPlayers = new Dictionary<string, TimedAudioStreamPlayer2D>();

        foreach (var playerKey in audioPlayerKeys)
        {
            AudioPlayers[playerKey] = ResourceLoader.Load<PackedScene>(audioPlayerPath).Instantiate() as TimedAudioStreamPlayer2D;
            _character.AddChild(AudioPlayers[playerKey]);
        }

        SetupSounds();
    }

    public void SetupSounds()
    {
        AudioPlayers["movement"].AddSoundSetsFromRaw(_character.CharacterResource.MovementSounds);
        // IdleSound.AddSoundSet("idle", _character.CharacterResource.IdleSounds);
        // IdleSound.CurrentSoundSet = "idle";

        // HitSound.SetStreams(_character.CharacterResource.HitSounds);

        // var DeathAudioStreamRandomizer = new AudioStreamRandomizer();
        var NoticedAudioStreamRandomizer = new AudioStreamRandomizer();
        // var SpawnedAudioStreamRandomizer = new AudioStreamRandomizer();

        foreach (AudioStream audioStream in _character.CharacterResource.NoticeSounds)
            NoticedAudioStreamRandomizer.AddStream(-1, audioStream);

        AudioPlayers["noticed"].Stream = NoticedAudioStreamRandomizer;

        // foreach (AudioStream audioStream in _character.CharacterResource.SpawnSounds)
        //     SpawnedAudioStreamRandomizer.AddStream(-1, audioStream);

        // SpawnedSound.Stream = SpawnedAudioStreamRandomizer;

        // foreach (AudioStream audioStream in _character.CharacterResource.DeathSounds)
        // DeathAudioStreamRandomizer.AddStream(-1, audioStream);

        // DeathSound.Stream = DeathAudioStreamRandomizer;

        // Set pitch for all audio players and add random pitch +/-
        // var audioPlayers = new[]
        // {
        //     AudioPlayers["state"],
        //     AudioPlayers["movement"],
        //     AudioPlayers["noticed"],
        // };

        // foreach (var player in audioPlayers)
        //     player.PitchScale = _character.CharacterResource.Pitch + (float)GD.RandRange(-_character.CharacterResource.RandomPitch, _character.CharacterResource.RandomPitch);
    }

    public bool HasSound(string soundID)
    {
        var result = AudioPlayers.ContainsKey(soundID);

        if (!result)
            Logger.Log($"Sound ID {soundID} not found", Logger.LogTypeEnum.Error);

        return result;
    }

    public void PlaySound(string soundID)
    {
        if (HasSound(soundID))
            AudioPlayers[soundID].Play();
    }

    public void SetCurrentSoundSet(string soundID, string soundSet)
    {
        if (HasSound(soundID))
            AudioPlayers[soundID].CurrentSoundSet = soundSet;
    }

    public void SetActivityState(string soundID, TimedAudioStreamPlayer2D.ActivityStateEnum activityState)
    {
        if (HasSound(soundID))
            AudioPlayers[soundID].ActivityState = activityState;
    }

    public void StartLoop(string soundID)
    {
        if (HasSound(soundID))
            AudioPlayers[soundID].StartLoop();
    }

    public void StopLoop(string soundID)
    {
        if (HasSound(soundID))
            AudioPlayers[soundID].StopLoop();
    }
}