using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "new Tile object", menuName = "Tiles/Tile Object")]
public class TileObject : ScriptableObject
{
    public TileBase baseTile;

    public Sprite tileIcon;
}