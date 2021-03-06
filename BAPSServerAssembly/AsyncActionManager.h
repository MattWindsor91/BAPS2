#pragma once
#include "LibraryTrack.h"
#include "decodeStructs.h"

namespace BAPSServerAssembly
{
	/**
		This class provides a method of running non critical actions in the background.
		Actions are queued up and then dispatched as and when the dispatch thread reaches them.
		There is no guarantee when an action will be dispatched, but all action updates
		should have bounded timeouts.
	**/

	ref struct PlaybackEventInfo
	{
		PlaybackEventInfo(System::String^ _filename, int _channel, bool _isStartEvent)
			:filename(_filename), trackid(-1), isStartEvent(_isStartEvent), channel(_channel)
		{}
		PlaybackEventInfo(System::String^ _filename, int _channel, bool _isStartEvent, int _trackid)
			: filename(_filename), trackid(_trackid), isStartEvent(_isStartEvent), channel(_channel)
		{}
		System::String^ filename;
		int trackid;
		bool isStartEvent;
		int channel;
	};

	ref class AsyncActionManager
	{
	public:
		static void initAsyncActionManager()
		{
			introPositions = gcnew System::Collections::Generic::Dictionary<int, int>;
			playbackEvents = gcnew System::Collections::Generic::List<PlaybackEventInfo^>;
			asyncActionMutex = gcnew System::Threading::Mutex();
			asyncDataMutex = gcnew System::Threading::Mutex();
			/** In 300ms run updateClients every 300ms **/
			updateInProgress = false;
		}
		static void start()
		{
			System::Threading::TimerCallback^ tc = gcnew System::Threading::TimerCallback(&AsyncActionManager::doActions);
			asyncActionTimer = gcnew System::Threading::Timer(tc,
													   nullptr,
													   5000,
													   5000);
		}
		static void closeAsyncActionManager()
		{
			/** Set the timer to never execute its delegate **/
			if (asyncActionTimer != nullptr)
			{
				asyncActionTimer->Change(System::Threading::Timeout::Infinite,System::Threading::Timeout::Infinite);
				asyncActionTimer->~Timer();
				asyncActionTimer = nullptr;
			}
		}

		static void doActions(System::Object^ name);
		static void saveServerState();
		static void addIntroPosition(int trackid, int introPosition);
		static void logPlayEvent(int channel, Track^ track);
		static void logStopEvent(int channel, Track^ track);

	private:
		static System::Collections::Generic::Dictionary<int, int>^ introPositions;
		static System::Collections::Generic::List<PlaybackEventInfo^>^ playbackEvents;
		static System::Threading::Mutex^ asyncActionMutex;
		static System::Threading::Mutex^ asyncDataMutex;
		static System::Threading::Timer^ asyncActionTimer;
		static bool updateInProgress;
	};
};