using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WallhavenAPI;

namespace WallhavenBGBot.Models
{
    public class BGBotViewModel : ConfigurationSection, INotifyPropertyChanged
    {
        private static Configuration _cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        public static BGBotViewModel ViewModel { get; } = _cfg.Sections["BGBotViewModel"] as BGBotViewModel;

        public static void Save()
        {
            _cfg.Save();
        }

        [ConfigurationProperty("Username", DefaultValue = "")]
        public string Username { get => (string)this["Username"]; set { this["Username"] = value; OnPropertyChanged("Username"); } }

        [ConfigurationProperty("Password", DefaultValue = "")]
        public string Password { get => (string)this["Password"]; set { this["Password"] = value; OnPropertyChanged("Password"); } }

        [ConfigurationProperty("Width", DefaultValue = (ushort)1920, IsRequired = true)]
        public ushort Width { get => (ushort)this["Width"]; set { this["Width"] = value; OnPropertyChanged("Width"); } }

        [ConfigurationProperty("Height", DefaultValue = (ushort)1080, IsRequired = true)]
        public ushort Height { get => (ushort)this["Height"]; set { this["Height"] = value; OnPropertyChanged("Height"); } }

        [ConfigurationProperty("SelectedSort", DefaultValue = SortMethod.Relevance, IsRequired = true)]
        public SortMethod SelectedSort { get => (SortMethod)this["SelectedSort"]; set { this["SelectedSort"] = value; OnPropertyChanged("SelectedSort"); } }

        [ConfigurationProperty("SelectedOrder", DefaultValue = SortOrder.Desc, IsRequired = true)]
        public SortOrder SelectedOrder { get => (SortOrder)this["SelectedOrder"]; set { this["SelectedOrder"] = value; OnPropertyChanged("SelectedOrder"); } }

        [ConfigurationProperty("Categories", DefaultValue = Category.General | Category.People, IsRequired = true)]
        public Category Categories { get => (Category)this["Categories"]; set { this["Categories"] = value; OnPropertyChanged("Categories"); } }

        [ConfigurationProperty("Purity", DefaultValue = Purity.SFW, IsRequired = true)]
        public Purity Purity { get => (Purity)this["Purity"]; set { this["Purity"] = value; OnPropertyChanged("Purity"); } }

        [ConfigurationProperty("Keywords", DefaultValue = "")]
        public string Keywords { get => (string)this["Keywords"]; set { this["Keywords"] = value; OnPropertyChanged("Keywords"); } }

        public SortMethod[] Sortings { get => Enum.GetValues(typeof(WallhavenAPI.SortMethod)).Cast<WallhavenAPI.SortMethod>().Select(x => x).ToArray(); }
        public SortOrder[] Orders { get => Enum.GetValues(typeof(WallhavenAPI.SortOrder)).Cast<WallhavenAPI.SortOrder>().Select(x => x).ToArray(); }

        [ConfigurationProperty("Interval", DefaultValue = (ushort)60, IsRequired = true)]
        public ushort Interval { get => (ushort)this["Interval"]; set { this["Interval"] = value; OnPropertyChanged("Interval"); } }

        public bool CategoryGeneral { get => (Categories & Category.General) == Category.General; set { Categories = value ? Category.General | Categories : Categories & ~Category.General; OnPropertyChanged("CategoryGeneral"); } }
        public bool CategoryAnime { get => (Categories & Category.Anime) == Category.Anime; set { Categories = value ? Category.Anime | Categories : Categories & ~Category.Anime; OnPropertyChanged("CategoryAnime"); } }
        public bool CategoryPeople { get => (Categories & Category.People) == Category.People; set { Categories = value ? Category.People | Categories : Categories & ~Category.People; OnPropertyChanged("CategoryPeople"); } }

        public bool PuritySFW { get => (Purity & Purity.SFW) == Purity.SFW; set { Purity = value ? Purity.SFW | Purity : Purity & ~Purity.SFW; OnPropertyChanged("PuritySFW"); } }
        public bool PuritySketchy { get => (Purity & Purity.Sketchy) == Purity.Sketchy; set { Purity = value ? Purity.Sketchy | Purity : Purity & ~Purity.Sketchy; OnPropertyChanged("PuritySketchy"); } }
        public bool PurityNSFW { get => (Purity & Purity.NSFW) == Purity.NSFW; set { Purity = value ? Purity.NSFW | Purity : Purity & ~Purity.NSFW; OnPropertyChanged("PurityNSFW"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public static Query GetQuery()
        {
            var query = new Query() {
                Categories = ViewModel.Categories,
                ExactResolution = true,
                Keyword = ViewModel.Keywords,
                Sort = ViewModel.SelectedSort,
                Order = ViewModel.SelectedOrder,
                Page = 1,
                Purities = ViewModel.Purity,
                Resolutions = new string[] { $"{ViewModel.Width}x{ViewModel.Height}" }
            };

            return query;
        }

        public static List<string> GetErrors()
        {
            var errors = new List<string>();

            if ((byte)ViewModel.Categories == 0)
                errors.Add("At least one category must be chosen");
            if ((byte)ViewModel.Purity == 0)
                errors.Add("At least one purity must be chosen");
            if (ViewModel.Width == 0 || ViewModel.Height == 0)
                errors.Add("A resolution must be defined");

            return errors;
        }
    }
}
