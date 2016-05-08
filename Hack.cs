using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Vectrosity;

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
        private int increment = 10;
        private int count = 0;

        private double[] powerSalt={5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 5.5, 6.0, 6.0, 6.0, 6.0, 6.0, 6.25, 6.25, 6.25, 6.25, 6.25, 6.5, 6.5, 6.5, 6.5, 6.5, 7.0, 7.0, 7.0, 7.0, 7.0, 7.75, 7.75, 7.75, 7.75, 7.75, 8.25, 8.25, 8.25, 8.25, 8.25, 8.75, 8.75, 8.75, 8.75, 8.75, 9.5, 9.5, 9.5, 9.5, 9.5, 9.75, 9.75, 9.75, 9.75, 9.75, 10.5, 10.5, 10.5, 10.5, 10.5, 11.0, 11.0, 11.0, 11.0, 11.0, 11.5, 11.5, 11.5, 11.5, 11.5, 12.25, 12.25, 12.25, 12.25, 12.25, 12.75, 12.75, 12.75, 13.25, 13.25, 13.25 };

        void Start()
        {

        }

        void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                increment += 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                increment -= 1;
                if (increment < 1)
                {
                    increment = 1;                    
                }
            }


            if (Game.round.me.mc != null)
            {
                OverlayString = Aimer.instance.power + "   " + Aimer.instance.angle + "  " + powerSalt[Aimer.instance.power] + "     increment:: "+ increment;
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
            int dAngle = Aimer.instance.angle;
            int power = Aimer.instance.power;
            float angle = (float)getAngle(dAngle);
            int Xpos = (int)(Game.round.me.x);

            count++;
            if (count > 5)
            {
                count = 0;
                Tracers.Clear();
            }

            if (dAngle <= 180)
            {
                Proj p = new Proj();
                TracerLine tracer = Tracers.CreateLine(p, Color.red);

                for (int i = 1; i <= 1000-Xpos; i += increment)
                {
                    float t = i / (float)((power + powerSalt[power]) * Mathf.Cos(angle));
                    float Y = (float)(((power + powerSalt[power]) * Mathf.Sin(angle)) * t - (0.5 * g * t * t));
                    
                    Vector3 point = new Vector3(Map.toWorldX((float)i+Xpos), Map.toWorldY(Game.round.me.y + Y));
                    tracer.line.points3.Add(point);

                    if (Game.round.me.y + Y < 0)
                    {
                        break;//round has gone off bottom of screen
                    }    
                }
                tracer.line.Draw3D();
                
            }
            else
            {
                Proj p = new Proj();
                TracerLine tracer = Tracers.CreateLine(p, Color.red);
                for (int i = 1; i < Xpos ; i += increment)
                {
                    float t = i / (float)((power + powerSalt[power]) * Mathf.Cos(angle));
                    float Y = (float)(((power + powerSalt[power]) * Mathf.Sin(angle)) * t - (0.5 * g * t * t));

                    Vector3 point = new Vector3(Map.toWorldX((float)Xpos - i), Map.toWorldY(Game.round.me.y + Y));
                    tracer.line.points3.Add(point);

                    if (Game.round.me.y + Y < 0)
                    {
                        break;//round has gone off bottom of screen
                    }                    
                }
                tracer.line.Draw3D();
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
                ret = (angle - 270) * 1;
            }
            else if (angle > 270)
            {
                ret = (angle - 270) * 1;
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
