import themidibus.*;
import javax.sound.midi.MidiMessage;
import fisica.*;


FWorld world;
MidiBus myBus;

float bounds = 40;
ArrayList<Lever> levers;

void setup() { 
  //size(700, 1000);
  fullScreen();
  colorMode(HSB, 360, 100, 100, 100);
  
  myBus = new MidiBus();
  myBus.registerParent(this);
  myBus.addInput("Bus 1");
  
  Fisica.init(this);
  world = new FWorld();
  world.setGravity(0, 800);
  levers = new ArrayList<Lever>();
  
  createChutes();
}

void draw() {
  background(0, 0, 100);
  world.step();
  world.draw();
  
  stroke(0);
  
  line(0, height - 190, width, height - 190);
  
  for(Object body : world.getBodies()) {
    if(body instanceof FCircle) {
      FCircle ball = (FCircle)body;
      // check if the ball is at rest at the designated height
      if(abs(ball.getVelocityX()) < 0.1 &&
         abs(ball.getVelocityY()) < 0.1 &&
         ball.getY() <= height - 190 &&
         ball.getY() > 500) {
           // get the lever that the ball is in
           for(float i = 0; i < 1; i += 0.125) {
             if(ball.getX() >= width * i && ball.getX() <= width * (i + 0.125)) {
               float val = i * 8;
               levers.get(int(val)).dissapear();
             }
           }
         }
               
      // check if it is OB
      if(ball.getY() > height * 1.5) {
        world.remove(ball);
      }
    }
  }
  
  for(Lever l : levers) {
    if(l.readyToAppear(3000)) {
      l.appear();
    }
  }
}

void noteOn(int channel, int pitch, int velocity) {
  // Receive a noteOn
  //println("Channel: ", channel, "Pitch: ", pitch, "Velocity: ", velocity);
  addBall(pitch, velocity);
}

void noteOff(int channel, int pitch, int velocity) {
  // Receive a noteOff
}

void controllerChange(int channel, int number, int value) {
  // Receive a controllerChange
}


void createChutes() {
  
  // create the walls
  createPlatform(0, height/2, 3, height, 0);
  createPlatform(width, height/2, 3, height, 0);
  
  // create the spikes
  for(float i = 0; i <= 1; i+=0.125) {
    createSpike(width * i, 200, 20);
  }
  for(float i = 0; i <= 1; i+=0.25) {
    createSpike(width * i, 300, 40);
  }
  for(float i = 0; i <= 1; i+=0.125) {
    createSpike(width * i, 400, 20);
  }
  for(float i = 0; i <= 1; i+=0.33){
    createSpike(width * i, 500, 120);
  }
  for(float i = 0; i <= 1; i+=0.25) {
    createSpike(width * i, 600, 60);
  }
  for(float i = 0; i <= 1; i+=0.125) {
    createSpike(width * i, 700, 20);
  }
  
  // create the boxes
  for(float i = 0; i <= 1; i += 0.125) {
    createPlatform(width * i, height - 100, 2, 200, 0);
    levers.add(new Lever(width * i, height, width * (i + 0.125), height, world));
  }
}

void createSpike(float x, float y, float l) {
  createPlatform(x - l * 0.35, y, 3, l, PI/4);
  createPlatform(x + l * 0.35, y, 3, l, -PI/4);
}

void createFunnel(float x, float y, float l, float w) {
  float angle = map(w, 0, 100, PI/4, 0);
  createPlatform(x - l * 0.35 - w, y, 3, l, -angle);
  createPlatform(x + l * 0.35 + w, y, 3, l, angle);
}

FBox getPlatform(float x, float y, float w, float h, float angle) {
  FBox platform = new FBox(w, h);
  platform.setPosition(x, y);
  platform.setRotation(angle);
  platform.setStatic(true);
  platform.setFriction(0.2);
  platform.setRestitution(0.3);
  platform.setFill(0);
  return platform;
}

void createPlatform(float x, float y, float w, float h, float angle) {
  FBox platform = getPlatform(x, y, w, h, angle);
  world.add(platform);
}

void addBall(float pitch, float vel) {
  float s = map(vel, 0, 127, 0, 50);
  float x = map(pitch, 21, 109, bounds, width-bounds);
  float vx = random(-300, 300);
  float vy = map(vel, 0, 127, 500, 100);
  
  FCircle ball = new FCircle(s);
  ball.setPosition(x, -s);
  ball.setVelocity(vx, vy);
  
  ball.setRestitution(0.7);
  ball.setFriction(0.1);
  //ball.setFillColor(getGradientColor());
  ball.setFillColor(getRainbow());
  ball.setNoStroke();
  ball.setDamping(0.2);
  
  ball.setDensity(s*s);
  
  world.add(ball);
}

color getGradientColor() {
  color from = color(#32E3DE);
  color to = color(#ADE332);
  float speed = 0.5;
  float lerpVal = map(sin( 0.002 * speed * millis()), -1, 1, 0, 1);
  return lerpColor(from, to, lerpVal);
}

color getRainbow() {
  float val = (0.1 * millis()) % 400;
  println(val);
  return color(val);
}

void keyPressed() {
  if(key == ' ') {
    println("SPACE BAR HIT");
    for(Lever l : levers) {
      l.dissapear();
    }
  }
}
