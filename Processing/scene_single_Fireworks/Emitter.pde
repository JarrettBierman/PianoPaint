class Emitter {
  public PVector pos;
  public PVector vel;
  ArrayList<Particle> particles;
  color c;
  boolean on;
  
  Emitter(float x, float y, float vx, float vy) {
    pos = new PVector(x, y);
    vel = new PVector(vx, -vy);
    particles = new ArrayList<Particle>();
    c = color(random(180), random(180), random(180));
    on = false;
  }
  
  Emitter(float x, float y) {
    pos = new PVector(x, y);
    vel = new PVector(0, 0);
    particles = new ArrayList<Particle>();
    c = color(random(200), random(200), random(200));
    on = false;
  }
  
  void setOn(boolean value) {
    on = value;
  }
  
  void setAttributes(float vx, float vy) {
    vel = new PVector(vx, vy);
  }
  
  void emit(int num) {
    for(int i = 0; i < num; i++) {
      particles.add(new Particle(pos.x, pos.y, vel.x, vel.y, this.c));
    }
  }
  
  void update() {
    for (Particle p : particles) {
      PVector gravity = new PVector(0, 0.2);
      p.applyForce(gravity);
      p.update();
    }
    for(int i = 0; i < particles.size(); i++) {
      if(particles.get(i).finished()) {
        particles.remove(i);
      }
    }        
  }
  
  void show() {
    for (Particle p : particles) {
      p.show();
    }
  }
}
