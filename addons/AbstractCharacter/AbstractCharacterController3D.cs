using Godot;
using System;

public partial class AbstractCharacterController3D : Node3D
{
    [Signal] public delegate void CharacterNoticedEventHandler(Character character);
    
    public enum ActivityStateEnum
    {
        Active,
        Inactive
    }

    public ActivityStateEnum ActivityState { get; set; } = ActivityStateEnum.Active;

    public AbstractCharacter3D ControlledCharacter { get; set; }

    public virtual void OnWeaponRangeEntered(Node3D body) { }
    public virtual void OnWeaponRangeExited(Node3D body) { }

    public virtual void OnScanArea3DBodyEntered(Node body) { }
    public virtual void OnScanArea3DBodyExited(Node body) { }
}
