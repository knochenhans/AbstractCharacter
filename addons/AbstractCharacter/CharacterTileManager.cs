using Godot;

public class CharacterTileManager
{
    private AbstractCharacter2D _character;

    public CharacterTileManager(AbstractCharacter2D character) => _character = character;

    public Vector2I TileSize { get; set; } = Vector2I.Zero;
    public Vector2I LastCheckedTile { get; set; } = Vector2I.Zero;

    public void SetCurrentTileData(TileData tileData)
    {
        if (tileData != null)
            _character.SoundManager.SetCurrentSoundSet("movement", tileData.GetCustomData("surface").ToString());
    }
}