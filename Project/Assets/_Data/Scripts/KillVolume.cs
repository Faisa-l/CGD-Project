using UnityEngine;

public class KillVolume : MonoBehaviour
{
    [SerializeField]
    Transform respawn_pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Lift")
            return;

        if(other.tag == "TriggerPrompt")
            return ;

        Debug.Log(other.gameObject.name);
        if(other.tag == "Player")
        {
            var player_character = other.gameObject.transform;
            player_character.GetComponent<CharacterController>().enabled = false;
            player_character.transform.position = respawn_pos.position;
            player_character.GetComponent <CharacterController>().enabled = true;   
        }
        else if (other.tag == "Forklift")
        {
            var forklift = other.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
            forklift.GetComponent<CharacterController>().enabled = false;
            forklift.transform.position = respawn_pos.position;
            forklift.GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            other.transform.position = respawn_pos.position;
        }
    }
}
