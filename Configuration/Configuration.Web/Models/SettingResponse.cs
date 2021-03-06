﻿namespace Configuration.Web.Models
{
    public class SettingResponse
    {
        public string Value { get; set; }
        public string DeepValue { get; set; }
        public string FileOverrideValue { get; set; }
        public string EnvironmentValue { get; set; }
        public string CommandLineValue { get; set; }
        public string SecretValue { get; set; }
        public Settings1 Options { get; set; }
        public Settings1 Snapshot { get; set; }
        public ISettings1 Interface { get; set; }
    }
}