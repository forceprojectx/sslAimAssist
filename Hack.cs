using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace sslAimAssist
{
    class Hack : MonoBehaviour
    {
        private const string NOT_FOUND = "Aimer not Found";
        private const string INIT_MESSAGE = "FPX aim assist loaded";

        private string OverlayString = INIT_MESSAGE;

        private int _lastPower = 0;
        private int _lastAngle = 0;
        private float lastTime = Time.time;

        private double g = 9.812;
        private float powerSalt = 10.75f;

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                powerSalt -= 0.25f;
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                powerSalt += 0.25f;
            }

            if (Game.round.me.mc != null)
            {
                OverlayString = Aimer.instance.power + "   " + Aimer.instance.angle + "  " + powerSalt;
                OverlayString += Environment.NewLine + string.Format("ME:: X={0:0.0###}   Y={1}", Game.round.me.x, Game.round.me.y);


                foreach (Tank t in Game.round.tanks)
                {
                    OverlayString += Environment.NewLine + string.Format("X={0:0.0###}   Y={1}", t.x, t.y);
                }

                if ((Time.time > lastTime + 0.75) && (Aimer.instance.power != _lastPower || Aimer.instance.angle != _lastAngle))
                {
                    lastTime = Time.time;
                    _lastAngle = Aimer.instance.angle;
                    _lastPower = Aimer.instance.power;

                    Tank tPlayer = Game.round.me;
                    projectileMotion(tPlayer.x, tPlayer.y);
                }
            }
            else
            {
                OverlayString = NOT_FOUND;
            }
        }

        private void projectileMotion(float p1, int p2)
        {
            float angle = (float)getAngle(Aimer.instance.angle);
            int count = 0;
            if (Aimer.instance.angle <= 180)
            {
                for (int i = 1; i <= 1000; i += 10)
                {
                    float t = i / ((Aimer.instance.power + powerSalt) * Mathf.Cos(angle));
                    float Y = (float)(((Aimer.instance.power + powerSalt) * Mathf.Sin(angle)) * t - (0.5 * g * t * t));
                    Spawn.GO(Scene_Game.instance.tracerDotPrefab, null, Map.toWorldX((float)i + Game.round.me.x), Map.toWorldY(Game.round.me.y + Y), 0f, null).GetComponent<TracerDot>().Init(count);
                    count++;
                }
            }
            else
            {
                for (int i = 1; i > -1000; i -= 10)
                {
                    float t = i / ((Aimer.instance.power + powerSalt) * Mathf.Cos(angle));
                    float Y = (float)(((Aimer.instance.power + powerSalt) * Mathf.Sin(angle)) * t - (0.5 * g * t * t));
                    Spawn.GO(Scene_Game.instance.tracerDotPrefab, null, Map.toWorldX((float)i + Game.round.me.x), Map.toWorldY(Game.round.me.y + Y), 0f, null).GetComponent<TracerDot>().Init(count);
                    count++;
                }
            }
        }

        private float getAngle(int angle)
        {
            float ret = 0;
            if (angle <= 90)
            {
                ret = 90 - angle;
            }
            else if (angle > 90 && angle <= 180)
            {
                ret = (angle - 90) * -1;
            }
            else if (angle > 180 && angle <= 270)
            {
                ret = (angle - 270) * -1;
            }
            else if (angle > 270)
            {
                ret = (angle - 270) * -1;
            }
            ret = (float)((ret * Mathf.PI) / 180.0);
            return ret;
        }

        void OnGUI()
        {
            GUI.Label(new Rect(10, 25, 300, 300), OverlayString );
        }
    }
}
