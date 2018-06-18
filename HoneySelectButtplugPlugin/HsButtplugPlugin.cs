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
        private bool _inOrgasm;
        private DateTime _endOrgsam;

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
            bool orgasm = false;
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

                    // f_heart and m_heart are the sweet spots for the actors
                    // clip names are either hit or idle
                    //Found: heart_m - heart_m - 1 looping
                    //    idle
                    //Found: heart_f - heart_f - 1 looping
                    //    hit
                    if ((go.name == "heart_m" || go.name == "heart_f") && c.clip.name == "hit")
                        hit = true;

                    // p_cm_anim and p_cf_anim are the character movement animations
                    // When orgasming, they are set to M_Orgasm, M_Orgasm, M_Orgasm_out, etc
                    //Found: p_cm_anim - p_cm_anim - 3.809526
                    //    M_Orgasm
                    //    M_Orgasm
                    //Found: p_cf_anim - p_cf_anim - 3.809526
                    //    M_Orgasm
                    //    M_Orgasm
                    if (!_inOrgasm && (go.name == "p_cf_anim" || go.name == "p_cm_anim") && c.clip.name.Contains("_Orgasm") && !c.clip.name.EndsWith("_A"))
                    {
                        orgasm = true;
                        _endOrgsam = DateTime.Now.AddSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
                    }
                }
            }

            if (_inOrgasm)
            {
                orgasm = _endOrgsam >= DateTime.Now;
            }

            if (_vibrate == hit && orgasm == _inOrgasm)
            {
                return;
            }

            Console.WriteLine("State changed!\n" + data);
            _vibrate = hit;
            _inOrgasm = orgasm;

            foreach (var dev in _bpClient.Devices)
            {
                Console.WriteLine("Sending vibrate cmd to " + dev.Name);
                var res = _bpClient.SendDeviceMessage(dev, new SingleMotorVibrateCmd(dev.Index, _inOrgasm ? 1 : _vibrate ? 0.5 : 0))
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
