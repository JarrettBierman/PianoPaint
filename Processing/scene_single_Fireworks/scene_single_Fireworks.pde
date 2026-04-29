import themidibus.*;
import javax.sound.midi.MidiMessage;
import java.util.ListIterator;


MidiBus myBus;
Emitter[] emitters;

int LOW_NOTE = 21;
int HIGH_NOTE = 109;

int LOW_CUTOFF = 53;
int HIGH_CUTOFF = 77;

String PORT_NAME = "Bus 1";

void setup() {
  //fullScreen();
  size(1920, 1080);
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  emitters = new Emitter[HIGH_NOTE - LOW_NOTE];
  
  for(int i = 0; i < HIGH_NOTE - LOW_NOTE; i++) {
    int note = LOW_NOTE + i;
    if(note < LOW_CUTOFF){
      emitters[i] = new Emitter(0, map(note, LOW_NOTE, LOW_CUTOFF, 0, height));
    }
    else if(note > HIGH_CUTOFF) {
      emitters[i] = new Emitter(width, map(note, HIGH_CUTOFF, HIGH_NOTE, height, 0));
    }
    else {
      emitters[i] = new Emitter(map(note, LOW_CUTOFF, HIGH_CUTOFF, 10, width-10), height);
    }
  }
}

void draw() {
  background(255);
  for (int i = 0; i < emitters.length; i++) {
    if(emitters[i].on) {
      emitters[i].emit(3);
    }
    emitters[i].show();
    emitters[i].update();
  }
}

void noteOn(int channel, int pitch, int velocity) {
  // Receive a noteOn
  //println("Channel: ", channel, "Pitch: ", pitch, "Velocity: ", velocity);
  int index = pitch - LOW_NOTE;
  float vx, vy;
  
  if(pitch < LOW_CUTOFF) { // lower third
      vx = map(velocity, 10, 90, 1, 13);
      vy = map(velocity, 10, 90, 1, 13);
    }
    else if(pitch > HIGH_CUTOFF) { // upper third
      vx = map(velocity, 10, 90, 1, -13);
      vy = map(velocity, 10, 90, 1, 13);
    }
    else { // middle third
      vx = random(-2,2);
      vy = map(velocity, 10, 90, 1, 18);
    }
    
    emitters[index].setAttributes(vx, vy);
    emitters[index].setOn(true);
}

void noteOff(int channel, int pitch, int velocity) {
  // Receive a noteOff
  int index = pitch - LOW_NOTE;
  emitters[index].setOn(false);
}

void controllerChange(int channel, int number, int value) {
  // Receive a controllerChange
} //<>// //<>//
