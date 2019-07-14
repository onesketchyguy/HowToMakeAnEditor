using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Tilemap FrontTilemap, HighLightTilemap;

    public TileBase HighLightTile;

    public TileBase GroundTile;

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
        if (Input.GetButton("Fire1") && FrontTilemap.GetTile(mousePosition) != GroundTile)
        {
            FrontTilemap.SetTile(mousePosition, GroundTile);
        }

        // Check for right click
        if (Input.GetButton("Fire2") && FrontTilemap.GetTile(mousePosition) != null)
        {
            FrontTilemap.SetTile(mousePosition, null);
        }
    }

    public void Save(string saveName = "0")
    {
        var TilesToSave = new List<Vector2>();
        var tiles = new SaveManager.SavedTiles();

        var worldPosition = new Vector2(saveRange.x / 2, saveRange.y / 2);

        for (int y = 0; y < saveRange.y; y++)
        {
            for (int x = 0; x < saveRange.x; x++)
            {
                var pos = new Vector3Int((int)(x - worldPosition.x), (int)(y - worldPosition.y), 0);

                var Tile = FrontTilemap.GetTile(pos);

                if (Tile != null)
                {
                    TilesToSave.Add(new Vector2(pos.x, pos.y));
                }
            }
        }

        tiles.tiles = TilesToSave.ToArray();

        SaveManager.Save(tiles, saveName);

        Debug.Log("Saving world...");
    }

    public void Load(string saveName = "0")
    {
        var loadedTiles = SaveManager.Load(saveName);

        if (loadedTiles == null || loadedTiles.tiles == null) return;

        FrontTilemap.ClearAllTiles();

        foreach (var tile in loadedTiles.tiles)
        {
            var pos = new Vector3Int((int)tile.x, (int)tile.y, 0);

            FrontTilemap.SetTile(pos, GroundTile);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(transform.position, saveRange);
    }
}