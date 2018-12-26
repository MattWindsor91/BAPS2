#pragma once
#include "decodeStructs.h"

namespace BAPSPresenter
{
	/** Delegated for all the various messages to be passed to subforms **/
	delegate void MethodInvokerObj(System::Object^);
	delegate void MethodInvokerStr(System::String^);
	delegate void MethodInvokerStrBool(System::String^, bool);
	delegate void MethodInvokerStrObj(System::String^, System::Object^);
	delegate void MethodInvokerObjStr(System::Object^, System::String^);
	delegate void MethodInvokerObjObj(System::Object^, System::Object^);
	delegate void MethodInvokerObjObjStr(System::Object^,System::Object^, System::String^);
	delegate void MethodInvokerObjObjStrStr(System::Object^,System::Object^, System::String^, System::String^);
	delegate void MethodInvokerObjObjObj(System::Object^,System::Object^, System::Object^);
	delegate void MethodInvokerObjObjObjStr(System::Object^,System::Object^, System::Object^, System::String^);
	delegate void MethodInvokerObjStrStr(System::Object^,System::String^,System::String^);
	delegate void MethodInvokerStrStrStrStr(System::String^,System::String^,System::String^,System::String^);
	delegate void MethodInvokerStrStrStr(System::String^,System::String^,System::String^);

	/** Used for the addfile buttons' tags **/
	public ref struct ChannelDirectoryLookup
	{
		int channel;
		int directory;
		ChannelDirectoryLookup(int _channel, int _directory)
			: channel(_channel), directory(_directory){};
	};
	/** Used for the operation buttons' tags **/
	public ref struct ChannelOperationLookup
	{
		int channel;
		Command co;
		ChannelOperationLookup(int _channel, Command _co)
			: channel(_channel), co(_co){};
	};

	#define PLAYLIST_MOVEUP			0
	#define PLAYLIST_MOVEDOWN		1
	#define PLAYLIST_DELETEITEM		2
	/** Used for the up down delete item buttons' tags **/
	public ref struct ChannelPlaylistOperationLookup
	{
		int channel;
		int operation;
		ChannelPlaylistOperationLookup(int _channel, int _operation)
			: channel(_channel), operation(_operation){};
	};

	public ref struct FolderTempStruct
	{
		int fromIndex;
		int fromFolder;
		FolderTempStruct(int _fromIndex, int _fromFolder)
			:fromIndex(_fromIndex), fromFolder(_fromFolder)
		{}
	};

	public ref struct ChannelTimeoutStruct
	{
		int channel;
		int timeout;
		ChannelTimeoutStruct(int _channel, int _timeout)
			:channel(_channel), timeout(_timeout)
		{}
	};
	public ref struct CountDownState
	{
		int channel;
		bool startAt;
		bool running;
		int theTime;
		CountDownState(int _channel)
			:channel(_channel), startAt(false), running(false), theTime(3590)
		{}
	};
};