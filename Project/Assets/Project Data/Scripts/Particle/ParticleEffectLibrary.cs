using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a collection of particle systems for specific particle effects.
/// Avoid directly editing instances of this object.
/// </summary>
[CreateAssetMenu(fileName = "ParticleEffectLibrary", menuName = "Scriptable Objects/ParticleEffectLibrary")]
public class ParticleEffectLibrary : ScriptableObject
{
    // For the list
    [System.Serializable]
    public struct Pair
    {
        public string name;
        public ParticleEffect particleEffect;
    }

    [SerializeField, Tooltip("The internal name of a particle effect and the particle effect script attached to the prefab, NOT the prefab itself.")]
    List<Pair> particleEffects;

    /// <summary>
    /// Play a particle effect prefab. The object is destroyed once the effect ends.
    /// </summary>
    /// <param name="effectName"> Name of the particle effect. Must exist in the <see cref="particleEffects"/> list. </param>
    /// <param name="position"> Position to spawn the effect at. </param>
    /// <param name="rotation"> Rotation of the effect. </param>
    /// <returns> The particle effect component. Note that non-looping effects are pending destruction once the effect stops. </returns>
    public ParticleEffect Play(string effectName, Vector3 position = default, Quaternion rotation = default)
    {
        ParticleEffect effect = Get(effectName)
            .AtPosition(position)
            .AtRotation(rotation);

        effect.Play();

        return effect;
    }

    /// <summary>
    /// Returns an instantaited Particle Effect prefab.
    /// </summary>
    /// <param name="effectName"> Name of the particle effect. Must exist in the <see cref="particleEffects"/> list. </param>
    public ParticleEffect Get(string effectName) => Instantiate(particleEffects.Find(x => x.name == effectName).particleEffect);


    // These do what the above functions does but it will automatically cast them into the required type
    public T Get<T>(string effectName) where T : ParticleEffect => Get(effectName) as T;

    public T Play<T>(string effectName, Vector3 position = default, Quaternion rotation = default) where T : ParticleEffect
        => Play(effectName, position, rotation) as T;

    // Some constant strings of known particle effect names
    public static readonly string
        CrateCollisionSparks = "CrateCollisionSparks",
        Confetti = "Confetti";
}
