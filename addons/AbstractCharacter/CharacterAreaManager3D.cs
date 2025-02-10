using Godot;

public class CharacterAreaManager3D
{
    private AbstractCharacter3D _character;

    public Area3D ScanArea => _character.GetNode<Area3D>("ScanArea");
    public Area3D PickupArea => _character.GetNode<Area3D>("PickupArea");

    public CharacterAreaManager3D(AbstractCharacter3D character)
    {
        _character = character;

        Area3D scanArea = _character.GetNode<Area3D>("ScanArea");
        scanArea.BodyEntered += (body) => _character.ControllerManager.OnScanArea3DBodyEntered(body);
        scanArea.BodyExited += (body) => _character.ControllerManager.OnScanArea3DBodyExited(body);

        var scanAreaShape = scanArea.GetNode<CollisionShape3D>("CollisionShape3D");
        (scanAreaShape.Shape as SphereShape3D).Radius = _character.CharacterResource.ScanRadius;

        PickupArea.AreaEntered += OnPickupAreaAreaEntered;
    }

    public void OnPickupAreaAreaEntered(Area3D area) => _character.OnPickupAreaAreaEntered(area);
}