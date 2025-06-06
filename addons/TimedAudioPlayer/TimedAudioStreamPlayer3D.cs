using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class TimedAudioStreamPlayer3D : AudioStreamPlayer3D
{
	[Export] public TimedAudioStreamPlayerResource TimedAudioStreamPlayerResource { get; set; }

	public enum ActivityStateEnum
	{
		Active,
		Inactive
	}

	public ActivityStateEnum ActivityState
	{
		get => activityState; set
		{
			activityState = value;

			if (activityState == ActivityStateEnum.Inactive)
				Timer.Paused = true;
			else
				Timer.Paused = false;
		}
	}
	Dictionary<string, Array<AudioStream>> soundSets = [];

	bool _isLooping = false;

	string currentSoundSet = "default";
	public string CurrentSoundSet
	{
		get => currentSoundSet;
		set
		{
			if (value != currentSoundSet)
			{
				currentSoundSet = value;
				UpdateSoundSet(currentSoundSet);
			}
		}
	}

	Timer Timer => GetNode<Timer>("Timer");

	AudioStreamRandomizer queuedStream = null;
	private ActivityStateEnum activityState = ActivityStateEnum.Active;

	public override void _Ready()
	{
		Finished += OnFinished;
		Timer.Timeout += OnTimeout;

		if (TimedAudioStreamPlayerResource != null)
		{
			if (TimedAudioStreamPlayerResource.SoundSets.Count > 0)
				if (TimedAudioStreamPlayerResource.SoundSets.ContainsKey(CurrentSoundSet))
					if (TimedAudioStreamPlayerResource.SoundSets[CurrentSoundSet].Count > 0)
						SetStreams(TimedAudioStreamPlayerResource.SoundSets[CurrentSoundSet]);

			if (TimedAudioStreamPlayerResource.Autoplay)
				StartLoop();
		}
	}

	public void SetRandomTime()
	{
		if (TimedAudioStreamPlayerResource != null)
			Timer.WaitTime = new Random().NextDouble() * TimedAudioStreamPlayerResource.RandomWaitTime + TimedAudioStreamPlayerResource.MinWaitTime;
	}

	public void OnTimeout()
	{
		if (_isLooping)
			Play();
	}

	public void OnFinished()
	{
		if (TimedAudioStreamPlayerResource.RandomWaitTime == 0 && TimedAudioStreamPlayerResource.MinWaitTime == 0)
		{
			if (_isLooping)
				Play();
		}
		else
		{
			SetRandomTime();
			Timer.Start();
		}
	}

	public void StartLoop()
	{
		_isLooping = true;
		if (TimedAudioStreamPlayerResource != null)
		{
			if (TimedAudioStreamPlayerResource.PlayOnLoopStart)
				Play();
		}
		else
		{
			SetRandomTime();
			Timer.Start();
		}
	}

	public void StopLoop()
	{
		_isLooping = false;
	}

	public void SetStreams(Array<AudioStream> streams)
	{
		var randomizer = new AudioStreamRandomizer();
		foreach (var stream in streams)
			randomizer.AddStream(-1, stream);

		if (TimedAudioStreamPlayerResource != null)
		{
			PitchScale = TimedAudioStreamPlayerResource.Pitch;
			randomizer.RandomPitch = TimedAudioStreamPlayerResource.RandomPitchAdded;
			randomizer.RandomVolumeOffsetDb = TimedAudioStreamPlayerResource.RandomVolumeAdded;
		}

		queuedStream = randomizer;
	}

	public void UpdateSoundSet(string soundSet)
	{
		if (soundSets.ContainsKey(soundSet))
			SetStreams(soundSets[CurrentSoundSet]);
		else
			GD.PrintErr($"Sound set {soundSet} not found in local soundSets");
	}

	public void AddSoundSet(string soundSet, Array<AudioStream> streams, bool replace = false)
	{
		if (soundSets.ContainsKey(soundSet))
		{
			if (replace)
				soundSets[soundSet] = streams;
			else
				foreach (var stream in streams)
					soundSets[soundSet].Add(stream);
		}
		else
			soundSets.Add(soundSet, streams);
	}

	public void AddSoundSetsFromRaw(Array<AudioStream> streams, bool replace = false)
	{
		Dictionary<string, Array<AudioStream>> tempSoundSets = [];

		foreach (var stream in streams)
		{
			var prefix = stream.ResourcePath.Split("/").Last<string>().Split(".").First<string>().Split("_").First<string>();

			if (!tempSoundSets.ContainsKey(prefix))
				tempSoundSets[prefix] = [];

			GD.Print($"Adding sound set \"{prefix}\" to local soundSets");
			tempSoundSets[prefix].Add(stream);
		}

		foreach (var soundSet in tempSoundSets)
			AddSoundSet(soundSet.Key, soundSet.Value, replace);
	}

	public void Play()
	{
		if (queuedStream != null)
		{
			Stream = queuedStream;
			queuedStream = null;
		}

		if (!Playing)
			base.Play();
	}
}
