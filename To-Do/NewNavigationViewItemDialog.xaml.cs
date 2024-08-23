using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using To_Do.Models;
using CommunityToolkit.WinUI.Collections;

namespace To_Do
{
    public sealed partial class NewNavigationViewItemDialog : ContentDialog
    {
        public CustomResult _CustomResult { get; set; }
        public ElementTheme THEME;
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public IconData defaultIcon = new IconData()
        {
            Name = "List",
            Code = "EA37",
            Tags = new string[0]
        };

        public IconData SelectedItem
        {
            get { return (IconData)GetValue(SelectedItemProperty); }
            set
            {
                SetValue(SelectedItemProperty, value);
            }
        }
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(IconData), typeof(NewNavigationViewItemDialog), new PropertyMetadata(null));

        public NewNavigationViewItemDialog()
        {
            this.InitializeComponent();
            THEME = ThemeHelper.ActualTheme;
            _CustomResult = CustomResult.Nothing;
            SelectedItem = defaultIcon;
        }

        [System.Obsolete]
        void Load()
        {
            var collection = new IncrementalLoadingCollection<IconsDataSource, IconData>(itemsPerPage: 30);
            this.IconsItemsView.ItemsSource = collection;
            this.IconsItemsView.SelectedIndex = 0;
            DataContext = collection;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            this.OKButton.IsEnabled = !string.IsNullOrEmpty(ListNameTextBox.Text) && !string.IsNullOrWhiteSpace(ListNameTextBox.Text);
        }

        private void NewListTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    if (this.OKButton.IsEnabled)
                    {
                        //store values
                        localSettings.Values["NEWlistName"] = ListNameTextBox.Text;
                        localSettings.Values["NEWlistIcon"] = SelectedItem.Character;

                        ListNameTextBox.Text = string.Empty;
                        _CustomResult = CustomResult.OK;
                        this.Hide();
                    }
                    break;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values["NEWlistName"] = ListNameTextBox.Text;
            localSettings.Values["NEWlistIcon"] = SelectedItem.Character;
            ListNameTextBox.Text = string.Empty;
            _CustomResult = CustomResult.OK;
            this.Hide();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _CustomResult = CustomResult.Cancel;
            this.Hide();
        }

        private void BasicGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SelectedItem = (IconData)e.ClickedItem;
            this.iconChoosingFlyout.Hide();
            this.IconsItemsView.ItemsSource = null;
        }

        private void OnOpeningFlyout(object sender, RoutedEventArgs e)
        {
            Load();
        }
    }
}
