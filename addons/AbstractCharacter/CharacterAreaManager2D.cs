using Godot;

public class CharacterAreaManager2D
{
    private AbstractCharacter2D _character;

    public Area2D ScanArea => _character.GetNode<Area2D>("ScanArea");
    public Area2D PickupArea => _character.GetNode<Area2D>("PickupArea");

    public CharacterAreaManager2D(AbstractCharacter2D character)
    {
        _character = character;

        Area2D scanArea = _character.GetNode<Area2D>("ScanArea");
        scanArea.BodyEntered += (body) => _character.ControllerManager.OnScanArea2DBodyEntered(body);
        scanArea.BodyExited += (body) => _character.ControllerManager.OnScanArea2DBodyExited(body);

        var scanAreaShape = scanArea.GetNode<CollisionShape2D>("CollisionShape2D");
        (scanAreaShape.Shape as CircleShape2D).Radius = _character.CharacterResource.ScanRadius;

        PickupArea.AreaEntered += OnPickupAreaAreaEntered;
    }

    public void OnPickupAreaAreaEntered(Area2D area) => _character.OnPickupAreaAreaEntered(area);
}