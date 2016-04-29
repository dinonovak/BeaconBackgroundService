using System;

namespace BeaconBackgroundService
{
	public interface IAltBeaconService
	{
		void InitializeService();
		void StartMonitoring();
		void StartRanging();
	}
}