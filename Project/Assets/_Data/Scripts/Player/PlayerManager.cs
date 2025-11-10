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

    List<PlayerInput> players = new();

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlayerJoined(PlayerInput input)
    {
        if(debug_mode_on)
        {
            input.GetComponent<CharacterController>().enabled = false;

            input.gameObject.GetComponent<PlayerController>().setPlayerNumber(input.playerIndex + 1);

            input.gameObject.transform.position = player_positions[input.playerIndex].position;
            input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

            GameObject temp = Instantiate(forklift_prefab);

            temp.transform.position = input.gameObject.transform.position + new Vector3(spawn_offest.x, 0, spawn_offest.y);

            input.GetComponent<CharacterController>().enabled = true;
        }
        else
        {            
            input.SwitchCurrentControlScheme(Gamepad.all[input.devices[0].deviceId - Gamepad.all[0].deviceId]);

            input.GetComponent<CharacterController>().enabled = false;

            input.gameObject.GetComponent<PlayerController>().setPlayerNumber(input.devices[0].deviceId - Gamepad.all[0].deviceId + 1);

            input.gameObject.transform.position = player_positions[input.playerIndex].position;
            input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

            GameObject temp = Instantiate(forklift_prefab);

            temp.transform.position = input.gameObject.transform.position + new Vector3(spawn_offest.x, 0, spawn_offest.y);

            input.GetComponent<CharacterController>().enabled = true;
        }
    }
}