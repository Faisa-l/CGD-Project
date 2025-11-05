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
    [NamedArray(new string[] { "Player 1 Camera", "Player 2 Camera", "Player 3 Camera", "Player 4 Camera", "OutOfRange" })]
    [SerializeField] List<Camera> cameras;

    [NamedArray(new string[] { "Player 1", "Player 2", "Player 3", "Player 4", "OutOfRange" })]
    [SerializeField] List<GameObject> players;

    [NamedArray(new string[] { "Forkflift 1", "Forkflift 2", "Forkflift 3", "Forkflift 4", "OutOfRange" })]
    [SerializeField] List<GameObject> forklifts;



    [SerializeField] Camera blank_camera;

    Rect screen_full = new Rect(0, 0, 1, 1);

    Rect screen_left = new Rect(0, 0, 0.5f, 1);
    Rect screen_right = new Rect(0.5f, 0, 0.5f, 1);

    Rect screen_top_left = new Rect(0, 0.5f, 0.5f, 0.5f);
    Rect screen_top_right = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 1; i <= 4; i++)
        {
            changePerspective1st(i);
        }

        //set the default setup to have the single-player layout
        disableExtraPlayers();
        cameras[0].rect = screen_full;
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
            players_shown %= cameras.Count;

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
            cameras[0].rect = screen_full;
        }
        else if (number_of_players == 1)
        {
            cameras[0].rect = screen_left;

            activatePlayer(1);
            cameras[1].rect = screen_right;
        }
        else if (number_of_players == 2)
        {
            cameras[0].rect = screen_top_left;

            activatePlayer(1);
            cameras[1].rect = screen_top_right;

            activatePlayer(2);

            //The blank camera allows the bottom right corner to be a solid colour when there's only 3 players
            //(There might be a better solution, this is currently for proof of concept)
            blank_camera.gameObject.SetActive(true);
        }
        else if (number_of_players == 3)
        {
            cameras[0].rect = screen_top_left;

            activatePlayer(1);
            cameras[1].rect = screen_top_right;

            activatePlayer(2);

            activatePlayer(3);
        }
    }

    private void activatePlayer(int player_num)
    {
        cameras[player_num].gameObject.SetActive(true);
        players[player_num].SetActive(true);
        forklifts[player_num].SetActive(true);
    }

    private void deactivatePlayer(int player_num)
    {
        cameras[player_num].gameObject.SetActive(false);
        players[player_num].SetActive(false);
        forklifts[player_num].SetActive(false);
    }

    private void disableExtraPlayers()
    {
        //Disabling players 2 to 4
        for (int i = 1; i < cameras.Count; i++)
        {
            cameras[i].gameObject.SetActive(false);
            players[i].gameObject.SetActive(false);
            forklifts[i].gameObject.SetActive(false);
        }
    }

    public void changePerspective3rd(int player_number)
    {
        Debug.Log(player_number);

        //deactivate the player
        players[player_number - 1].SetActive(false);

        Transform camera_transform = forkliftCameraRoot(player_number);
        cameras[player_number - 1].transform.parent = camera_transform;
        cameras[player_number - 1].transform.position = camera_transform.position;
        cameras[player_number - 1].transform.rotation = camera_transform.rotation;
    }

    public void changePerspective1st(int player_number)
    {
        //activate the player
        players[player_number - 1].SetActive(true);

        //set the camera position to the camera root of the player
        Transform camera_transform = playerCameraRoot(player_number);
        cameras[player_number - 1].transform.parent = camera_transform;
        cameras[player_number - 1].transform.position = camera_transform.position;
        cameras[player_number - 1].transform.rotation = camera_transform.rotation;
    }

    private Transform forkliftCameraRoot(int forklift_num)
    {
        return forklifts[forklift_num - 1].transform.Find("ForkliftCameraRoot").transform;
    }

    private Transform playerCameraRoot(int player_num)
    {
        return players[player_num - 1].transform.Find("PlayerCapsule").transform.Find("PlayerCameraRoot").transform;
    }
}
