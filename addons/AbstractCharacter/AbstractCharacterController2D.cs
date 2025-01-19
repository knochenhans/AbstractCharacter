using Godot;
using System;

public partial class AbstractCharacterController2D : Node2D
{
    [Signal] public delegate void CharacterNoticedEventHandler(Character character);
    
    public enum ActivityStateEnum
    {
        Active,
        Inactive
    }

    public ActivityStateEnum ActivityState { get; set; } = ActivityStateEnum.Active;

    public AbstractCharacter2D ControlledCharacter { get; set; }

    public virtual void OnWeaponRangeEntered(Node2D body) { }
    public virtual void OnWeaponRangeExited(Node2D body) { }

    public virtual void OnScanArea2DBodyEntered(Node body) { }
    public virtual void OnScanArea2DBodyExited(Node body) { }
}
