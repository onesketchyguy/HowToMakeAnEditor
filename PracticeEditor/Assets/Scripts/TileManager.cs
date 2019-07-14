using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap FrontTilemap, HighLightTilemap;

    public TileBase HighLightTile;

    public TileBase PlacingTile;

    public TileBase[] possibleTiles;
    private Dictionary<string, TileBase> tiles = new Dictionary<string, TileBase>();

    private void PopulateDictionary()
    {
        foreach (var item in possibleTiles)
        {
            tiles.Add(item.name, item);
        }

        PlacingTile = possibleTiles[0];
    }

    private Vector3Int lastMousePos;

    private Vector3Int mousePosition
    {
        get
        {
            var mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            return new Vector3Int((int)mousePos.x, (int)mousePos.y, (int)mousePos.z);
        }
    }

    public Vector2 saveRange = new Vector2(18, 10);

    private void Start() => PopulateDictionary();

    private void Update()
    {
        if (Cursor.visible == true)
        {
            if (HighLightTilemap.GetTile(lastMousePos) != null)
                HighLightTilemap.SetTile(lastMousePos, null);

            return;
        }

        if (mousePosition != lastMousePos)
        {
            HighLightTilemap.SetTile(lastMousePos, null);

            HighLightTilemap.SetTile(mousePosition, HighLightTile);

            lastMousePos = mousePosition;
        }

        // Check for left click
        if (Input.GetButton("Fire1") && FrontTilemap.GetTile(mousePosition) != PlacingTile)
        {
            FrontTilemap.SetTile(mousePosition, PlacingTile);
        }

        // Check for right click
        if (Input.GetButton("Fire2") && FrontTilemap.GetTile(mousePosition) != null)
        {
            FrontTilemap.SetTile(mousePosition, null);
        }
    }

    public void Save(string saveName = "0")
    {
        var Room = new SaveManager.SavedTiles();

        var TilesToSave = new List<Vector2Int>();
        var SquaresToSave = new List<Vector2Int>();

        var worldPosition = new Vector2(saveRange.x / 2, saveRange.y / 2);

        for (int y = 0; y < saveRange.y; y++)
        {
            for (int x = 0; x < saveRange.x; x++)
            {
                var pos = new Vector3Int((int)(x - worldPosition.x), (int)(y - worldPosition.y), 0);

                var Tile = FrontTilemap.GetTile(pos);

                if (Tile != null)
                {
                    if (Tile.name == "Square")
                    {
                        SquaresToSave.Add(new Vector2Int(pos.x, pos.y));
                    }
                    else
                    {
                        TilesToSave.Add(new Vector2Int(pos.x, pos.y));
                    }
                }
            }
        }

        Room.tiles = TilesToSave.ToArray();
        Room.squares = SquaresToSave.ToArray();

        SaveManager.Save(Room, saveName);

        Debug.Log("Saving world...");
    }

    public void Load(string saveName = "0")
    {
        var loadedTiles = SaveManager.Load(saveName);

        if (loadedTiles == null || loadedTiles.tiles == null) return;

        FrontTilemap.ClearAllTiles();

        var Tile = PlacingTile;
        var Square = PlacingTile;

        tiles.TryGetValue("Tile", out Tile);
        tiles.TryGetValue("Square", out Square);

        foreach (var tile in loadedTiles.tiles)
        {
            var pos = new Vector3Int((int)tile.x, (int)tile.y, 0);

            FrontTilemap.SetTile(pos, Tile);
        }
        foreach (var tile in loadedTiles.squares)
        {
            var pos = new Vector3Int((int)tile.x, (int)tile.y, 0);

            FrontTilemap.SetTile(pos, Square);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, saveRange);
    }
}