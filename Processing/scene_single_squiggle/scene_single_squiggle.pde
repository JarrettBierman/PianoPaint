import themidibus.*;
import java.util.HashMap; // import the HashMap class

float inc = 0.02;
int scl = 24;
int cols, rows;
float zoff = 0;
PVector[] flowfield;
ArrayList<Squig> squigs;
ArrayList<Ripple> ripples;

float yoff = 0;
float yinc = 0.01;

color c1;
color c2;

NoisyLerpColor noisyColor;
GradientManager gm;

MidiBus myBus;

void setup() {
  //fullScreen();
  size(1920, 1080);
  
  //colorMode(HSB, 360, 100, 100);
  cols = floor(width / scl);
  rows = floor(height / scl);
  flowfield = new PVector[cols * rows];
  squigs = new ArrayList<Squig>();
  ripples = new ArrayList<Ripple>();
  
  noisyColor = new NoisyLerpColor();
  
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  gm = new GradientManager(300);
}

void draw() {
  background(255);
  yoff += yinc;
  float yoff = 0;
  for (int y = 0; y < rows; y++) {
     float xoff = 0;
     for (int x = 0; x < cols; x++) {
       int index = x + y * cols;
       float angle = noise(xoff, yoff, zoff) * TWO_PI * 4;
       PVector v = PVector.fromAngle(angle);
       v.setMag(1);
       flowfield[index] = v;
       xoff += inc;
       stroke(noisyColor.GetColor(), 100);       
       pushMatrix();
       translate(x * scl, y * scl);
       rotate(v.heading());
       strokeWeight(1);
       line(0, 0, scl, 0);
       popMatrix();
       noStroke();
    }
    yoff += inc;
    zoff += 0.00005;
  }
  
  for(int i = 0; i < ripples.size(); i++){
    ripples.get(i).update();
    ripples.get(i).show();
    if(ripples.get(i).lifeSpan <= -40) {
      ripples.remove(i);
    }
  }
  
  for (int i = 0; i < squigs.size(); i++) {
    squigs.get(i).follow(flowfield);
    squigs.get(i).update();
    squigs.get(i).edges();
    squigs.get(i).show();
    if(squigs.get(i).state == SquigState.DEAD) {
      squigs.remove(i);
    }
  }
  noisyColor.update();
  gm.update();
}

void noteOn(int channel, int pitch, int velocity) {
  // Receive a noteOn
  float mappedX = map(pitch, 35, 88, 30, width - 30);
  float randX = mappedX + random(-30, 30);
  float randY = noise(yoff) * height + random(-50, 50);
  //color randColor = gm.gradient(map(pitch, 30, 90, 0, 1));
  //color randColor = lerpColor(c1, c2, map(pitch, 35, 88, 0, 1));
  color randColor = color(random(50, 256), random(50, 256), random(50, 256));
  //color randColor = lerpColor(color(255, 255, 0), color(0, 0, 255), map(pitch, 40, 90, 0, 1));
  //color randColor = noisyColor.GetMoreRandomColor(30);
 
  
  float mappedWeight = map(velocity, 20, 127, 4, 30);
  float mappedSpeed = map(velocity, 20, 120, 3, 9);
  float mappedGrow = map(velocity, 20, 120, 0.5, 4);
  
  ripples.add(new Ripple(randX, randY, mappedGrow, randColor));
  squigs.add(new Squig(randX, randY, randColor, mappedWeight, mappedSpeed, pitch));
}

void noteOff(int channel, int pitch, int velocity) {
  // Receive a noteOff
  int index = getIndexOfStoppingSquig(pitch);
  if(index >= 0) {
    squigs.get(index).state = SquigState.STAY;
  }
}

int getIndexOfStoppingSquig(int pitch) {
  for (int i = 0; i < squigs.size(); i++) {
    if(squigs.get(i).reference == pitch && squigs.get(i).state == SquigState.GROW) {
      return i;
    }
  }
  return -1;
}

PVector getRandomPerimeterLocation() {
  int rand = int(random(4));
  if(rand == 0) {
    return new PVector(random(width), 0);
  }
  if(rand == 1) {
    return new PVector(random(width), height);
  }
  if(rand == 2){
    return new PVector(0, random(height));
  }
  return new PVector(width, random(height));
}
