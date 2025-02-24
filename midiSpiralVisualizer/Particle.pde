class Particle {
  PVector pos;
  float lifespan;
  float diameter;
  color c;
  
  float circleX;
  float circleY;
  float circleRad;
  
  float RAND_OFFSET = 0.5;
  
  Particle(float x, float y) {
    pos = new PVector(x, y);
    circleX = x;
    circleY = y;
    circleRad = 1;
    lifespan = 2000;
    diameter = 10;
    this.c = color(255, 0, 0);
  }
  
  void update() {
    float angle = degrees(frameCount) * 0.0005 % 360;
    float pointX = circleX + circleRad * cos(angle);
    float pointY = circleY + circleRad * sin(angle);
    pos.x = pointX;
    pos.y = pointY;
    //lifespan -= 1;
    circleRad += 0.05;
  }
  
  boolean finished() {
    return lifespan < 0;
  }
  
  void show() {
    stroke(c, map(lifespan, 0, 2000, 0, 255));
    strokeWeight(1);
    fill(c, map(lifespan, 0, 2000, 0, 255));
    ellipse(pos.x, pos.y, diameter, diameter);
  }
}
