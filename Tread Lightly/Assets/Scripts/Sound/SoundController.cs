using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    private SoundSource? _currentSoundSource;
    private SoundEmitter _emitter;
    private bool _emitOnce = true;

    private float _currentSoundCooldown = 0;

    void Start()
    {
        _emitter = GetComponent<SoundEmitter>();    
    }

    void Update()
    {
        if (_currentSoundSource == null)
        {
            return;
        }
        
        if (_currentSoundCooldown > 0)
        {
            _currentSoundCooldown -= Time.deltaTime;
        }

        if (_currentSoundCooldown <= 0)
        {
            EmitSound();
        }
    }

    private void EmitSound()
    {
        _emitter.MakeSound(new Sound(_currentSoundSource.Value, _currentSoundSource.Value.GetDefaultVolume()));
        if (_emitOnce)
        {
            _currentSoundSource = null;
        }
        else
        {
            _currentSoundCooldown = _currentSoundSource.Value.GetFrequency();
        }
    }

    public void SetSoundSource(SoundSource sound, bool emitOnce)
    {
        if (_currentSoundSource != sound)
        {
            _currentSoundCooldown = 0;
        }
        _currentSoundSource = sound;
        _emitOnce = emitOnce;
    }

    public void ClearSoundSource()
    {
        _currentSoundSource = null;
    }
}
