using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
	public IGameState currentState { get; private set; }
	
	// Source - https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
	public PreludeState preludeState = new PreludeState(); // E.g. tutorial room timer not started
    public PlayingState playingState = new PlayingState();
    public VictoryState victoryState = new VictoryState();
    public GameOverState gameOverState = new GameOverState();
	
	// Singleton
	public static GameManager instance { get; private set; }
	
	private void Awake()
	{
		// Singleton
		instance = this;
	}
	
	#region State Machine
	
	private void Start()
	{
		// Subscribe to events
		GameOverButtonInteractable.onPressed += SetGameOverState;
		
		Time.timeScale = 1.0f; // Reset from any previous playthroughs
		ChangeState(playingState);
	}
	
	private void Update()
	{
		if (currentState != null)
			currentState.UpdateState(this);
		
		if (Input.GetKeyDown(KeyCode.L))
			ChangeState(gameOverState);
	}
	
	private void ChangeState(IGameState newState)
	{
		if (currentState != null)
			currentState.OnExit(this);
		
		currentState = newState;
		currentState.OnEnter(this);
	}
	
	private void SetGameOverState()
	{
		ChangeState(gameOverState);
	}
	
	#endregion
	
	#region Level Utility
	
	public void LoadScene(string sceneToLoad)
	{
		if (sceneToLoad != string.Empty)
			SceneManager.LoadScene(sceneToLoad);
	}
	
	public void ReloadCurrentScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	#endregion Utility
	
	private void OnDestroy()
	{
		// Unsubscribe from events
		GameOverButtonInteractable.onPressed -= SetGameOverState;
	}
}





// Source - https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/
public interface IGameState
{
	public void OnEnter(GameManager manager);
	public void UpdateState(GameManager manager);
	public void OnExit(GameManager manager);
}






public class PreludeState : IGameState
{
	public delegate void OnEntered();
	public static event OnEntered onEntered;
	public delegate void OnExited();
	public static event OnExited onExited;
	
    public void OnEnter(GameManager manager)
    { 
        if (onEntered != null)
			onEntered();
    }

    public void UpdateState(GameManager manager)
    {
        
    }

    public void OnExit(GameManager manager)
    {
        if (onExited != null)
			onExited();
    }
}

public class PlayingState : IGameState
{
	public delegate void OnEntered();
	public static event OnEntered onEntered;
	public delegate void OnExited();
	public static event OnExited onExited;	
	
    public void OnEnter(GameManager manager)
    { 
        if (onEntered != null)
			onEntered();
    }

    public void UpdateState(GameManager manager)
    {
        
    }

    public void OnExit(GameManager manager)
    {
        if (onExited != null)
			onExited();
    }
}

public class VictoryState : IGameState
{
	public delegate void OnEntered();
	public static event OnEntered onEntered;
	public delegate void OnExited();
	public static event OnExited onExited;
	
    public void OnEnter(GameManager manager)
    { 
        if (onEntered != null)
			onEntered();
    }

    public void UpdateState(GameManager manager)
    {
        
    }

    public void OnExit(GameManager manager)
    {
		if (onExited != null)
			onExited();
    }
}

public class GameOverState : IGameState
{
	public delegate void OnEntered();
	public static event OnEntered onEntered;
	public delegate void OnExited();
	public static event OnExited onExited;
	
    public void OnEnter(GameManager manager)
    {
		// Stop player movement, physics, etc.
		Time.timeScale = 0.0f;
		
        if (onEntered != null)
			onEntered();
    }

    public void UpdateState(GameManager manager)
    {
        
    }

    public void OnExit(GameManager manager)
    {
		if (onExited != null)
			onExited();
    }
}