using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Buttplug4Net35.Messages;
using JetBrains.Annotations;

namespace Buttplug4Net35
{
    /// <summary>
    /// Abstract representation of a device
    /// </summary>
    public abstract class ButtplugDevice : IButtplugDevice
    {
        /// <inheritdoc />
        public string Name { get; protected set; }

        /// <inheritdoc />
        public string Identifier { get; }

        /// <inheritdoc />
        public uint Index { get; set; }

        /// <inheritdoc />
        public bool IsConnected => !_isDisconnected;

        /// <inheritdoc />
        public event EventHandler DeviceRemoved;

        /// <summary>
        /// Gets the logger
        /// </summary>
        [NotNull]
        protected readonly IButtplugLog BpLogger;

        /// <summary>
        /// Gets the message handler functions
        /// </summary>
        [NotNull]
        protected readonly Dictionary<Type, ButtplugDeviceWrapper> MsgFuncs;

        private bool _isDisconnected;

        /// <inheritdoc />
        public event EventHandler<MessageReceivedEventArgs> MessageEmitted;

        /// <summary>
        /// A container class for message functions and attributes
        /// </summary>
        public class ButtplugDeviceWrapper
        {
            /// <summary>
            /// The function to call when a message of the particular type is received
            /// </summary>
            public Func<ButtplugDeviceMessage, Task<ButtplugMessage>> Function;

            /// <summary>
            /// A list of attributes ascoiated with the message
            /// </summary>
            public MessageAttributes Attrs;

            /// <summary>
            /// Initializes a new instance of the <see cref="ButtplugDeviceWrapper"/> class.
            /// </summary>
            /// <param name="aFunction">The method to call for the message</param>
            /// <param name="aAttrs">The message attributes</param>
            public ButtplugDeviceWrapper(Func<ButtplugDeviceMessage, Task<ButtplugMessage>> aFunction,
                                         MessageAttributes aAttrs = null)
            {
                Function = aFunction;
                Attrs = aAttrs ?? new MessageAttributes();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ButtplugDevice"/> class.
        /// </summary>
        /// <param name="aLogManager">The log manager</param>
        /// <param name="aName">The device name</param>
        /// <param name="aIdentifier">The device identifier</param>
        protected ButtplugDevice([NotNull] IButtplugLogManager aLogManager,
            [NotNull] string aName,
            [NotNull] string aIdentifier)
        {
            BpLogger = aLogManager.GetLogger(GetType());
            MsgFuncs = new Dictionary<Type, ButtplugDeviceWrapper>();
            Name = aName;
            Identifier = aIdentifier;
        }

        /// <inheritdoc />
        /// TODO: This should be a getter.
        public IEnumerable<Type> GetAllowedMessageTypes()
        {
            return MsgFuncs.Keys;
        }

        /// <inheritdoc />
        public MessageAttributes GetMessageAttrs(Type aMsg)
        {
            if (MsgFuncs.TryGetValue(aMsg, out var wrapper))
            {
                return wrapper.Attrs ?? new MessageAttributes();
            }

            return new MessageAttributes();
        }

        /// <summary>
        /// Invokes the DeviceRemoved event handler.
        /// Required to disconnect devices from the lower levels.
        /// </summary>
        protected void InvokeDeviceRemoved()
        {
            _isDisconnected = true;
            DeviceRemoved?.Invoke(this, new EventArgs());
        }

        /// <inheritdoc />
        public Task<ButtplugMessage> ParseMessage([NotNull] ButtplugDeviceMessage aMsg)
        {
            return new Task<ButtplugMessage>(() =>
            {
                if (_isDisconnected)
                {
                    return BpLogger.LogErrorMsg(aMsg.Id, Error.ErrorClass.ERROR_DEVICE,
                        $"{Name} has disconnected and can no longer process messages.");
                }

                if (!MsgFuncs.ContainsKey(aMsg.GetType()))
                {
                    return BpLogger.LogErrorMsg(aMsg.Id, Error.ErrorClass.ERROR_DEVICE,
                        $"{Name} cannot handle message of type {aMsg.GetType().Name}");
                }

                // We just checked whether the key exists above, so we're ok.
                // ReSharper disable once PossibleNullReferenceException
                return MsgFuncs[aMsg.GetType()].Function.Invoke(aMsg).Result;
            });
        }

        /// <inheritdoc />
        public virtual Task<ButtplugMessage> Initialize()
        {
            return new Task<ButtplugMessage>(() => new Ok(ButtplugConsts.SystemMsgId));
        }

        /// <inheritdoc />
        public abstract void Disconnect();

        /// <summary>
        /// Invokes the EmitMessage event handler.
        /// Required to allow events to be raised for this device from the lower levels.
        /// </summary>
        /// <param name="aMsg">The message to emit from the device</param>
        protected void EmitMessage(ButtplugMessage aMsg)
        {
            MessageEmitted?.Invoke(this, new MessageReceivedEventArgs(aMsg));
        }
    }
}