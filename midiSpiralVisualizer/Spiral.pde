PVector pos;

class Spiral {
  ArrayList<Particle> particles;
  
  Spiral(float x, float y) {
    pos = new PVector(x, y);
    particles = new ArrayList<Particle>();
  }
  
  void update() {
    for (Particle p : particles) {
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
  void emit(int num) {
    for(int i = 0; i < num; i++) {
      //particles.add(new Particle(pos, vel.add(PVector.random2D()).mult(random(-0.1, 0.1)), c));
      particles.add(new Particle(pos.x, pos.y));
    }
  }
}
