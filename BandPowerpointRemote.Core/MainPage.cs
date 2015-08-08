using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Xamarin.Forms;

namespace BandPowerpointRemote
{
    public class MainPage : ContentPage
    {
        private BandManager _bandManager;
		private int? _pairId;

        public MainPage()
        {
			var pairIdEntry = new Entry
			{
				Keyboard = Keyboard.Numeric
			};
			pairIdEntry.PropertyChanged += (sender, e) => {
				if(pairIdEntry.Text == null || pairIdEntry.Text.Length == 0)
					_pairId = null;
				else
					_pairId = int.Parse(pairIdEntry.Text);
			};

			var nextButton = new Button {
				Text = "Next >",
				Font = Font.SystemFontOfSize(NamedSize.Large),

			};
			nextButton.Clicked += (sender, e) => {
				NextSlide ();
			};
			var prevButton = new Button {
				Text = "< Prev",
				Font = Font.SystemFontOfSize(NamedSize.Large)
			};
			prevButton.Clicked += (sender, e) => {
				PrevSlide ();
			};


            Content = new StackLayout
            {
				Padding = 20,
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
								pairIdEntry
                            }
                        },
						new StackLayout
						{
							Orientation = StackOrientation.Horizontal,
							Children = 
							{
								prevButton, nextButton
							}
						}
                    }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _bandManager = new BandManager(this);
            var result = _bandManager.StartBandMonitor().ConfigureAwait(false);

			_bandManager.MoveNextGesture += (sender, e) => {
				Debug.WriteLine("Wave (next) gesture detected");
				NextSlide();
			};
			_bandManager.MovePrevGesture += (sender, e) => {
				Debug.WriteLine("Wave (prev) gesture detected");
				PrevSlide();
			};
        }

		private void NextSlide()
		{
			if (_pairId != null)
				ProxyClient.NextSlide (_pairId.Value);
		}

		private void PrevSlide()
		{
			if (_pairId != null)
				ProxyClient.PrevSlide (_pairId.Value);
		}
    }
}
