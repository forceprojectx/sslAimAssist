using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace sslAimAssist
{
    public class Class1
    {
        public static GameObject load_obj;

        public static void Load()
        {
            load_obj = new GameObject();
            load_obj.AddComponent<Hack>();
            UnityEngine.Object.DontDestroyOnLoad(load_obj);
        }
    }
}
