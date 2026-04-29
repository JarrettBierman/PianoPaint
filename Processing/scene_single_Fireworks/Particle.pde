class Particle {
  PVector pos;
  PVector vel;
  float lifespan;
  float diameter;
  color c;
  
  float RAND_OFFSET = 0.8;
  
  Particle(float x, float y, float vx, float vy, color c) {
    pos = new PVector(x, y);
    vx += random(-RAND_OFFSET, RAND_OFFSET);
    vy += random(-RAND_OFFSET, RAND_OFFSET);
    vel = new PVector(vx, -vy);
    lifespan = 255;
    diameter = 7 + random(-3, 3);
    this.c = c;
  }
  
  void update() {
    pos.add(vel);
    lifespan -= 1.25;
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
