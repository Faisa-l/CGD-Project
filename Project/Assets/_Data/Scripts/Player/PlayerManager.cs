using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Windows;

public class PlayerManager : MonoBehaviour
{
    [Space(20)]
    [Header("Player Spawn Points")]
    [SerializeField] Transform player_1_transform;
    [SerializeField] Transform player_2_transform;
    [SerializeField] Transform player_3_transform;
    [SerializeField] Transform player_4_transform;

    [Space(20)]
    [Header("Forklift Spawning")]
    [SerializeField] GameObject forklift_prefab;
    [SerializeField] Vector2 spawn_offest;

    [Space(20)]
    [Header("Player Debug Mode")]
    [SerializeField] bool debug_mode_on = false;
    [SerializeField] GameObject player_prefab;

    List<Transform> player_positions = new();

    private int players = 0;
    [SerializeField] private Camera blankCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player_positions.Add(player_1_transform);
        player_positions.Add(player_2_transform);
        player_positions.Add(player_3_transform);
        player_positions.Add(player_4_transform);

        if (debug_mode_on)
        {
            PlayerInputManager input_manager = GetComponent<PlayerInputManager>();

            input_manager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            for (int i = 0; i < input_manager.maxPlayerCount; i++)
            {
                PlayerInput.Instantiate(player_prefab, i, splitScreenIndex: i);
            }
        }

        blankCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayerJoined(PlayerInput input)
    {
        players++;

        blankCamera.enabled = false;
        if (players == 3)
        {
            blankCamera.enabled = true;
        }

        if (debug_mode_on)
        {
            input.GetComponent<CharacterController>().enabled = false;

            input.gameObject.transform.position = player_positions[input.playerIndex].position;
            input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

            input.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            InputDevice playerDevice = input.devices[0]; // the only device used for player is controller at index 0
            Gamepad playerGamepad = (Gamepad)InputSystem.GetDeviceById(playerDevice.deviceId); // cast the device as a gamepad using the associated id.

            input.SwitchCurrentControlScheme(playerGamepad);

            input.GetComponent<CharacterController>().enabled = false;

            input.gameObject.GetComponent<PlayerController>().setPlayerGamepad(playerGamepad);

            input.gameObject.transform.position = player_positions[input.playerIndex].position;
            input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

            input.GetComponent<CharacterController>().enabled = true;
        }
        GameObject temp = Instantiate(forklift_prefab);

        temp.transform.position = input.gameObject.transform.position + new Vector3(spawn_offest.x, 0, spawn_offest.y);

    }
}