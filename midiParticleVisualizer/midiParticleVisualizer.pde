import themidibus.*;
import javax.sound.midi.MidiMessage;
import java.util.ListIterator;


MidiBus myBus;
Emitter[] emitters;
//Emitter e;
int WINDOW_SIZE = 800;
int LOW_NOTE = 21;
int HIGH_NOTE = 109;

int LOW_CUTOFF = 53;
int HIGH_CUTOFF = 77;

String PORT_NAME = "Bus 1";

void setup() {
  size(700, 1000);
  //fullScreen();
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

void midiMessage(MidiMessage m){
  
  int lowerCutoff = 53; //<>//
  int upperCutoff = 77;
  
  int status = m.getStatus();
  int note = m.getMessage()[1];
  int velocity = m.getMessage()[2];
  
  int index = note - LOW_NOTE;
  
  float vx, vy;
  
  // debugging
  //println("Note: " + note + "\tVelocity: " + velocity + "\tStatus: " + status);
  
  if(status == 144) { // note on
    if(note < lowerCutoff) { // lower third
      vx = map(velocity, 10, 90, 1, 13);
      vy = map(velocity, 10, 90, 1, 13);
    }
    else if(note > upperCutoff) { // upper third
      vx = map(velocity, 10, 90, 1, -13);
      vy = map(velocity, 10, 90, 1, 13);
    }
    else { // middle third
      vx = random(-2,2);
      vy = map(velocity, 10, 90, 1, 18);
    } //<>//
    
    emitters[index].setAttributes(vx, vy);
    emitters[index].setOn(true);
  }
  
  if(status == 128) { // note off
    emitters[index].setOn(false);
  }
}
