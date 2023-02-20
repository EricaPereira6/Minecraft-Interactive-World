using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class Receiver : MonoBehaviour
{
    OSCReceiver receiver;
    public Transform dirLight;

    float lux;
    Vector3 acc;

    enum sky { DARK, KINDADARK, LIGHT};
    sky skyState;

    // Start is called before the first frame update
    void Start()
    {
        receiver = gameObject.AddComponent<OSCReceiver>();
        receiver.LocalHost = "192.168.1.73";
        receiver.LocalPort = 7002;
        receiver.Bind("/light", LightReceiver);
        receiver.Bind("/", AccReceiver);

        skyState = sky.LIGHT;
    }

    void MessageReceiver(OSCMessage message)
    {
        Debug.Log(message);
    }

    void LightReceiver(OSCMessage message)
    {
        //Debug.Log(message);
        message.ToFloat(out lux);

        if (lux < 26 && skyState != sky.DARK)
        {
            dirLight.transform.rotation = Quaternion.Euler(-10, 0, 0);
            skyState = sky.DARK;
        }
        else if (lux > 25 && lux < 500 && skyState != sky.KINDADARK)
        {
            dirLight.transform.rotation = Quaternion.Euler(0, 0, 0);
            skyState = sky.KINDADARK;
        }
        else if (lux > 499 && skyState != sky.LIGHT)
        {
            dirLight.transform.rotation = Quaternion.Euler(50, 0, 0);
            skyState = sky.LIGHT;
        }
    }

    void AccReceiver(OSCMessage message)
    {
        message.ToVector3(out acc);
        Debug.Log("Acc: " + acc);
    }

}
