# Honey Select Buttplug Plugin

Because what's more fun than letting the poke-a-doll poke you back?

## Building

* You'll need Visual Studio 2017 with the .net3.5 toolkit installed.
* You'll also need Honey Select Ultimate from [Fakku](https://www.fakku.net/honey-select-unlimited)
* Clone this repo
* Copy the UnityEngine.dll from Honey Select Unlimited_64_Data\Managed (in wherever you extracted Honey Select) into Libs\Unity5.3
* Open the solution in Visual Studio
* Build the project (Build->Build Solution)

## Install

- You'll need Honey Select Ultimate from [Fakku](https://www.fakku.net/honey-select-unlimited).
- Then you'll need to [IPA (Illusion Plugin Architecture)](https://github.com/Eusth/IPA/releases) to add plugin support: extract the latest release into the Honey Select folder (so IPA.exe is next to Honey Select Unlimited_64.exe), then drag Honey Select Unlimited_64.exe onto IPA.exe to patch the game.

* You should have a Plugins folder now, so all you have to do is copy the HSButtplug library and its dependencies in there (from HoneySelectButtplugPlugin\bin\Debug). 
* That's it, done!

## Usage

* At the moment, the plugin only drives vibrators during H scenes (50% when on a sweet spot, 100% during orgasm).

* It assumes that the Buttplug Server is listening on ws://127.0.0.1:12345/buttplug (the SSL/TLS option must be unchecked)

* To see what the plugin is doing,  try starting Honey Select from the command line with the --verbose argument:

  ```
  cd "c:\Honey Select"
  Honey Select Unlimited_64.exe" --verbose
  ```