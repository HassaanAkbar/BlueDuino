using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace BlueDuino.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public sealed partial class FeedbackPage : Page
    {

        public FeedbackPage()
        {
            this.InitializeComponent();
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            PackageVersion version = Package.Current.Id.Version;

            string output = string.Format(
                               "Name: BlueDuino\n" +
                               "Version: {0}.{1}.{2}\n" +
                               "HassaanAkbar.obj" ,
                               version.Major, version.Minor, version.Build);

            about.Text = output;
            
        }

    }
}
