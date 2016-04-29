using System;

using Xamarin.Forms;

namespace BeaconBackgroundService
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new ContentPage {
				Content = new StackLayout {
					VerticalOptions = LayoutOptions.Center,
					Children = {
						new Label {
							XAlign = TextAlignment.Center,
							Text = "Beacons in Background Monitoring Test"
						}
					}
				}
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts

			// Start beacon service
			var beaconService = DependencyService.Get<IAltBeaconService>();
			beaconService.InitializeService();
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

