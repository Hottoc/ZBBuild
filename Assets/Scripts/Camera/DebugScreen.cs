using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScreen : MonoBehaviour
{
    World world;
    Player player;
    Text text;
    float frameRate;
    float timer;
    string facing;
    float playerRotationRound;
    private bool cameraCanRotate = false;

    // Start is called before the first frame update
    void Start()
    {
        world = GameObject.Find("Tile").GetComponent<World>();
        player = GameObject.Find("Player").GetComponent<Player>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        string debugText = "Untitled Game By Owen Hott";
        debugText += "\n";
        debugText += frameRate + "fps";
        debugText += "\n\n";
        debugText += "[Player]";
        debugText += "\n";
        debugText += "XYZ: " + (Mathf.Round(world.player.transform.position.x * 100) / 100) + " / " + (Mathf.Round(world.player.transform.position.y * 100) / 100) + " / " + (Mathf.Round(world.player.transform.position.z * 100) / 100);
        debugText += "\n";

        playerRotationRound = (Mathf.Round(world.player.transform.rotation.eulerAngles.y));

        if (playerRotationRound < 45  || playerRotationRound > 315)
        {
            facing = "north (Towards positive Z)";
        }
        
        if (playerRotationRound >= 45  && playerRotationRound <= 135)
        {
            facing = "east (Towards positive X)";
        }
        
        if ((playerRotationRound > 135.0f) && (playerRotationRound < 225.0f))
        {
            facing = "south (Towards negative Z)";
        }
        if ((playerRotationRound >= 225.0f) && (playerRotationRound <= 315.0f))
        {
            facing = "west (Towards negative X)";
        }

        debugText += "Facing: " + facing;
        debugText += "\n";
        debugText += "Can jump: " + player.jumpRequest;
        debugText += "\n";
        debugText += "Is running: " + player.isSprinting;
        debugText += "\n";
        debugText += "Can move: " + player.playerCanMove;
        debugText += "\n\n";

        // CAMERA

        debugText += "[Camera]";
        debugText += "\n";
        //debugText += "XYZ: " + (Mathf.Round(world.camera.transform.position.x * 100) / 100) + " / " + (Mathf.Round(world.camera.transform.position.y * 100) / 100) + " / " + (Mathf.Round(world.camera.transform.position.z * 100) / 100);

        /*
        if (Input.GetMouseButton(1))
        {
            cameraCanRotate = true;
        }
        else
        {
            cameraCanRotate = false;
        }
        */

        debugText += "\n";
        debugText += "Can move: " + cameraCanRotate;
        debugText += "\n\n";

        // HARDWARE
        debugText += "[Hardware]";
        debugText += "\n";
        debugText += "OS: " + SystemInfo.operatingSystem;
        debugText += "\n";
        debugText += "CPU: " + SystemInfo.processorType;
        debugText += "\n";
        debugText += "----Threads: " + SystemInfo.processorCount;
        //debugText += "\n";
        //debugText += SystemInfo.processorFrequency;
        debugText += "\n";
        debugText += "GPU: " + SystemInfo.graphicsDeviceName;
        //debugText += "\n";
        //debugText += "----Memory: " + SystemInfo.graphicsMemorySize;
        debugText += "\n";
        debugText += "RAM: " + SystemInfo.systemMemorySize;



        



        debugText += "\n\n";
        debugText += "To close debug: press F3";


        text.text = debugText;

        if (timer > 1f)
        {
            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
        }


    }

    private void DrawBorder()
    {


    }
}
