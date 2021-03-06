Customs Forge Notifier
=================

Notifier for new watched entries from http://search.customsforge.com/

## Purpose

When ran periodically, this app will look for new or updated entries which match artists you are interested in. When a matching entry is found, a notification will be sent. Currently, only Pushbullet is supported for notifications, however, nearly any type of service could be used.

## Installation

1. Download the latest release from the [releases section](https://github.com/JakeH/CustomsForgeNotifier/releases). 
2. Extract zip to desired location on your computer.
3. Edit the `settings.ini` file according to the [Settings Guide](#settings-guide) section.
4. Add `CustomsForgeNotifier.exe` as a scheduled task. See the [Scheduled Task](#scheduled-task) section for one way to accomplish this.

## Settings Guide

There are some settings that you must establish in the `settings.ini` file as well as the `ArtistsToMatch.txt` file before you can run this app.

### ArtistsToMatch.txt

This file needs to contain the list of artists you want to be notified for. Place a single artist per line.

### settings.ini

#### App Section

##### AbsoluteRetrievalLimit

This is the maximum amount of entries that the app will attempt to retrieve from the Customs Forge web service. If the app is ran on a shorter interval (daily), the default limit of 150 will likely never be reached. However, it ran on a longer interval (weekly, monthly), there might be some missed entries if this value should be set higher. 

##### Notifier

If blank, no notification will be sent upon successful update.

The valid non-blank values are: 

* `pushbullet` => Notifications will be sent via Pushbullet

##### LastEntryUpdated

This is the date of the last known entry encountered while monitoring, which is used by the app so it will only process new entries since the last monitor activity. Set this to 0 if you want to reset that functionality.

#### Pushbullet Section

##### APIToken

If you choose to have Pushbullet notification, this must be your API token. This can be found on your 
Pushbullet page https://www.pushbullet.com/account

##### APIUri

Pushbullet API Uri. Should not need to be changed from the default.

##### DeviceIden

Specific device to push to. Use the Pushbullet site to see your device identifiers.

## Scheduled Task

A very easy way to get this to run is to use Window's Task Scheduler. 

The following command (ran in a Command Prompt) will add a task which is ran once every 2 days at 8PM. 
You will need to replace the /TR parameter with the absolute path to where you extracted the app.

``` 
schtasks /Create /RU "SYSTEM" /SC DAILY /MO 2 /ST 20:00 /TN "Customs Forge Notifier" /TR "..\path-to-exe\CustomsForgeNotifier.exe"
```

If you wish to add this task manually, I only suggest that you have it ran under the System account to prevent 
the app window from showing on your desktop when the task executes.


## Notice

There's little error handling in this project. There is a log with minimal information in the app directory. If you have issues 
with this app, please include that file, or the relevant information, when you create a new GitHub issue.

This project is in no way associated with Customs Forge or Rocksmith. Please use at your own risk.
