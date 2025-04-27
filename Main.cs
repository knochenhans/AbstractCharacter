using Godot;
using System;

public partial class Main : Node2D
{
	AbstractCharacterBody2D AbstractCharacter => GetNode<AbstractCharacterBody2D>("CharacterBody2D");
	Timer HealthTimer => GetNode<Timer>("DamageTimer");

	public override void _Ready()
	{
		HealthTimer.Timeout += () => ReduceHealth();
		AbstractCharacter.Character.Died += () => OnCharacterDead(AbstractCharacter);
	}

	public void ReduceHealth()
	{
		Logger.Log($"Reducing health of {AbstractCharacter.Name} by 2");
		AbstractCharacter.Character.Hit(2);
	}

	public void OnCharacterDead(AbstractCharacterBody2D character)
	{
		Logger.Log($"{character.Name} is dead");
		HealthTimer.Stop();
		// character.QueueFree();
	}
}
