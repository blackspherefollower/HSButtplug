using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Buttplug4Net35.Messages;
using JetBrains.Annotations;
using LitJson;
using UnityEngine;
using Ping = Buttplug4Net35.Messages.Ping;

namespace Buttplug4Net35
{
    /// <summary>
    /// Handles the seralization (object to JSON), deserialization (JSON to object) and validation of messages.
    /// </summary>
    public class ButtplugJsonMessageParser
    {
        [NotNull]
        private readonly Dictionary<string, Type> _messageTypes;

        private readonly IButtplugLog _bpLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtplugJsonMessageParser"/> class.
        /// </summary>
        /// <param name="aLogManager">Log manager</param>
        public ButtplugJsonMessageParser(IButtplugLogManager aLogManager = null)
        {
            _bpLogger = aLogManager.GetLogger(GetType());
            _bpLogger?.Info($"Setting up {GetType().Name}");

            IEnumerable<Type> allTypes;

            // Some classes in the library may not load on certain platforms due to missing symbols.
            // If this is the case, we should still find messages even though an exception was thrown.
            try
            {
                allTypes = Assembly.GetAssembly(typeof(ButtplugMessage)).GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                allTypes = e.Types;
            }

            var messageClasses = allTypes.Where(t => t != null && t.IsClass && t.Namespace == "Buttplug4Net35.Messages" && typeof(ButtplugMessage).IsAssignableFrom(t));

            var enumerable = messageClasses as Type[] ?? messageClasses.ToArray();
            _bpLogger?.Debug($"Message type count: {enumerable.Length}");
            _messageTypes = new Dictionary<string, Type>();
            enumerable.ToList().ForEach(aMessageType =>
            {
                _bpLogger?.Debug($"- {aMessageType.Name}");
                _messageTypes.Add(aMessageType.Name, aMessageType);
            });
        }

