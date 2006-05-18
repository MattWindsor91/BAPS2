#include "stdafx.h"
#include "BAPSPresenterMain.h"

using namespace BAPSPresenter;

void BAPSPresenterMain::showChannelOperation(System::Object^ _channel, System::Object^ _operation)
{
	int channel = safe_cast<int>(_channel);
	int operation = safe_cast<int>(_operation);
	switch (operation)
	{
	case BAPSNET_PLAY:
		channelPlay[channel]->Highlighted = true;
		channelPause[channel]->Highlighted = false;
		channelStop[channel]->Highlighted = false;
		break;
	case BAPSNET_PAUSE:
		channelPlay[channel]->Highlighted = false;
		channelPause[channel]->Highlighted = true;
		channelStop[channel]->Highlighted = false;
		break;
	case BAPSNET_STOP:
		channelPlay[channel]->Highlighted = false;
		channelPause[channel]->Highlighted = false;
		channelStop[channel]->Highlighted = true;
		break;
	}
}

void BAPSPresenterMain::showPosition(System::Object^ _channel, System::Object^ _value)
{
	int channel = safe_cast<int>(_channel);
	int value = safe_cast<int>(_value);
	if (channel < 3)
	{
		/** Channels are ready when they have a valid duration **/
		if (trackTime[channel]->Duration >= value)
		{
			/** Set the trackbar position **/
			trackTime[channel]->Position = value;
			value = (u32int)System::Math::Round(value/1000)*1000;
			/** Set the amount of time gone **/
			timeGoneText[channel]->Text = MillisecondsToTimeString(value);
			/** Set the time left **/
			timeLeftText[channel]->Text = MillisecondsToTimeString(trackTime[channel]->Duration - value);
		}
		else
		{
			/** WORK NEEDED: there is a problem **/
		}
	}
}

void BAPSPresenterMain::showVolume(System::Object^ _channel, System::Object^ _value)
{
	int channel = safe_cast<int>(_channel);
	float value = safe_cast<float>(_value);
	if (channel < 3)
	{
		if (volumeBar[channel]->Minimum > (int)(value*100) ||
			volumeBar[channel]->Maximum < (int)(value*100))
		{
			/** WORK NEEDED: if this happens we need the user to know **/
		}
		else
		{
			/** Set the value (*100) **/
			volumeBar[channel]->Value = (int)(value*100);
		}
	}
}

void BAPSPresenterMain::showLoadedItem(System::Object^ _channel, System::Object^ _index, System::Object^ _itemType, System::String^ description)
{
	int channel = safe_cast<int>(_channel);
	int index = safe_cast<int>(_index);
	int itemType = safe_cast<int>(_itemType);
	if (channel < 3)
	{
		trackList[channel]->LoadedIndex = index;
		/** Set the description text **/
		loadedText[channel]->Text = description;
		if (itemType == BAPSNET_VOIDITEM)
		{
			trackTime[channel]->Position = 0;
			trackTime[channel]->Duration = 0;
			trackTime[channel]->CuePosition = 0;
			trackTime[channel]->IntroPosition = 0;
			trackLengthText[channel]->Text = MillisecondsToTimeString(0);
			timeLeftText[channel]->Text = MillisecondsToTimeString(0);
			timeGoneText[channel]->Text = MillisecondsToTimeString(0);
		}
		refreshAudioWall();
	}
}

void BAPSPresenterMain::showDuration(System::Object^ _channel, System::Object^ _value)
{
	int channel = safe_cast<int>(_channel);
	int value = safe_cast<int>(_value);
	if (channel < 3)
	{
		/** Set up the min and max for the trackbar **/
		trackTime[channel]->Position = 0;
		trackTime[channel]->Duration = value;
		/** Set the track length box **/
		trackLengthText[channel]->Text = MillisecondsToTimeString(value);
	}
}


void BAPSPresenterMain::showText(System::Object^ _channel,System::Object^ _index, System::String^ description, System::String^ text)
{
	int channel = safe_cast<int>(_channel);
	int index = safe_cast<int>(_index);
	if (channel < 3)
	{
		trackList[0]->LoadedTextIndex = -1;
		trackList[1]->LoadedTextIndex = -1;
		trackList[2]->LoadedTextIndex = -1;
		trackList[channel]->LoadedTextIndex = index;
		MainTextDisplay->Text = text;
		if (textDialog->Visible)
		{
			MethodInvokerStr^ mi = gcnew MethodInvokerStr(textDialog, &TextDialog::updateText);
			array<System::Object^>^ dd = gcnew array<System::Object^>(1) {text};
			textDialog->Invoke(mi, dd);
		}
	}
}

void BAPSPresenterMain::showCuePosition(System::Object^ _channel, System::Object^ _cuePosition)
{
	int channel = safe_cast<int>(_channel);
	int cuePosition = safe_cast<int>(_cuePosition);
	if (channel < 3)
	{
		trackTime[channel]->CuePosition = cuePosition;
	}
}

void BAPSPresenterMain::showIntroPosition(System::Object^ _channel, System::Object^ _introPosition)
{
	int channel = safe_cast<int>(_channel);
	int introPosition = safe_cast<int>(_introPosition);
	if (channel < 3)
	{
		trackTime[channel]->IntroPosition = introPosition;
	}
}