import themidibus.*; //<>//
import processing.video.*;

MidiBus myBus;

ArrayList<BigLetter> bigLetters;
ArrayList<Note> notes;

int bounds = 100;

int cLydianScale[] = {  36, 38, 40, 42, 43, 45, 47, 
                        48, 50, 52, 54, 55, 57, 59, 
                        60, 62, 64, 66, 67, 69, 71,
                        72, 74, 76, 78, 79, 81, 83
                     };
Movie movie;

void setup() {
  fullScreen();
  
  colorMode(HSB, 360, 100, 100);
  
  textAlign(CENTER, CENTER);
  
  MidiBus.list();
  
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1"); //<>//
  
  bigLetters = new ArrayList<BigLetter>();
  notes = new ArrayList<Note>();
    
  movie = new Movie(this, "backgroundvideo.mov");
  movie.loop();
  
}

void draw() {
  
  // set background
  background(0); // Clear the screen
  image(movie, 0, 0);
  
  // show, update, and remove dead letters
  for(int i = 0; i < bigLetters.size(); i++) {
    bigLetters.get(i).update();
    bigLetters.get(i).show();
    if(bigLetters.get(i).lifespan <= 0) {
      bigLetters.remove(i);
    }
  }
  
  // update and remove dead notes
  for(int j = 0; j < notes.size(); j++) {
    notes.get(j).update();
    if(notes.get(j).finished == true) {
      notes.remove(j);
    }
  }
}

void keyPressed() {
  handleLetterAdd(key);
  handleNoteAdd(key);
}

void handleLetterAdd(char k) {
  if(validKey(k)) {
    float xPos = random(bounds, width-bounds);
    float yPos = random(bounds, height-bounds);
    float size = 100;
    color randColor = color(random(360), random(70, 100), random(70, 100));
    bigLetters.add(new BigLetter(k, xPos, yPos, size, randColor));
  }
}

void handleNoteAdd(char k) {
  println(k);
  int pitch = 0;  
  if(k == 'I' || k == 'i') {
    pitch = 60;
  }
  else if(k == ' ') {
    pitch = 72;
  }
  else if(k == 'W' || k == 'w') {
    pitch = 59;
  }
  else if(k == 'S' || k == 's') {
    pitch = 71;
  }
  else if(k == 'H' || k == 'h') {
    pitch = 64;
  }
  else if(validKey(k)) {
    pitch = cLydianScale[int(random(cLydianScale.length))];
  }
  if(pitch > 0){
    notes.add(new Note(myBus, pitch, 100));
  }
}

boolean validKey(char k) {
  return  (k >= 'a' && k <= 'z') || 
          (k >= 'A' && k <= 'Z') ||
          (k >= '0' && k <= '9') ||
          k == '.' ||
          k == '?' ||
          k == '!' ||
          k == ',' ||
          k == '\'';
}

void movieEvent(Movie movie) {
  movie.read();
}