        /// <summary>
        /// Deserializes Buttplug messages from JSON into an array of <see cref="ButtplugMessage"/> objects.
        /// </summary>
        /// <param name="aJsonMsg">String containing one or more Buttplug messages in JSON format</param>
        /// <returns>Array of <see cref="ButtplugMessage"/> objects</returns>
        [NotNull]
        public ButtplugMessage[] Deserialize(string aJsonMsg)
        {
            _bpLogger?.Trace($"Got JSON Message: {aJsonMsg}");

            var res = new List<ButtplugMessage>();
            JsonData json = JsonMapper.ToObject(aJsonMsg);
            for (var i = 0; i < json.Count; ++i)
            {
                foreach (var key in json[i].Keys)
                {
                    if (json[i][key].Keys.Contains("Id"))
                    {
                        _bpLogger?.Trace($"Got BP {key} Message: {json[i][key].ToJson()}");
                        switch (key)
                        {
                            case "Ok":
                                res.Add(JsonUtility.FromJson<Ok>(json[i][key].ToJson()));
                                break;
                            case "Test":
                                res.Add(JsonUtility.FromJson<Test>(json[i][key].ToJson()));
                                break;
                            case "Error":
                                res.Add(JsonUtility.FromJson<Error>(json[i][key].ToJson()));
                                break;
                            case "DeviceList":
                                var dList = JsonUtility.FromJson<DeviceList>(json[i][key].ToJson());
                                List<DeviceMessageInfo> devs = new List<DeviceMessageInfo>();
                                if (json[i][key].ContainsKey("Devices") && json[i][key]["Devices"].IsArray)
                                {
                                    foreach (JsonData dev in json[i][key]["Devices"])
                                    {
                                        var dInfo = JsonUtility.FromJson<DeviceMessageInfo>(dev.ToJson());
                                        dInfo.DeviceMessages = new Dictionary<string, MessageAttributes>();
                                        if (dev.ContainsKey("DeviceMessages"))
                                        {
                                            foreach (var key2 in dev["DeviceMessages"].Keys)
                                            {
                                                var attr = new MessageAttributes();
                                                if (dev["DeviceMessages"][key2].ContainsKey("FeatureCount"))
                                                {
                                                    attr.FeatureCount = Convert.ToUInt32(dev["DeviceMessages"][key2]["FeatureCount"].IsInt);
                                                }

                                                dInfo.DeviceMessages.Add(key2, attr);
                                            }
                                        }
                                        devs.Add(dInfo);
                                    }

                                    dList.Devices = devs.ToArray();
                                }

                                res.Add(dList);
                                _bpLogger.Trace("Converted back: " + JsonMapper.ToJson(res.Last()));
                                break;
                            case "DeviceAdded":
                                var dAdded = JsonUtility.FromJson<DeviceAdded>(json[i][key].ToJson());
                                dAdded.DeviceMessages = new Dictionary<string, MessageAttributes>();
                                if (json[i][key].ContainsKey("DeviceMessages"))
                                {
                                    foreach (var key2 in json[i][key]["DeviceMessages"].Keys)
                                    {
                                        var attr = new MessageAttributes();
                                        if (json[i][key]["DeviceMessages"][key2].ContainsKey("FeatureCount"))
                                        {
                                            attr.FeatureCount = (uint)json[i][key]["DeviceMessages"][key2]["FeatureCount"];
                                        }

                                        dAdded.DeviceMessages.Add(key2, attr);
                                    }
                                }
                                res.Add(dAdded);
                                _bpLogger.Trace("Converted back: " + JsonMapper.ToJson(res.Last()));
                                break;
                            case "DeviceRemoved":
                                res.Add(JsonUtility.FromJson<DeviceRemoved>(json[i][key].ToJson()));
                                break;
                            case "ScanningFinished":
                                res.Add(JsonUtility.FromJson<ScanningFinished>(json[i][key].ToJson()));
                                break;
                            case "Log":
                                res.Add(JsonUtility.FromJson<Log>(json[i][key].ToJson()));
                                break;
                            case "ServerInfo":
                                res.Add(JsonUtility.FromJson<ServerInfo>(json[i][key].ToJson()));
                                break;
                            default:
                                _bpLogger?.Trace($"Don't know how to handle: {key}");
                                break;
                        }
                    }
                }
            }
            /*
            JArray msgArray;
            try
            {
                msgArray = JArray.Parse(aJsonMsg);
            }
            catch (JsonReaderException e)
            {
                var err = new Error($"Not valid JSON: {aJsonMsg} - {e.Message}", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                _bpLogger?.LogErrorMsg(err);
                res.Add(err);
                return res.ToArray();
            }

            if (!msgArray.Any())
            {
                var err = new Error("No messages in array", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                _bpLogger?.LogErrorMsg(err);
                res.Add(err);
                return res.ToArray();
            }

            // JSON input is an array of messages.
            // We currently only handle the first one.
            foreach (var o in msgArray.Children<JObject>())
            {
                if (!o.Properties().Any())
                {
                    var err = new Error("No message name available", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                    _bpLogger?.LogErrorMsg(err);
                    res.Add(err);
                    continue;
                }

                var msgName = o.Properties().First().Name;
                if (!_messageTypes.Any() || !_messageTypes.ContainsKey(msgName))
                {
                    var err = new Error($"{msgName} is not a valid message class", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                    _bpLogger?.LogErrorMsg(err);
                    res.Add(err);
                    continue;
                }

                // This specifically could fail due to object conversion.
                res.Add(DeserializeAs(o, _messageTypes[msgName], msgName, aJsonMsg));
            }
            */
            return res.ToArray();
        }

