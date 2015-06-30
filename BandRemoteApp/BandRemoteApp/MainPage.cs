using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace BandRemoteApp
{
    public class MainPage : ContentPage
    {
        private BandManager _bandManager;

        public MainPage()
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
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _bandManager = new BandManager();
            var result = _bandManager.StartBandMonitor().Result;
        }
    }
}
