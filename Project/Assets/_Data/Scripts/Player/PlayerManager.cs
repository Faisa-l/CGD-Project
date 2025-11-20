using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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
    [SerializeField] UnityEvent<int> playerJoined;

    List<Transform> player_positions = new List<Transform>();
    int player_count = 0;
    [Space(20)]
    [Header("Player Debug Mode")]
    [SerializeField] bool debug_mode_on = false;
    [SerializeField] GameObject player_prefab;

    private int players = 0;
    [SerializeField] private Camera blankCamera;

    List<GameObject> inputs = new();
    [SerializeField] InputActionReference player_join_action;

    [SerializeField] MinimapPanel minimap;

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
                PlayerInput player = PlayerInput.Instantiate(player_prefab, i, splitScreenIndex: i);

                player.SwitchCurrentControlScheme(Gamepad.all[Mathf.Min(player_count, Gamepad.all.Count-1)]);
                player.gameObject.GetComponent<PlayerController>().setPlayerGamepad(Gamepad.all[Mathf.Min(player_count, Gamepad.all.Count - 1)]);

                player_count++;
                playerJoined.Invoke(player_count);
            }

            minimap.RepositionPanel(input_manager.maxPlayerCount);
        }

        blankCamera.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        GameObject temp;
        blankCamera.enabled = false;

        inputs.Add(input.gameObject);

        if(debug_mode_on)
        {
            input.GetComponent<CharacterController>().enabled = false;

            input.gameObject.transform.position = player_positions[input.playerIndex].position;
            input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

            input.GetComponent<CharacterController>().enabled = true;

            temp = Instantiate(forklift_prefab);

            temp.transform.position = input.gameObject.transform.position + new Vector3(spawn_offest.x, 0, spawn_offest.y);

            return;
        }

        players++;

        
        if (players == 3)
        {
            blankCamera.enabled = true;
        }

        minimap.RepositionPanel(players);

        InputDevice playerDevice = input.devices[0]; // the only device used for player is controller at index 0
        Gamepad playerGamepad = (Gamepad)InputSystem.GetDeviceById(playerDevice.deviceId); // cast the device as a gamepad using the associated id.

        input.SwitchCurrentControlScheme(playerGamepad);

        input.GetComponent<CharacterController>().enabled = false;

        input.gameObject.GetComponent<PlayerController>().setPlayerGamepad(playerGamepad);

        input.gameObject.transform.position = player_positions[input.playerIndex].position;
        input.gameObject.transform.rotation = player_positions[input.playerIndex].rotation;

        input.GetComponent<CharacterController>().enabled = true;

        temp = Instantiate(forklift_prefab);

        temp.transform.position = input.gameObject.transform.position + new Vector3(spawn_offest.x, 0, spawn_offest.y);

        player_count++;
        playerJoined.Invoke(player_count);
    }
}