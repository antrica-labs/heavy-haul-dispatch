using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SingerDispatch
{
    class UserSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("true")]
        public Boolean MetricMeasurements
        {
            get
            {
                return (Boolean)this["MetricMeasurements"];
            }
            set
            {
                this["MetricMeasurements"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string MainWindowPlacement
        {
            get
            {
                return (string)this["MainWindowPlacement"];
            }
            set
            {
                this["MainWindowPlacement"] = value;
            }
        }

        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string DocumentViewerWindowPlacement
        {
            get
            {
                return (string)this["DocumentViewerWindowPlacement"];
            }
            set
            {
                this["DocumentViewerWindowPlacement"] = value;
            }
        }
    }
}
