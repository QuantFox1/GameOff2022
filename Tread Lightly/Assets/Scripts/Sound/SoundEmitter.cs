using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundEmitter : MonoBehaviour
{
    public static bool CanListenerHearASound(Vector3 positionOfListener)
    {
        var soundEmitters = FindObjectsOfType<SoundEmitter>();
        return soundEmitters.Any(emitter => emitter.SoundIntersectsPoint(positionOfListener));
    }

    [SerializeField] private Slider _soundLevelSlider;
    [SerializeField] private float _currentLoudestSound;
    [SerializeField] private float _maxVolume;
    [SerializeField] private float _distanceSoundTravels;

    private readonly List<Sound> _sounds = new List<Sound>();

    public void Update()
    {
        _sounds.RemoveAll(sound => !sound.IsAudible);

        if (!_sounds.Any())
        {
            return;
        }

        _currentLoudestSound = Mathf.Min(_sounds.Max(sound => sound.CurrentVolume), _maxVolume) / _maxVolume;

        if (_soundLevelSlider != null)
        {
            _soundLevelSlider.value = Mathf.Lerp(_soundLevelSlider.value, _currentLoudestSound, Time.deltaTime * 10);
        }
    }

    public void MakeSound(Sound soundCreated, bool removeOthersOfSameType=false)
    {
        if (removeOthersOfSameType)
        {
            _sounds.RemoveAll(sound => sound.Source == soundCreated.Source);
        }
        _sounds.Add(soundCreated);
    }

    public bool SoundIntersectsPoint(Vector3 point)
    {
        return (point - transform.position).magnitude <= _distanceSoundTravels * _currentLoudestSound;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _distanceSoundTravels * _currentLoudestSound);
    }
}
