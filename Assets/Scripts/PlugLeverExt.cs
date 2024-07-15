using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlugLeverExt : Plug
{

    private new void Update()
    {
        if(!isConnected && _light.sprite != lightOff)
        {
            _light.sprite = lightOff;
        }
        if (isConnected && roomSolved)
        {
            // player was connected
            if(!CheckCorrectPoles())
            {
                // is not correct anymore (i.e. pole has disappeared)
                _light.sprite = lightWrong;
                doorL.PlayerDetached(_side);
                doorR.PlayerDetached(_side);
            }
        }
    }


}