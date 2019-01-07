using System;
using System.Linq;
using IllusionPlugin;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HoneySelectButtplugPlugin
{
    public class HsButtplugPlugin : IEnhancedPlugin
    {
        private const string PluginName = "HoneySelectButtplugPlugin";
        private const string PluginVersion = "0.0.2";

        public string Name => PluginName;

        public string Version => PluginVersion;

        public string[] Filter { get; } = null;

        private static readonly string[] SceneFilter =
        {
            "HScene",
        };


        public void OnLevelWasLoaded(int level)
        {
            StartMod();
        }

        private static void StartMod()
        {
            if (SceneFilter.Contains(SceneManager.GetActiveScene().name))
            {
                new GameObject(PluginName).AddComponent<HsButtplug>();
            }
        }

        public static void Bootstrap()
        {
            Console.Out.WriteLine("[BP] Bootstapping");
            var gameObject = GameObject.Find(PluginName);
            if (gameObject != null)
            {
                GameObject.DestroyImmediate(gameObject);
            }

            StartMod();
        }

        public void OnApplicationStart(){}
        public void OnUpdate(){}
        public void OnLateUpdate(){}
        public void OnApplicationQuit(){}
        public void OnLevelWasInitialized(int level){}
        public void OnFixedUpdate(){}
    }
}
