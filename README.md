<a href="http://commandpost.io/" ><img src="https://github.com/CommandPost/CommandPost-Website/blob/master/assets/images/CommandPost_512x512@2x.png" align="right"  width="15%" height="15%" /> </a>

# [CommandPost](http://commandpost.io/)
[![Latest-Release](https://img.shields.io/github/v/release/CommandPost/CommandPost?include_prereleases)](https://github.com/CommandPost/CommandPost/releases) [![Platform](https://img.shields.io/badge/platform-MacOS-lightgrey.svg)](https://commandpost.io/#system-requirements) [![Website](https://img.shields.io/website?down_message=offline&up_message=online&url=https%3A%2F%2Fcommandpost.io)](https://commandpost.io/)
> Workflow Enhancements for Creatives

## Loupedeck Plugin for CommandPost

This repository contains a Loupedeck Plugin written in C#, that allows you to connect CommandPost to the official Loupedeck software using the [Loupedeck API](https://github.com/Loupedeck/LoupedeckPluginSdk4).

Once installed you can then trigger a variety of CommandPost actions directly within the Loupedeck software.

This plugin will eventually be bundled within CommandPost, so you should never have to download it directly from this repository - as we'll include a compiled version within CommandPost.

## Installing the Loupedeck Plugin:

You can download the latest beta release of CommandPost with the Loupedeck Plugin [here](https://github.com/CommandPost/LoupedeckPlugin/releases).

You can find general information on how to install CommandPost [here](https://help.commandpost.io/getting-started/installation).

Once CommandPost is installed click on the CommandPost menubar icon, then select "Control Surfaces > Loupedeck Plugin".

![CommandPost Menubar](Documentation/Images/01-menubar.png)

This will open the Loupedeck Plugin panel. You can then click the "Enable Loupedeck Plugin" checkbox to enable the plugin.

![Loupedeck Plugin Panel](Documentation/Images/02-ui.png)

When installing the Loupedeck Plugin for the first time, or if the Loupedeck Plugin has been updated, CommandPost will restart both the Loupedeck service application and the LoupedeckConfig application.

Once installed, you can access the CommandPost actions from the CommandPost icon within the LoupedeckConfig application.

![LoupedeckConfig](Documentation/Images/03-loupedeck.png)

## Known Issues:

- Depending on how many effects, transitions, generators and titles you have installed on your system, LoupedeckConfig can be quite slow at loading the list of CommandPost actions.
- Currently changing the language via the LoupedeckConfig menubar won't automatically update the CommandPost actions. You'll need to restart the LoupedeckConfig application for UI to update.
- Despite the UI saying "if you enable the Loupedeck Plugin, CommandPost's native Loupedeck support will be disabled" - we don't actually currently check for this. This will be fixed in a future beta.

## What is CommandPost?

CommandPost is a **free** and [open source](https://github.com/CommandPost/CommandPost/blob/develop/LICENSE.md) macOS application that bridges between control surfaces and software that doesn’t support them natively, such as Apple’s [Final Cut Pro](https://www.apple.com/final-cut-pro/) and Adobe’s [After Effects](https://www.adobe.com/products/aftereffects.html).

It’s been downloaded over [114 thousand times](https://hanadigital.github.io/grev/?user=commandpost&repo=commandpost), and there are over [2.3 thousands members](https://www.facebook.com/groups/commandpost/members) in our [Facebook Community](https://www.facebook.com/groups/commandpost/).

It’s been translated into Arabic, Bengali, Catalan, Chinese (Simplified), Chinese (Traditional), Danish, Dutch, French, German, Greek, Hindi, Hungarian, Italian, Japanese, Korean, Malayalam, Norwegian, Panjabi/Punjabi, Polish, Portuguese, Russian, Spanish, Swedish, Ukrainian & Vietnamese by our awesome community.

It’s used by filmmakers, developers, scientists and macOS power users all over the world to seriously speed up mundane tasks through powerful and customisable automation tools.

For example, you can apply individual effects within Final Cut Pro or After Effects with the single tap of a button.

Powered by [Lua](https://dev.commandpost.io/lua/overview/) (the same scripting language used by [Blackmagic Fusion](https://www.blackmagicdesign.com/products/fusion/), [Adobe Lightroom](https://www.adobe.com/au/products/photoshop-lightroom.html) and even parts of [Apple iOS](https://twitter.com/_inside/status/1026173832527265792)), it's insanely customisable and powerful.

It runs natively on Apple Silicon and is fully compatible with [Hammerspoon](http://www.hammerspoon.org).

You can listen to Chris explain CommandPost on Final Cut Pro Radio [Episode #57](http://fcpradio.com/episode057.html) and [Episode #43](http://fcpradio.com/episodes/episode043.html).

## CommandPost's Developer Guide:

You can access the Developer Guide [here](http://dev.commandpost.io/). This includes information on Installation & Usage.
[![Contributors](https://img.shields.io/github/contributors/CommandPost/CommandPost.svg)](https://github.com/CommandPost/CommandPost/graphs/contributors)

You can participate in the translation [here](https://poeditor.com/join/project/QWvOQlF1Sy).
[![Join Translation](https://img.shields.io/badge/POEditor-translate-green.svg)](https://poeditor.com/join/project/QWvOQlF1Sy)

You can also read our Developer Code of Conduct [here](https://github.com/CommandPost/CommandPost/blob/develop/CODE_OF_CONDUCT.md). The source is released under [MIT-License](http://opensource.org/licenses/MIT).
[![GitHub license](https://img.shields.io/badge/License-MIT-yellow.svg)](https://github.com/CommandPost/CommandPost/blob/develop/LICENSE.md)

## Sponsor:

To help continue CommandPost to grow, evolve, and stay completely free and open-source, we offer the ability to [sponsor CommandPost directly through GitHub](https://github.com/sponsors/commandpost).

