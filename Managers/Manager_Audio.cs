using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager_Audio : MonoBehaviour
{
    [SerializeField] FMODUnity.EventReference _currentSongReference;
    FMOD.Studio.EventInstance _currentSongInstance;

    [SerializeField] public List<LocalParameter> _localParameters;
    [SerializeField] List<GlobalParameter> _globalParameters;

    private void Start()
    {
        PlaySong(_currentSongReference);
    }

    public void PlaySong(FMODUnity.EventReference audio)
    {
        _currentSongReference = audio;
        _currentSongInstance = FMODUnity.RuntimeManager.CreateInstance(audio);

        _currentSongInstance.getDescription(out FMOD.Studio.EventDescription eventDescription);
        eventDescription.getParameterDescriptionCount(out int parameterCount);

        for (int i = 0; i < parameterCount; i++)
        {
            FMOD.RESULT result = eventDescription.getParameterDescriptionByIndex(i, out FMOD.Studio.PARAMETER_DESCRIPTION parameterDescription);
            if (result != FMOD.RESULT.OK) Debug.LogError($"Failed to get parameter description for index {i}: {result}");
            LocalParameter localParameter = new LocalParameter().SetParameterID(parameterDescription.name, parameterDescription.id);
            _localParameters.Add(localParameter);
        }

        _currentSongInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        _currentSongInstance.start();
    }

    void Update()
    {
        // Change this to me only updated according to code rather than this which is for editor.
        foreach (LocalParameter parameter in _localParameters) ChangeLocalParameters(parameter);
        foreach (GlobalParameter parameter in _globalParameters) ChangeGlobalParameters(parameter);

        _currentSongInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    public void ChangeLocalParameters(LocalParameter parameter)
    {
        _currentSongInstance.setParameterByID(parameter.ParameterID, parameter.Value);
    }

    public void ChangeGlobalParameters(GlobalParameter parameter)
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByID(parameter.ParameterID, parameter.Value);
    }
}

[Serializable]
public class LocalParameter
{
    [SerializeField] string _name;
    public FMOD.Studio.PARAMETER_ID ParameterID { get; private set; }
    [Range(0,1)]public float Value;

    public LocalParameter SetParameterID(string name, FMOD.Studio.PARAMETER_ID parameterID)
    {
        _name = name;
        ParameterID = parameterID;

        return this;
    }
}

[Serializable]
public class GlobalParameter
{
    [SerializeField] string _name;
    public FMOD.Studio.PARAMETER_ID ParameterID { get; private set; }
    [Range(0, 1)] public float Value;

    public GlobalParameter SetParameterID(string name, FMOD.Studio.PARAMETER_ID parameterID)
    {
        _name = name;
        ParameterID = parameterID;

        return this;
    }
}
