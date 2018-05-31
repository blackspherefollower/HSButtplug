using System;
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

        public void OnApplicationStart()
        {
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

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject go in allObjects)
            {
                Animator anim;
                if (!go.activeInHierarchy || (anim = go.GetComponent<Animator>()) == null)
                {
                    continue;
                }

                Console.WriteLine("Found: " + go.name + " - " + anim.name + " - " +
                                  anim.GetCurrentAnimatorStateInfo(0).length +
                                  (anim.GetCurrentAnimatorStateInfo(0).loop ? " looping" : ""));
                foreach (var c in anim.GetCurrentAnimatorClipInfo(0))
                {
                    Console.WriteLine("\t" + c.clip.name);
                }

                // f_heart and m_heart are the sweet spots for the actors
                // clip names are either hit or idle
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
