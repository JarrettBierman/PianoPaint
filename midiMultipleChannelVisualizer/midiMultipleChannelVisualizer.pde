import themidibus.*;
import javax.sound.midi.MidiMessage;
import java.util.ListIterator;

MidiBus myBus;
Emitter[] emitters;
ArrayList<Confetti> confettis;
ArrayList<Comet> comets;


int WINDOW_SIZE = 800;
int LOW_NOTE = 21;
int HIGH_NOTE = 109;

int LOW_CUTOFF = 53;
int HIGH_CUTOFF = 77;

String PORT_NAME = "Bus 1";

void setup() {
  size(800, 800);
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  emitters = new Emitter[HIGH_NOTE - LOW_NOTE];
  confettis = new ArrayList<Confetti>();
  comets = new ArrayList<Comet>();
  
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
      emitters[i].emit(2);
    }
    emitters[i].show();
    emitters[i].update();
  }
  
  for(int j = 0; j < confettis.size(); j++) {
    confettis.get(j).update();
    confettis.get(j).show();
    if(confettis.get(j).finished()) {
        confettis.remove(j);
      }
  }
  
  for(int k = 0; k < comets.size(); k++) {
    comets.get(k).updateAndDraw();
    if(comets.get(k).lifeSpan <= 0) {
      comets.remove(k);
    }
  }
  
}

void midiMessage(MidiMessage m){
  
  // STATUSES IN RELATION TO MIDI CHANNEL:
  // CHANNEL  STATUS_ON  STATUS_OFF
  // 1        144        128
  // 2        145        129
  
  // MORE HERE: https://midi.org/expanded-midi-1-0-messages-list 
  
  int lowerCutoff = 53; //<>//
  int upperCutoff = 77;
  
  int status = m.getStatus();
  int note = m.getMessage()[1];
  int velocity = m.getMessage()[2];
  
  int index = note - LOW_NOTE;
  
  float vx, vy;
  
  // debugging
  println("Note: " + note + "\tVelocity: " + velocity + "\tStatus: " + status);
  
  if(status == 144) { // note on
    if(note < lowerCutoff) { // lower third
      vx = map(velocity, 10, 90, 0, 15);
      vy = map(velocity, 10, 90, 0, 15);
    }
    else if(note > upperCutoff) { // upper third
      vx = map(velocity, 10, 90, 0, -15);
      vy = map(velocity, 10, 90, 0, 15);
    }
    else { // middle third
      vx = random(-2,2);
      vy = map(velocity, 10, 90, 0, 23);
    } //<>//
    emitters[index].setAttributes(vx, vy);
    emitters[index].setOn(true);
    emitters[index].c = color(200, 0, 0);
  }
  if(status == 145) { // note on
    confettis.add(new Confetti(
      map(note, 40, 80, 20, width-20),
      300 + random(-5 , 5),
      color(
        map(index, LOW_NOTE, LOW_CUTOFF, 10, 150),
        map(index, LOW_NOTE, LOW_CUTOFF, 255, 100),
        200
      ),
      40,
      4
    ));
  }
  if(status == 146) {
    comets.add(new Comet(map(note, LOW_NOTE, HIGH_NOTE, 20, width-20), 500));
  }
  
  if(status == 128)  emitters[index].setOn(false);
  if(status == 129)  emitters[index].setOn(false);
  if(status == 130)  emitters[index].setOn(false);
}
