using Godot;

public class CharacterControllerManager3D
{
    private AbstractCharacter3D _character;

    public AbstractCharacterController3D CharacterController { get; set; }

    public CharacterControllerManager3D(AbstractCharacter3D character)
    {
        _character = character;

        CharacterController = _character.CharacterControllerScene.Instantiate() as AbstractCharacterController3D;
        CharacterController.ControlledCharacter = _character;
        // CharacterController.CharacterNoticed += OnCharacterControllerCharacterNoticed;
        _character.AddChild(CharacterController);
    }

    // public void OnCharacterControllerCharacterNoticed(AbstractCharacter2D player) => _character.OnCharacterControllerCharacterNoticed(player);
    public void OnScanArea3DBodyEntered(Node3D body) => CharacterController.OnScanArea3DBodyEntered(body);
    public void OnScanArea3DBodyExited(Node3D body) => CharacterController.OnScanArea3DBodyExited(body);
}
