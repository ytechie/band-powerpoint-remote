using Xamarin.Forms;

namespace BandRemoteApp
{
    public class App : Xamarin.Forms.Application
    {
        private BandManager _bandManager;


        public App()
        {
            MainPage = new ContentPage
            {
                Content = new StackLayout
                {
                    VerticalOptions = LayoutOptions.Center,
                    Spacing = 40,
                    Children = {
                        new Label {
                            XAlign = TextAlignment.Center,
                            Text = "This app allows you to control PowerPoint with your Microsoft Band"
                        },
                        new StackLayout
                        {
                            Children =
                            {
                                new Label
                                {
                                    Text = "1. Ensure that your band is paired with this phone."
                                },
                                new Label
                                {
                                    Text = "2. Install the Band PowerPoint Remote app from the Office store."
                                },
                                new Label
                                {
                                    Text = "3. Enter the code shown in PowerPoint."
                                }
                            }
                        },
                        new StackLayout
                        {
                            Children =
                            {
                                new Label
                                {
                                    Text = "Pairing Code:"
                                },
                                new Entry
                                {
                                    Keyboard = Keyboard.Numeric
                                }
                            }
                        }
                    }
                }
            };
        }

        //These are not working. No idea why.

        protected override void OnStart()
        {
            _bandManager = new BandManager();
            var result = _bandManager.StartBandMonitor().Result;
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
