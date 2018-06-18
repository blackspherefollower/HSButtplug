using System;
using System.Linq;
using System.Threading.Tasks;
using Buttplug4Net35;
using Buttplug4Net35.Messages;
using IllusionPlugin;
using UnityEngine;

namespace HoneySelectButtplugPlugin
{
    public class HsButtplugPlugin : IEnhancedPlugin
    {
        public string Name { get; } = "HoneySelectButtplugPlugin";

        public string Version { get; } = "0.0.1";

        public string[] Filter { get; } = null;

        private bool _inHScene;
        private ButtplugWSClient _bpClient;

        private bool _vibrate;

        public void OnApplicationStart()
        {
            _bpClient = new ButtplugWSClient("HoneySelect");
            _bpClient.Connect(new Uri("ws://127.0.0.1:12345/buttplug"));
            Console.WriteLine("[Buttplug] Connected");
            if (_bpClient.StartScanning().Result)
            {
                Console.WriteLine("[Buttplug] Scanning started");
            }
        }

        public void OnApplicationQuit()
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
            if (GameObject.Find("HScene") != null)
            {
                _inHScene = true;
            }
            else
            {
                if (_inHScene)
                {
                    _inHScene = false;
                }
            }
        }

        public void OnUpdate()
        {
            if (!_inHScene)
            {
                return;
            }

            bool hit = false;
            string data = "";
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                Animator anim;
                if (!go.activeInHierarchy || (anim = go.GetComponent<Animator>()) == null)
                {
                    continue;
                }

                data += "Found: " + go.name + " - " + anim.name + " - " +
                                  anim.GetCurrentAnimatorStateInfo(0).length +
                                  (anim.GetCurrentAnimatorStateInfo(0).loop ? " looping\n" : "\n");
                
                foreach (var c in anim.GetCurrentAnimatorClipInfo(0))
                {
                    data += "\t" + c.clip.name + "\n";
                    if ((go.name == "heart_m" || go.name == "heart_f") && c.clip.name == "hit")
                        hit = true;
                }


                // f_heart and m_heart are the sweet spots for the actors
                // clip names are either hit or idle
                //Found: heart_m - heart_m - 1 looping
                //idle
                //Found: heart_f - heart_f - 1 looping
                //    hit
            }

            if (_vibrate == hit)
            {
                return;
            }

            Console.WriteLine("State changed!\n" + data);
            _vibrate = hit;
            foreach (var dev in _bpClient.Devices)
            {
                Console.WriteLine("Sending vibrate cmd to " + dev.Name);
                var res = _bpClient.SendDeviceMessage(dev, new SingleMotorVibrateCmd(dev.Index, _vibrate ? 1 : 0))
                    .Result;
                if (res.GetType() == typeof(Error))
                {
                    Console.WriteLine(((Error)res).ErrorMessage);
                }
            }
        }

        public void OnFixedUpdate()
        {
        }

        public void OnLateUpdate()
        {
        }
    }
}
