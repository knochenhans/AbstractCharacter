using System;
using Godot;
using Godot.Collections;

public partial class CharacterState : GodotObject
{
    public string ID { get; set; }
    public float Duration { get; set; }

    public string DefaultNextState { get; set; }
    public TimedAudioStreamPlayerResource TimedAudioStreamPlayerResource { get; set; }
}

public partial class CharacterStateManager : GodotObject
{
    [Signal] public delegate void LifeStateChangedEventHandler(CharacterState newState);

    private ICharacter _character;

    public CharacterStateManager(ICharacter character)
    {
        _character = character;
        SetupStates();
    }

    private void SetupStates()
    {
        foreach (var stateResource in _character.CharacterResource.States)
        {
            LifeStates.Add(
                new CharacterState
                {
                    ID = stateResource.ID,
                    Duration = stateResource.Duration,
                    DefaultNextState = stateResource.DefaultNextState,
                    TimedAudioStreamPlayerResource = stateResource.TimedAudioStreamPlayerResource
                });
        }
    }

    public enum ActivityStateEnum { Active, Inactive }
    public enum MovementStateEnum { Idle, Moving }
    public Array<CharacterState> LifeStates { get; set; } = new Array<CharacterState>();

    private ActivityStateEnum _activityState = ActivityStateEnum.Active;
    private MovementStateEnum _movementState = MovementStateEnum.Idle;
    private CharacterState _lifeState;

    public ActivityStateEnum ActivityState
    {
        get => _activityState;
        set
        {
            if (_activityState != value)
            {
                _activityState = value;
                OnActivityStateChange(value);
            }
        }
    }

    public MovementStateEnum MovementState
    {
        get => _movementState;
        set
        {
            if (_movementState != value)
            {
                _movementState = value;
                OnMovementStateChange(value);
            }
        }
    }

    public CharacterState LifeState
    {
        get => _lifeState;
        set
        {
            if (_lifeState != value)
            {
                var _oldState = _lifeState;
                _lifeState = value;
                OnLifeStateChange(_oldState, value);
            }
        }
    }

    public void SetLifeState(string stateID)
    {
        foreach (var state in LifeStates)
        {
            if (state.ID == stateID)
            {
                LifeState = state;
                return;
            }
        }
    }

    protected virtual void OnActivityStateChange(ActivityStateEnum newState)
    {
        // _character.SoundManager.SetActivityState("hit", newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive);
        // _character.SoundManager.SetActivityState("idle", newState == ActivityStateEnum.Active ? TimedAudioStreamPlayer2D.ActivityStateEnum.Active : TimedAudioStreamPlayer2D.ActivityStateEnum.Inactive);
    }

    protected virtual void OnMovementStateChange(MovementStateEnum newState)
    {
        // switch (newState)
        // {
        //     case MovementStateEnum.Idle:
        //         // _currentAnimationPrefix = CharacterResource.IdlePrefix;
        //         _character.SoundManager.StopLoop("movement");
        //         break;
        //     case MovementStateEnum.Moving:
        //         // _currentAnimationPrefix = CharacterResource.MovementPrefix;
        //         _character.EmitSignal(AbstractCharacter2D.SignalName.RequestCurrentTileData, _character);
        //         _character.SoundManager.StartLoop("movement");
        //         break;
        // }
    }

    protected virtual void OnLifeStateChange(CharacterState oldState, CharacterState newState)
    {
        // _character.OnLifeStateChange(newState);

        // Wait for the duration of the state before transitioning to the next state, if there is one
        if (newState.Duration > 0)
            _character.LifeStateTimer.Start(newState.Duration);
        // else
        //     _character.LifeStateTimer.Stop();
        EmitSignal(SignalName.LifeStateChanged, newState);
    }

    public void TransitionToNextState()
    {
        if (LifeState.DefaultNextState != null)
            SetLifeState(LifeState.DefaultNextState);
    }
}