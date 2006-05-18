#include "StdAfx.h"
#include "HelpDialog.h"

using namespace BAPSPresenter;

void HelpDialog::writeText()
{
	faqText->Text="1) My track keeps playing from mid-way through.\n"
				  "     Tracks start playing from the cue point. Try dragging the red bar back to\n"
				  "     the beginning of the track.\n\n"
				  "2) I cannot find a track in the Music Library but I know that it is there.\n"
				  "     There could be 2 reasons for this,\n"
				  "     a) The track has been entered under a different spelling.\n"
				  "     b) The track has not been made into a digital track and cannot be played on BAPS.\n\n"
				  "3) When I play my track after pausing it, it starts from the beginning again.\n"
				  "     This is the correct behaviour, play will always play from the cuepoint, in order\n"
				  "     to continue playing after pausing a track, use the pause button again to un-pause\n\n"
				  "4) Files have been added to the 'xxxx' folder recently but they do not appear in the\n"
				  "   the file list.\n"
				  "     The file lists at the top of the screen need to be refreshed using the named\n"
				  "     buttons above the list in order to see any changes to their contents.\n\n"
				  "5) A new audio file has been added to one of the folders but it does not appear even\n"
				  "   after refreshing the file list\n"
				  "     The file lists only show certain types of audio file not all files.\n\n"
				  "6) One of the tracks in my track list says 'failed to load' after clicking on it.\n"
				  "     This may happen when a file is removed from a folder after you have added it to\n"
				  "     your track list, this is most likely when the track list was created using the \n"
				  "     web interface. Delete the track and try to add a new copy. If this continues \n"
				  "     contact support via the feedback button on the main screen\n\n"
				  "7) I keep getting an error saying '<any error message here>'\n"
				  "     This is undoubtably a bad thing and you should contact support immediately\n\n"
				  "";
	ppsText->Text = "To select a track to be played you simply click on the track name in the track list and it "
					"will get loaded and some information about it will be displayed.\n\n"
					"The 'F' (function) keys are used to control the basic playback functions of Play, Pause and Stop. "
					"Each logical channel is controlled by a set of 4 function keys. The first channel is "
					"controlled by F1-F4, the second channel by F5-F8 and the third by F9-F12. These keys are "
					"normally grouped together on the keyboard so they will be easy to find. The first key in "
					"each group (F1, F5, F9) is the play key for their respective channels. The second key in "
					"each group (F2, F6, F10) is the pause/un-pause key, the third key (F3, F7, F11) is the "
					"stop key.\n\n"
					"When using the 'play key' a track will start from its 'cue-point'. Upon loading a track the "
					"'cue-point' is always set to the start. You may however change the 'cue-point' by dragging the "
					"bottom arrow on the track controller to the desired position.  \n\n"
					"If you wish the track to start from the beginning again drag the red bar on the track controller "
					"back to the beginning.\n\n"
					"In order to get to the desired place in a track, you can drag the black arrow in the track "
					"controller to the desired position.\n\n"
					"If the volume bars are enabled on screen you can use them to set the volume for the channel. "
					"It is suggested that these be left at maximum and the volume be controlled externally.\n\n"
					"Right clicking on a track list gives you the ability to change the following options:\n"
					"1) Automatic Advance\n"
					"-- This will automatically load the next track when the current one has finished\n"
					"2) Play On Load\n"
					"-- This will immediately play a track when it is loaded (1-click play)\n"
					"3) Repeat\n"
					"-- You can set a track to repeat over and over by selecting repeat one, or you can repeat all "
					"tracks in a loop with repeat all. The repeat setting overrides the Automatic Advance and Play "
					"on Load settings\n\n"
					"For convenience the track controller has a second (green) bar that you can move using the top "
					"arrow handle. It is intended that this be used to mark where the lyrics start in a track such "
					"that you can talk up to them. The setting you make will be stored if the track is a (L)ibrary "
					"track.";
	modifyText->Text = "Adding items to a track list:\n"
	  				   "There are 2 ways to add an item to a track list:\n"
					   "1) Drag an item out of any of the top three file lists and drop it in a track list\n"
					   "   (The named buttons above the file lists will refresh the contents of the each list)\n"
					   "2) Use the Music Library to search for a digital track to play (See Music Library help).\n"
					   "\n"
					   "All new items are placed at the end of the track list, you may then move them around in the list. "
					   "You do this by clicking and dragging the item up or down in the list, the track will move to the "
					   "position you drop it at. If you drop the track outside the track list then it will not move.\n"
					   "When you click on an item in the track list (without dragging), the track will be loaded.\n"
					   "\n"
					   "To delete an item from a track list you can either right click on it and select delete or you "
					   "can push delete which will delete the currently loaded track and immediately stop playback\n\n"
					   "All items in a track list have an icon representing what they are:\n"
					   "An 'A' represents an ordinary audio track either from one of your personal collections or from a "
					   "predefined collection.\n"
					   "An 'L' represents a track from the music library\n"
					   "A 'T' represents a Textual entry which may be a news story or a link see";
	searchText->Text = "To add an item to a track list from the record library click on the 'Search record library' button. "
					   "This will open a new window that you can use to search the library.\nSimply enter an artist or "
					   "Title (or both) and click search. It will search the library for tracks CONTAINING the text you "
					   "put in the two search fields (ignoring the CaSe).\nYou will receive no more than 200 results so "
					   "if you receive 200 results it means there are more tracks but you must be more specific to get to "
					   "them.\n To help in searching the library you may use the '%' character as a wildcard. Wildcards are "
					   "automatically added to the beginning and end of your search phrase.\n\n"
					   "'fre%erc' will return songs by the artist freddy mercury as well as any other artists it happens to "
					   "match with.\n\n"
					   "Once you have found a track/tracks you wish to add then select the track in the list and click the "
					   "appropriate button to add it to a track list.\n\n"
					   "When done close the window to continue."
					   "";
	textText->Text =   "Textual entries may be included in a track list if it has been loaded using the Load Show feature.\n\n"
					   "No support is available yet to add textual links using the Presenter application.\n\n"
					   "Textual entries appear in track lists with a 'T' next to them.\n"
					   "Clicking on a (T)extual entry will not affect the audio which is playing in that channel.\n"
					   "When loaded the icon next to the textual entry goes black and the text will appear in the "
					   "bottom section of the interface (when the chat feature is turned off) and will also appear "
					   "in the dedicated textual display window if it is open.\n"
					   "Both areas where the text is loaded are editable, however any changes you make are not saved. Loading "
					   "another textual entry will lose any changes."
					   "";
	loadingText->Text= "To make use of the track list loading functionality, you must either have prepared a show using the "
					   "BAPS website or know details of a show which you would like to use.\n\n"
					   "After clicking on the Load Show button you have 3 choices:\n"
					   "1) Load a show from the currently logged on user's pre-made shows\n"
					   "2) Load a show from the 'system' user's shows\n"
					   "3) Load a show from a named user's shows. Your personal username will be the e-mail address which "
					   "you gave when signing up. If in doubt what this is, log into the BAPS Show Manager website and "
					   "look at the top right of the interface. Your username will be displayed there.\n\n"
					   "You may also select whether you get a list of all shows ever created by the selected user or just "
					   "those which have a broadcast date from now onwards.\n\n"
					   "After clicking the go button a list of shows will appear along with the intended broadcast dates. "
					   "Select a show by left clicking on it. Then click go again.\n\n"
					   "The final dialog is used to show how the different track lists are going to be loaded into the "
					   "'computerised' desk. Each track list will pre-select the channel which it was set to use when the show "
					   "was made. Only one track list can be loaded onto each channel. Before loading the new track list the current "
					   "contents of the track list will be REMOVED.\n\n"
					   "You should check that the music which was available when the show was created is still "
					   "available, by loading each entry in turn and checking that it does not fail to load."
					   "";
}