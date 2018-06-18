using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Buttplug4Net35.Messages
{
    [Serializable]
    public class ButtplugMessageWrapper
    {
        public Ok Ok;
        public Test Test;
        public Error Error;
        public DeviceList DeviceList;
        public DeviceAdded DeviceAdded;
        public DeviceRemoved DeviceRemoved;
        public ScanningFinished ScanningFinished;
        public Log Log;
        public ServerInfo ServerInfo;

        public Type GetType()
        {
            if (Ok != null)
            {
                return typeof(Ok);
            }
            if (Test != null)
            {
                return typeof(Test);
            }
            if (Error != null)
            {
                return typeof(Error);
            }
            if (DeviceList != null)
            {
                return typeof(DeviceList);
            }
            if (DeviceAdded != null)
            {
                return typeof(DeviceAdded);
            }
            if (DeviceRemoved != null)
            {
                return typeof(DeviceRemoved);
            }
            if (ScanningFinished != null)
            {
                return typeof(ScanningFinished);
            }
            if (Log != null)
            {
                return typeof(Log);
            }
            if (ServerInfo != null)
            {
                return typeof(ServerInfo);
            }

            return null;
        }
    }
}
