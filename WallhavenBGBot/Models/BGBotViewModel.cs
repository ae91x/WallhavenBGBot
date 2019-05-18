using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WallhavenAPI;
using WallhavenBGBot.ConfigurationSections;

namespace WallhavenBGBot.Models
{
    public class BGBotViewModel : INotifyPropertyChanged
    {
        private WallhavenBGBotConfiguration _cfgSection;

        public BGBotViewModel()
        {
            _cfgSection = WallhavenBGBotConfiguration.GetInstance();

            Username = _cfgSection.Username;
            Password = _cfgSection.Password;
            Width = _cfgSection.Width;
            Height = _cfgSection.Height;
            SelectedSort = _cfgSection.SelectedSort;
            SelectedOrder = _cfgSection.SelectedOrder;
            Keywords = _cfgSection.Keywords;
            Interval = _cfgSection.Interval;
            Categories = _cfgSection.Categories;
            Purities = _cfgSection.Purities;
            AutomaticallyCleanAppDataFolder = _cfgSection.AutomaticallyCleanAppDataFolder;
        }

        public void Save()
        {
            _cfgSection.Username = Username;
            _cfgSection.Password = Password;
            _cfgSection.Width = Width;
            _cfgSection.Height = Height;
            _cfgSection.SelectedSort = SelectedSort;
            _cfgSection.SelectedOrder = SelectedOrder;
            _cfgSection.Keywords = Keywords;
            _cfgSection.Interval = Interval;
            _cfgSection.Categories = Categories;
            _cfgSection.Purities = Purities;
            _cfgSection.AutomaticallyCleanAppDataFolder = AutomaticallyCleanAppDataFolder;

            _cfgSection.Save();
        }

        private string _username;
        public string Username { get => _username; set { _username = value; OnPropertyChanged(); } }

        private string _password;
        public string Password { get => _password; set { _password = value; OnPropertyChanged(); } }

        private ushort _width;
        public ushort Width { get => _width; set { _width = value; OnPropertyChanged(); } }

        private ushort _height;
        public ushort Height { get => _height; set { _height = value; OnPropertyChanged(); } }

        private SortMethod _selectedSort;
        public SortMethod SelectedSort { get => _selectedSort; set { _selectedSort = value; OnPropertyChanged(); } }

        private SortOrder _selectedOrder;
        public SortOrder SelectedOrder { get => _selectedOrder; set { _selectedOrder = value; OnPropertyChanged(); } }

        private string _keywords;
        public string Keywords { get => _keywords; set { _keywords = value; OnPropertyChanged(); } }

        private ushort _interval;
        public ushort Interval { get => _interval; set { _interval = value; OnPropertyChanged(); } }

        private bool _automaticallyCleanAppDataFolder;
        public bool AutomaticallyCleanAppDataFolder { get => _automaticallyCleanAppDataFolder; set { _automaticallyCleanAppDataFolder = value; OnPropertyChanged(); } }

        private Category _categories;
        public Category Categories { get => _categories; set { _categories = value; OnPropertyChanged("CategoryGeneral"); OnPropertyChanged("CategoryAnime"); OnPropertyChanged("CategoryPeople"); } }

        public bool CategoryGeneral { get => (_categories & Category.General) == Category.General; set { _categories = value ? Category.General | _categories : _categories & ~Category.General; OnPropertyChanged(); } }
        public bool CategoryAnime { get => (_categories & Category.Anime) == Category.Anime; set { _categories = value ? Category.Anime | _categories : _categories & ~Category.Anime; OnPropertyChanged(); } }
        public bool CategoryPeople { get => (_categories & Category.People) == Category.People; set { _categories = value ? Category.People | _categories : _categories & ~Category.People; OnPropertyChanged(); } }

        private Purity _purities;
        public Purity Purities { get => _purities; set { _purities = value; OnPropertyChanged("PuritySFW"); OnPropertyChanged("PuritySketchy"); OnPropertyChanged("PurityNSFW"); } }

        public bool PuritySFW { get => (_purities & Purity.SFW) == Purity.SFW; set { _purities = value ? Purity.SFW | _purities : _purities & ~Purity.SFW; OnPropertyChanged(); } }
        public bool PuritySketchy { get => (_purities & Purity.Sketchy) == Purity.Sketchy; set { _purities = value ? Purity.Sketchy | _purities : _purities & ~Purity.Sketchy; OnPropertyChanged(); } }
        public bool PurityNSFW { get => (_purities & Purity.NSFW) == Purity.NSFW; set { _purities = value ? Purity.NSFW | _purities : _purities & ~Purity.NSFW; OnPropertyChanged(); } }

        public SortMethod[] Sortings { get => Enum.GetValues(typeof(SortMethod)).Cast<SortMethod>().Select(x => x).ToArray(); }
        public SortOrder[] Orders { get => Enum.GetValues(typeof(SortOrder)).Cast<SortOrder>().Select(x => x).ToArray(); }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName]string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Query GetQuery()
        {
            var query = new Query() {
                Categories = _categories,
                ExactResolution = true,
                Keyword = Keywords,
                Sort = SelectedSort,
                Order = SelectedOrder,
                Page = 1,
                Purities = _purities,
                Resolutions = new string[] { $"{Width}x{Height}" }
            };

            return query;
        }

        public List<string> GetErrors()
        {
            var errors = new List<string>();

            if ((byte)_categories == 0)
                errors.Add("At least one category must be chosen");
            if ((byte)_purities == 0)
                errors.Add("At least one purity must be chosen");
            if (Width == 0 || Height == 0)
                errors.Add("A resolution must be defined");

            return errors;
        }
    }
}
