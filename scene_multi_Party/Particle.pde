public class Particle {
  PVector pos;
  PVector vel;
  float lifespan;
  float lifeRate;
  float diameter;
  color c;
  
  float RAND_OFFSET = 0.75;
  
  Particle(float x, float y, float vx, float vy, color c, float diameter, float lifeRate) {
    pos = new PVector(x, y);
    vx += random(-RAND_OFFSET, RAND_OFFSET);
    vy += random(-RAND_OFFSET, RAND_OFFSET);
    vel = new PVector(vx, -vy);
    lifespan = 255;
    this.c = c;
    this.diameter = diameter;
    this.lifeRate = lifeRate;
  }
  
  Particle(float x, float y, float vx, float vy, color c) {
    this(x, y, vx, vy, c, 10 + random(-6, 6), 1.25);
  }
  
  void update() {
    pos.add(vel);
    lifespan -= lifeRate;
  }
  
  boolean finished() {
    return lifespan < 0;
  }
  
  void applyForce(PVector force) {
    vel.add(force);
  }
  
  void show() {
    stroke(c, lifespan);
    strokeWeight(1);
    fill(c, lifespan);
    ellipse(pos.x, pos.y, diameter, diameter);
  }
}
