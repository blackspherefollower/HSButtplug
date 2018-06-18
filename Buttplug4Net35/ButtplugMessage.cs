using System;
using System.Collections.Generic;
using Buttplug4Net35.Messages;

namespace Buttplug4Net35
{
    /// <summary>
    /// Base class for Buttplug protocol messages.
    /// </summary>
    [Serializable]
    public abstract class ButtplugMessage
    {
        /// <summary>
        /// Message ID.
        /// </summary>
        public uint Id;

        

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtplugMessage"/> class.
        /// </summary>
        /// <param name="aId">Message ID</param>
        protected ButtplugMessage(uint aId)
        {
            Id = aId;
        }
    }

    /// <summary>
    /// Interface for messages only sent from server to client.
    /// </summary>
    public interface IButtplugMessageOutgoingOnly
    {
    }

    /// <summary>
    /// Interface for messages containing Device Info, such as DeviceAdded/Removed.
    /// </summary>
    public interface IButtplugDeviceInfoMessage
    {
        /// <summary>
        /// Device name.
        /// </summary>
        string DeviceName { get; }

        /// <summary>
        /// Device index, as assigned by a Buttplug server.
        /// </summary>
        uint DeviceIndex { get; }

        /// <summary>
        /// Buttplug messages supported by this device, with additional attributes.
        /// </summary>
        Dictionary<string, MessageAttributes> DeviceMessages { get; }
    }
}