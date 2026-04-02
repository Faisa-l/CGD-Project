using System.Collections;
using UnityEngine;

/* I decided to make the particle effect system revolve around this abstract class since it seemed to be the most simple method of implementing  
 * a 'Particle System Library' that could host different kinds of particle effects.
 * The idea is that any particle system that has some special code to function would implement this class so that it could still be added in the library.
 * I didn't want there to be a lot of separate particle effect classes and instead thought it would be neater if they all could exist in the same space.
 */

// --- IMPORTANT ---
// To create a new type of particle effect, make a new C# code that inherits from ParticleEffect.
// Note that particle effects with their particle system's "play on awake" OR "looping" set to true will not be automatically managed by the system.
// This system is not intended for particle systems with "play on awake" set to true 

/// <summary>
/// Represents a particle system on a game object.
/// This class has the basic functions for handling the lifetime of the particle effect and its particle system.
/// Any more complex particle effect should be casted to a specified type.
/// </summary>
[RequireComponent(typeof(ParticleSystem))]
public abstract class ParticleEffect : MonoBehaviour
{
    ParticleSystem ps;

    // When first getting the particle system it will fetch the component
    public ParticleSystem ParticleSystem 
    {
        get => GetPS();
        protected set => ps = value; 
    }

    public ParticleEffect AtPosition(Vector3 position)
    {
        transform.position = position;
        return this;
    }

    public ParticleEffect AtRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
        return this;
    }

    /// <summary>
    /// Calls <see cref="ParticleSystem.Play()"/> and destroy's this object once the effect stops.
    /// </summary>
    /// <remarks> When overriding this function, ensure that any logic or setup is done before calling base.Play() </remarks>
    public virtual void Play()
    {
        var main = ParticleSystem.main;
        if (!ParticleSystem.isPlaying) ParticleSystem.Play();
        if (!main.loop || main.playOnAwake) StartCoroutine(YieldForDestroy());
    }

    // Destroys this object when the particle system stops playing
    IEnumerator YieldForDestroy()
    {
        yield return new WaitWhile(() => ParticleSystem.isPlaying);
        Destroy(gameObject);
    }

    ParticleSystem GetPS()
    {
        if (ps == null) ps = GetComponent<ParticleSystem>();
        return ps;
    }

}
