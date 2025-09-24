import themidibus.*;
import java.util.HashMap; // import the HashMap class

float inc = 0.02;
int scl = 10;
int cols, rows;
float zoff = 0;
PVector[] flowfield;
ArrayList<Squig> squigs;
ArrayList<Ripple> ripples;

float yoff = 0;
float yinc = 0.01;

MidiBus myBus;

color backgroundColor;
color leftColor;
color rightColor;

void setup() {
  fullScreen(1);
  
  cols = floor(width / scl);
  rows = floor(height / scl);
  flowfield = new PVector[cols * rows];
  squigs = new ArrayList<Squig>();
  ripples = new ArrayList<Ripple>();
    
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  backgroundColor = color(#333333);
  leftColor = color(#48e5c2);
  rightColor = color(#fcfaf9);
  
}

void draw() {
  background(backgroundColor);
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
       
       //stroke(noisyColor.GetColor(), 100);       
       //pushMatrix();
       //translate(x * scl, y * scl);
       //rotate(v.heading());
       //strokeWeight(1);
       //line(0, 0, scl, 0);
       //popMatrix();
       //noStroke();
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
    //squigs.get(i).edges();
    squigs.get(i).show();
    if(squigs.get(i).state == SquigState.DEAD) {
      squigs.remove(i);
    }
  }
}

void noteOn(int channel, int pitch, int velocity) {
  // Receive a noteOn
  float mappedX = map(pitch, 30, 90, 0, width); 
  float randY = noise(yoff) * height + random(-10, 10);
  color randColor = pitch < 54 ? getSlightlyRandomColor(leftColor, 40) : getSlightlyRandomColor(rightColor, 40); 
 
  
  float mappedWeight = map(velocity, 10, 127, 0, 12);
  float mappedSpeed = map(velocity, 10, 120, 0, 15);
  float mappedGrow = map(velocity, 10, 120, 0.5, 3);
  
  ripples.add(new Ripple(mappedX, randY, mappedGrow, randColor));
  squigs.add(new Squig(mappedX, randY, randColor, mappedWeight, mappedSpeed, pitch));
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

color getSlightlyRandomColor(color initialColor, int offset) {
  float dr = random(-offset, offset);
  float dg = random(-offset, offset);
  float db = random(-offset, offset);
  
  float nr = red(initialColor) + dr;
  if (nr < 0) nr = 0; if (nr > 255) nr = 255;
  float ng = green(initialColor) + dg;
  if (ng < 0) ng = 0; if (ng > 255) ng = 255;
  float nb = blue(initialColor) + db;
  if (nb < 0) nb = 0; if (nb > 255) nb = 255;
  
  return color(nr, ng, nb);
}

//PVector getRandomPerimeterLocation() {
//  int rand = int(random(4));
//  if(rand == 0) {
//    return new PVector(random(width), 0);
//  }
//  if(rand == 1) {
//    return new PVector(random(width), height);
//  }
//  if(rand == 2){
//    return new PVector(0, random(height));
//  }
//  return new PVector(width, random(height));
//}
