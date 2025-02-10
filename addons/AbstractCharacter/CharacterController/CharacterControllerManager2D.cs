using Godot;

public class CharacterControllerManager2D
{
    private AbstractCharacter2D _character;

    public AbstractCharacterController2D CharacterController { get; set; }

    public CharacterControllerManager2D(AbstractCharacter2D character)
    {
        _character = character;

        CharacterController = _character.CharacterControllerScene.Instantiate() as AbstractCharacterController2D;
        CharacterController.ControlledCharacter = _character;
        // CharacterController.CharacterNoticed += OnCharacterControllerCharacterNoticed;
        _character.AddChild(CharacterController);
    }

    // public void OnCharacterControllerCharacterNoticed(AbstractCharacter2D player) => _character.OnCharacterControllerCharacterNoticed(player);
    public void OnScanArea2DBodyEntered(Node2D body) => CharacterController.OnScanArea2DBodyEntered(body);
    public void OnScanArea2DBodyExited(Node2D body) => CharacterController.OnScanArea2DBodyExited(body);
}
