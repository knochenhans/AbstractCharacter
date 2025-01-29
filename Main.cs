using Godot;
using System;

public partial class Main : Node2D
{
	AbstractCharacter2D AbstractCharacter => GetNode<AbstractCharacter2D>("CharacterBody2D");
	Timer HealthTimer => GetNode<Timer>("HealthTimer");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect the HealthTimer's timeout signal to the Main script
		HealthTimer.Timeout += () => ReduceHealth();
		AbstractCharacter.Died += (_) => RemoveCharacter(AbstractCharacter);
	}

	public void ReduceHealth()
	{
		Logger.Log($"Reducing health of {AbstractCharacter.Name} by 1");
		AbstractCharacter.Hit(1);
	}

	public void RemoveCharacter(AbstractCharacter2D character)
	{
		Logger.Log($"Removing {character.Name}");
		HealthTimer.Stop();
		// character.QueueFree();
	}
}
