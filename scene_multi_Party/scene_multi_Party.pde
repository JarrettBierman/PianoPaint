import fisica.*; //<>// //<>//
import themidibus.*;
import java.util.ListIterator;

MidiBus myBus;
//Emitter[] emitters;
//ArrayList<Confetti> confettis;
ArrayList<Comet> comets;

FWorld world;


int WINDOW_SIZE = 800;
int LOW_NOTE = 21;
int HIGH_NOTE = 109;

int LOW_CUTOFF = 53;
int HIGH_CUTOFF = 77;

String PORT_NAME = "Bus 1";


// DRUM CONSTANTS
int KICK = 44;
int SNARE = 45;
int TOMRACK = 46;
int TOMFLOOR = 47;
int HIHAT = 51;
int RIDE = 50;
int CRASH = 49;
int SHAKER = 48;


void setup() {
  size(800, 800);
  
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  Fisica.init(this);
  world = new FWorld();
  world.setGravity(0, 0);
  
  //emitters = new Emitter[HIGH_NOTE - LOW_NOTE];
  //confettis = new ArrayList<Confetti>();
  comets = new ArrayList<Comet>();
    
  //for(int i = 0; i < HIGH_NOTE - LOW_NOTE; i++) {
  //  int note = LOW_NOTE + i;
  //  if(note < LOW_CUTOFF){
  //    emitters[i] = new Emitter(0, map(note, LOW_NOTE, LOW_CUTOFF, 0, height));
  //  }
  //  else if(note > HIGH_CUTOFF) {
  //    emitters[i] = new Emitter(width, map(note, HIGH_CUTOFF, HIGH_NOTE, height, 0));
  //  }
  //  else {
  //    emitters[i] = new Emitter(map(note, LOW_CUTOFF, HIGH_CUTOFF, 10, width-10), height);
  //  }
  //}
}

void draw() {
  background(255);
  
  world.step();
  world.draw();
  
  //for (int i = 0; i < emitters.length; i++) {
  //  if(emitters[i].on) {
  //    emitters[i].emit(2);
  //  }
  //  emitters[i].show();
  //  emitters[i].update();
  //}
  
  //for(int j = 0; j < confettis.size(); j++) {
  //  confettis.get(j).update();
  //  confettis.get(j).show();
  //  if(confettis.get(j).finished()) {
  //      confettis.remove(j);
  //    }
  //}
  
  for(int k = 0; k < comets.size(); k++) {
    comets.get(k).updateAndDraw();
    if(comets.get(k).lifeSpan <= 0) {
      comets.remove(k);
    }
  }
  
}

void noteOn(int channel, int pitch, int velocity) {
  if(channel == 0) {
    //DRUMS
    if(pitch == KICK) {
      comets.add(new Comet(40, height - 40));
    }
    else if(pitch == SNARE) {
      comets.add(new Comet(width/2, height/2));
    }
    else if(pitch == HIHAT) {
      //comets.add(new Comet(width-40, 40));
      AddLine(0, 600, velocity);
    }
    
  }
  else if(channel == 1) {
    //BASS
  }
  else if(channel == 2) {
    //KEYS
  }
  else if(channel == 3) {
    //SYNTH 1
  }
  else if(channel == 4) {
    //SYNTH 2
  }
  println(channel, pitch, velocity);
}

void noteOff(int channel, int pitch, int velocity) {
  if(channel == 0) {
    //DRUMS
  }
  else if(channel == 1) {
    //BASS
  }
  else if(channel == 2) {
    //KEYS
  }
  else if(channel == 3) {
    //SYNTH 1
  }
  else if(channel == 4) {
    //SYNTH 2
  }
}

// Updated AddLine function with more explicit physics properties
void AddLine(float x, float y, float input) {
    float maxAngle = 0.1;
    float lineSize = 50;
    float yOffset = map(input, 0, 128, -maxAngle, maxAngle);
    
    // Create a physical rectangle instead of a line
    // FLines can be tricky with physics behavior
    FBox box = new FBox(lineSize, 5);
    box.setPosition(x, y);
    box.setRotation(yOffset);
    
    // Physics properties
    box.setStatic(false);
    box.setDamping(0);
    box.setRestitution(0.5);
    box.setDensity(10000);
    
    // Appearance
    box.setNoStroke();
    box.setFill(0, 10, 150);
    
    // Set velocity - positive X to move right
    box.setVelocity(500, 0);
    
    world.add(box);
}
