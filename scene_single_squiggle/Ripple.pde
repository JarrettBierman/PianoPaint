class Ripple {
  PVector pos;
  float growRate;
  float radius;
  float lifeSpan;
  color colorName;
  
  public Ripple(float x, float y, float growRate, color colorName) {
    pos = new PVector(x, y);
    this.growRate = growRate;
    radius = 1;
    lifeSpan = 200;
    this.colorName = colorName;
  }
  
  void update() {
    if(lifeSpan < 30) {
      lifeSpan--;
    }
    else {
      lifeSpan -= 3;
    }
    
    radius += growRate;
  }
  
  void show() {
    fill(colorName, lifeSpan);
    circle(pos.x, pos.y, radius);
  }
}
