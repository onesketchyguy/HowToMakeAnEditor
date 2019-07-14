using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditorUI : MonoBehaviour
{
    public static bool notPlacing;

    public void TogglePlacing()
    {
        notPlacing = !Cursor.visible;

        if (notPlacing)
        {
            UpdateLoadableList();

            DestroyAllChildren(TilePalleteParent.transform);

            foreach (var item in tiles)
            {
                CreateObjectFromTile(item);
            }
        }
    }

    private void CreateObjectFromTile(TileObject tile)
    {
        GameObject go = new GameObject(tile.name);
        var obj = Instantiate(go, TilePalleteParent.transform) as GameObject;
        Destroy(go);

        var image = obj.AddComponent<Image>();
        image.sprite = tile.tileIcon;
        var button = obj.AddComponent<Button>();

        button.targetGraphic = image;
        button.onClick.AddListener(() => SwapTile(tile));
    }

    private void DestroyAllChildren(Transform parent)
    {
        foreach (var item in parent.GetComponentsInChildren<Transform>())
        {
            if (item == parent) continue;

            Destroy(item.gameObject);
        }
    }

    private void SwapTile(TileObject tile)
    {
        FindObjectOfType<TileManager>().PlacingTile = tile.baseTile;
    }

    private void UpdateLoadableList()
    {
        var options = new List<Dropdown.OptionData>();
        var Saves = SaveManager.LoadAllSaves();

        foreach (var item in Saves)
        {
            var option = new Dropdown.OptionData(item);

            options.Add(option);
        }

        savesDropdown.ClearOptions();
        savesDropdown.AddOptions(options);
    }

    public Dropdown savesDropdown;
    public InputField SaveInput;

    public GameObject parent;

    [Space]
    public TileObject[] tiles;

    public GameObject TilePalleteParent;

    private void Start()
    {
        savesDropdown.onValueChanged.AddListener(LoadSelectedLevel);
    }

    private void Update()
    {
        Cursor.visible = notPlacing;

        if (Input.GetButtonDown("Cancel")) TogglePlacing();

        parent.SetActive(notPlacing);
    }

    private void LoadSelectedLevel(int index)
    {
        var name = savesDropdown.options[index].text;

        FindObjectOfType<TileManager>().Load(name);
    }

    public void SaveLevel()
    {
        var name = SaveInput.text;

        if (name == null || name.ToCharArray().Length <= 1)
        {
            Debug.LogError("Name too short!");

            return;
        }

        FindObjectOfType<TileManager>().Save(name);

        UpdateLoadableList();
    }
}