public class Confetti extends Particle {
  
  float angle;
  
  Confetti(float x, float y, color c, float diameter, float lifeRate) {
    super(x, y, 0, 0, c, random(10, 80), lifeRate);   
    angle = random(0, 8);
    this.lifeRate = lifeRate;
  }
  
  @Override void show(){
    stroke(c, lifespan);
    strokeWeight(1);
    fill(c, lifespan);
    pushMatrix();
    translate(this.pos.x, this.pos.y);
    rotate(angle);
    square(0, 0, diameter);
    popMatrix();
  }
  
  @Override void update() {
    diameter += 0.1;
    super.update();
  }
}
