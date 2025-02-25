# Processing-Midi-Visualizations
 A colletion of Processing projects that connect midi input signals to various interactive visuals

Each of the folders starting with `midi` contains a Processing project that controls physical or virtual visual elements with MIDI input.

The app relies on reading MIDI information from a Virtual MIDI port. The midi is read through the application which causes interraction.

![ALT TEXT](http://img.youtube.com/vi/WwJQb9Wk3rs/maxresdefault.jpg)
![ALT TEXT](http://img.youtube.com/vi/CKqsVY0MZms/maxresdefault.jpg)

## Prerequisites
- Must be run on a computer running Windows or MacOS
- Must have a Digital Audio Worstation (DAW) or other application that can send MIDI information to a virtual port
- Must install and use the [Processing IDE](https://processing.org/)
  - Once in the IDE, must install [TheMidiBus](https://github.com/sparks/themidibus)

## Installation and usage
1. [Set up a Virtual MIDI Port](https://help.ableton.com/hc/en-us/articles/209774225-Setting-up-a-virtual-MIDI-bus)
2. Connect your virtual MIDI port as an OUT through your DAW. [This video](https://youtu.be/3bhiUGFt6as?si=fhiZUNshgyFihL8A) explains how to do it in Ableton Live
3. Download the [Processing IDE](https://processing.org/)
4. Clone this repository, open one of the projects in Processing
5. In the `setup()` function, change the input of the `myBus` MIDI bus to your port name
```
myBus = new MidiBus();
myBus.registerParent(this);
myBus.addInput(YOUR_VIRTUAL_PORT_NAME);
```
6. Ensure you hhave installed TheMidiBus package
7. Add any changes you see fit and run the program!

 ## Demos
[MIDI Particle Visualization Example](https://www.youtube.com/watch?v=bW3E8PGd3CI)

[MIDI Physics Visualization Example](https://www.youtube.com/watch?v=WwJQb9Wk3rs)


`Created by Jarrett Bierman`
