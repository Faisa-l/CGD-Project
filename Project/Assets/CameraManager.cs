using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        try
        {
            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            EditorGUI.ObjectField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]));
        }
        catch
        {
            EditorGUI.ObjectField(rect, property, label);
        }
    }
}

public class CameraManager : MonoBehaviour
{
    [NamedArray(new string[] { "Player 1", "Player 2", "Player 3", "Player 4", "OutOfRange" })]
    [SerializeField] List<GameObject> players;


    [SerializeField] Camera blank_camera;

    Rect screen_full = new Rect(0, 0, 1, 1);

    Rect screen_left = new Rect(0, 0, 0.5f, 1);
    Rect screen_right = new Rect(0.5f, 0, 0.5f, 1);

    Rect screen_top_left = new Rect(0, 0.5f, 0.5f, 0.5f);
    Rect screen_top_right = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //set the default setup to have the single-player layout
        disableExtraPlayers();
        players[0].GetComponentInChildren<Camera>().rect = screen_full;
    }

    // Update is called once per frame, this can be removed later
    [SerializeField] KeyCode key_switch;
    int players_shown = 0;
    void Update()
    {
        //Interfacing with the number of players using a key press
        if (Input.GetKeyDown(key_switch))
        {
            players_shown++;
            players_shown %= players.Count;

            setNumberOfPlayers(players_shown);
        }

        //This can be changed to use an input from a selection UI
    }

    public void setNumberOfPlayers(int number_of_players)
    {
        blank_camera.gameObject.SetActive(false);

        //Disabling players 2 to 4 as a precaution 
        disableExtraPlayers();

        //Player 1 is always enabled so we don't call the enable function for them
        //Players 3 and 4 always have the same screen size and so we don't need to change their viewport size

        //Hard-coded window view manipulation
        if (number_of_players == 0)
        {
            players[0].GetComponentInChildren<Camera>().rect = screen_full;
        }
        else if (number_of_players == 1)
        {
            players[0].GetComponentInChildren<Camera>().rect = screen_left;

            players[1].SetActive(true);
            players[1].GetComponentInChildren<Camera>().rect = screen_right;
        }
        else if (number_of_players == 2)
        {
            players[0].GetComponentInChildren<Camera>().rect = screen_top_left;

            players[1].SetActive(true);
            players[1].GetComponentInChildren<Camera>().rect = screen_top_right;

            players[2].SetActive(true);

            //The blank camera allows the bottom right corner to be a solid colour when there's only 3 players
            //(There might be a better solution, this is currently for proof of concept)
            blank_camera.gameObject.SetActive(true);
        }
        else if (number_of_players == 3)
        {
            players[0].GetComponentInChildren<Camera>().rect = screen_top_left;

            players[1].SetActive(true);
            players[1].GetComponentInChildren<Camera>().rect = screen_top_right;

            players[2].SetActive(true);
            players[3].SetActive(true);
        }
    }

    private void disableExtraPlayers()
    {
        //Disabling players 2 to 4
        for (int i = 1; i < players.Count; i++)
        {
            players[i].SetActive(false);
        }
    }
}
