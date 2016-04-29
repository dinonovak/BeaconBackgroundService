using System;
using AltBeaconOrg.BoundBeacon;
using BeaconBackgroundService.Droid.Services;
using Android.Widget;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Android.App;
using Android.Util;
using Android.Widget;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(AltBeaconService))]
namespace BeaconBackgroundService.Droid.Services
{
	public class AltBeaconService : Java.Lang.Object, IAltBeaconService
	{
		private readonly MonitorNotifier _monitorNotifier;
		private readonly RangeNotifier _rangeNotifier;
		private BeaconManager _beaconManager;

		Region _tagRegion;

		Region _emptyRegion;
		private ListView _list;
		private readonly List<Beacon> _data;

		public AltBeaconService()
		{
			_monitorNotifier = new MonitorNotifier();
			_rangeNotifier = new RangeNotifier();
			_data = new List<Beacon>();
		}

		public BeaconManager BeaconManagerImpl
		{
			get {
				if (_beaconManager == null)
				{
					_beaconManager = InitializeBeaconManager();
				}
				return _beaconManager;
			}
		}

		public void InitializeService()
		{
			_beaconManager = InitializeBeaconManager();
		}

		private BeaconManager InitializeBeaconManager()
		{
			// Enable the BeaconManager 
			BeaconManager bm = BeaconManager.GetInstanceForApplication(Xamarin.Forms.Forms.Context);

			#region Set up Beacon Simulator if testing without a BLE device
			//			var beaconSimulator = new BeaconSimulator();
			//			beaconSimulator.CreateBasicSimulatedBeacons();
			//
			//			BeaconManager.BeaconSimulator = beaconSimulator;
			#endregion

			var iBeaconParser = new BeaconParser();
			//	Estimote > 2013
			iBeaconParser.SetBeaconLayout("m:2-3=0215,i:4-19,i:20-21,i:22-23,p:24-24,d:25-25");
			bm.BeaconParsers.Add(iBeaconParser);

			_monitorNotifier.EnterRegionComplete += EnteredRegion;
			_monitorNotifier.ExitRegionComplete += ExitedRegion;
			_monitorNotifier.DetermineStateForRegionComplete += DeterminedStateForRegionComplete;
			_rangeNotifier.DidRangeBeaconsInRegionComplete += RangingBeaconsInRegion;

			_tagRegion = new AltBeaconOrg.BoundBeacon.Region("myUniqueBeaconId", Identifier.Parse("F0018B9B-7509-4C31-A905-1A27D39C003C"), null, null);
			_emptyRegion = new AltBeaconOrg.BoundBeacon.Region("myEmptyBeaconId", null, null, null);

			//bm.SetBackgroundMode(false);
			bm.Bind((IBeaconConsumer)Xamarin.Forms.Forms.Context);

			return bm;
		}

		public void StartMonitoring()
		{
			BeaconManagerImpl.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds
			BeaconManagerImpl.SetBackgroundBetweenScanPeriod(5000);

			BeaconManagerImpl.SetMonitorNotifier(_monitorNotifier); 
			_beaconManager.StartMonitoringBeaconsInRegion(_tagRegion);
			_beaconManager.StartMonitoringBeaconsInRegion(_emptyRegion);
		}

		public void StartRanging()
		{
			BeaconManagerImpl.SetForegroundBetweenScanPeriod(5000); // 5000 milliseconds
			BeaconManagerImpl.SetBackgroundBetweenScanPeriod(5000);

			BeaconManagerImpl.SetRangeNotifier(_rangeNotifier);
			_beaconManager.StartRangingBeaconsInRegion(_tagRegion);
			_beaconManager.StartRangingBeaconsInRegion(_emptyRegion);
		}

		private void DeterminedStateForRegionComplete(object sender, MonitorEventArgs e)
		{
			Console.WriteLine("DeterminedStateForRegionComplete");

		}

		private void ExitedRegion(object sender, MonitorEventArgs e)
		{
			Console.WriteLine("ExitedRegion");
			Toast.MakeText(Xamarin.Forms.Forms.Context, "ExitedRegion", ToastLength.Long).Show();
			WriteToLocFile ("ExitedRegion");
		}

		private void EnteredRegion(object sender, MonitorEventArgs e)
		{
			Console.WriteLine("EnteredRegion");
			Toast.MakeText(Xamarin.Forms.Forms.Context, "EnteredRegion", ToastLength.Long).Show();
			WriteToLocFile ("EnteredRegion");
		}

		async void RangingBeaconsInRegion(object sender, RangeEventArgs e)
		{
			if(e.Beacons.Count > 0)
			{
				Toast.MakeText(Xamarin.Forms.Forms.Context, "Beacons in range", ToastLength.Long).Show();
				WriteToLocFile ("Beacons in range");
			}
			else
			{
				// unknown
			}
		}


		private void WriteToLocFile(String eventString)
		{
			var path = Android.OS.Environment.ExternalStorageDirectory + Java.IO.File.Separator + "Download";
			string filename = Path.Combine(path, "BeaconServiceLog.txt");

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			using (var streamWriter = new StreamWriter(filename, true))
			{
				String fLine = DateTime.Now.ToString () + ": " + eventString;
				streamWriter.WriteLine(fLine);
				streamWriter.Close ();
			}

		}
	}
}