        /*
        private ButtplugMessage DeserializeAs(JObject aObject, Type aMsgType, string aMsgName, string aJsonMsg)
        {
            try
            {
                var r = aObject[aMsgName].Value<JObject>();
                var msg = (ButtplugMessage)r.ToObject(aMsgType, _serializer);
                _bpLogger?.Trace($"Message successfully parsed as {aMsgName} type");
                return msg;
            }
            catch (InvalidCastException e)
            {
                var err = new Error($"Could not create message for JSON {aJsonMsg}: {e.Message}", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                _bpLogger?.LogErrorMsg(err);
                return err;
            }
            catch (JsonSerializationException e)
            {
                // Object didn't fit. Downgrade?
                var tmp = (ButtplugMessage)aMsgType.GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null, Type.EmptyTypes, null)?.Invoke(null);
                if (tmp?.PreviousType != null)
                {
                    var msg = DeserializeAs(aObject, tmp.PreviousType, aMsgName, aJsonMsg);
                    if (!(msg is Error))
                    {
                        return msg;
                    }
                }

                var err = new Error($"Could not create message for JSON {aJsonMsg}: {e.Message}", Error.ErrorClass.ERROR_MSG, ButtplugConsts.SystemMsgId);
                _bpLogger?.LogErrorMsg(err);
                return err;
            }
        }*/

        /// <summary>
        /// Serializes a single <see cref="ButtplugMessage"/> object into a JSON string for a specified version of the schema.
        /// </summary>
        /// <param name="aMsg"><see cref="ButtplugMessage"/> object</param>
        /// <param name="clientSchemaVersion">Target schema version</param>
        /// <returns>JSON string representing a Buttplug message</returns>
        public string Serialize([NotNull] ButtplugMessage aMsg, uint clientSchemaVersion)
        {
            return Serialize(new [] {aMsg}, clientSchemaVersion);
        }

        /// <summary>
        /// Serializes a collection of ButtplugMessage objects into a JSON string for a specified version of the schema.
        /// </summary>
        /// <param name="aMsgs">A collection of ButtplugMessage objects</param>
        /// <param name="clientSchemaVersion">The target schema version</param>
        /// <returns>A JSON string representing one or more Buttplug messages</returns>
        public string Serialize([NotNull] IEnumerable<ButtplugMessage> aMsgs, uint clientSchemaVersion)
        {
            Console.WriteLine($"Type: {aMsgs.GetType()}, Size: {aMsgs.Count()}");
            if (!aMsgs.Any())
            {
                _bpLogger?.Trace($"Sending JSON Message: []", true);
                return "[]";
            }

            var msgs = new List<string>();
            foreach (var msg in aMsgs)
            {
                Console.WriteLine($"Type: {msg.GetType()}");
                if (msg.GetType() == typeof(Ok))
                    msgs.Add("{\"" + msg.GetType().Name + "\": {" + JsonUtility.ToJson((Ok)msg) + "}");
                else if (msg.GetType() == typeof(Error))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((Error)msg) + "}");
                else if (msg.GetType() == typeof(Ping))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((Ping)msg) + "}");
                else if (msg.GetType() == typeof(RequestServerInfo))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((RequestServerInfo)msg) + "}");
                else if (msg.GetType() == typeof(RequestDeviceList))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((RequestDeviceList)msg) + "}");
                else if (msg.GetType() == typeof(StartScanning))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((StartScanning)msg) + "}");
                else if (msg.GetType() == typeof(StopScanning))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((StopScanning)msg) + "}");
                else if (msg.GetType() == typeof(StopAllDevices))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((StopAllDevices)msg) + "}");
                else if (msg.GetType() == typeof(StopDeviceCmd))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((StopDeviceCmd)msg) + "}");
                else if (msg.GetType() == typeof(RequestLog))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((RequestLog)msg) + "}");
                else if (msg.GetType() == typeof(SingleMotorVibrateCmd))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((SingleMotorVibrateCmd)msg) + "}");
                else if (msg.GetType() == typeof(VibrateCmd))
                    msgs.Add("{\"" + msg.GetType().Name + "\": " + JsonUtility.ToJson((VibrateCmd)msg) + "}");
            }

            var aJsonMsg = "[" + string.Join(",", msgs.ToArray()) + "]";
            _bpLogger?.Trace($"Sending JSON Message: {aJsonMsg}", true);
            return aJsonMsg;
        }
    }
}