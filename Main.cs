using Godot;
using System;

public partial class Main : Node2D
{
	AbstractCharacter2D AbstractCharacter => GetNode<AbstractCharacter2D>("CharacterBody2D");
	Timer HealthTimer => GetNode<Timer>("DamageTimer");

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect the HealthTimer's timeout signal to the Main script
		HealthTimer.Timeout += () => ReduceHealth();
		AbstractCharacter.Died += (_) => OnCharacterDead(AbstractCharacter);
	}

	public void ReduceHealth()
	{
		Logger.Log($"Reducing health of {AbstractCharacter.Name} by 2");
		AbstractCharacter.Hit(2);
	}

	public void OnCharacterDead(AbstractCharacter2D character)
	{
		Logger.Log($"{character.Name} is dead");
		HealthTimer.Stop();
		// character.QueueFree();
	}
}
