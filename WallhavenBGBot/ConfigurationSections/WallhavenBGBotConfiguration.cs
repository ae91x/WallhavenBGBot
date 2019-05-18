using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallhavenAPI;

namespace WallhavenBGBot.ConfigurationSections
{
    public class WallhavenBGBotConfiguration : ConfigurationSection
    {
        private static Configuration _cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static WallhavenBGBotConfiguration GetInstance() => _cfg.Sections["WallhavenBGBotConfiguration"] as WallhavenBGBotConfiguration;

        public void Save()
        {
            _cfg.Save();
        }

        [ConfigurationProperty("Username", DefaultValue = "")]
        public string Username { get => (string)this["Username"]; set { this["Username"] = value; } }

        [ConfigurationProperty("Password", DefaultValue = "")]
        public string Password { get => (string)this["Password"]; set { this["Password"] = value; } }

        [ConfigurationProperty("Width", DefaultValue = (ushort)1920, IsRequired = true)]
        public ushort Width { get => (ushort)this["Width"]; set { this["Width"] = value; } }

        [ConfigurationProperty("Height", DefaultValue = (ushort)1080, IsRequired = true)]
        public ushort Height { get => (ushort)this["Height"]; set { this["Height"] = value; } }

        [ConfigurationProperty("SelectedSort", DefaultValue = SortMethod.Relevance, IsRequired = true)]
        public SortMethod SelectedSort { get => (SortMethod)this["SelectedSort"]; set { this["SelectedSort"] = value; } }

        [ConfigurationProperty("SelectedOrder", DefaultValue = SortOrder.Desc, IsRequired = true)]
        public SortOrder SelectedOrder { get => (SortOrder)this["SelectedOrder"]; set { this["SelectedOrder"] = value; } }

        [ConfigurationProperty("Categories", DefaultValue = Category.General | Category.People, IsRequired = true)]
        public Category Categories { get => (Category)this["Categories"]; set { this["Categories"] = value; } }

        [ConfigurationProperty("Purities", DefaultValue = Purity.SFW, IsRequired = true)]
        public Purity Purities { get => (Purity)this["Purities"]; set { this["Purities"] = value; } }

        [ConfigurationProperty("Keywords", DefaultValue = "")]
        public string Keywords { get => (string)this["Keywords"]; set { this["Keywords"] = value; } }

        public SortMethod[] Sortings { get => Enum.GetValues(typeof(SortMethod)).Cast<SortMethod>().Select(x => x).ToArray(); }
        public SortOrder[] Orders { get => Enum.GetValues(typeof(SortOrder)).Cast<SortOrder>().Select(x => x).ToArray(); }

        [ConfigurationProperty("Interval", DefaultValue = (ushort)60, IsRequired = true)]
        public ushort Interval { get => (ushort)this["Interval"]; set { this["Interval"] = value; } }

        [ConfigurationProperty("AutomaticallyCleanAppDataFolder", DefaultValue = true, IsRequired = true)]
        public bool AutomaticallyCleanAppDataFolder { get => (bool)this["AutomaticallyCleanAppDataFolder"]; set { this["AutomaticallyCleanAppDataFolder"] = value; } }
    }
}
