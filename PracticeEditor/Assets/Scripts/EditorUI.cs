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
    }

    public Dropdown savesDropdown;
    public InputField SaveInput;

    public GameObject parent;

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
    }
}