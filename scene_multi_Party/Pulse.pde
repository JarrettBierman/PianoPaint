public class Pulse {
  float x;
  float y;
  float s;
  color c;
  float alpha;
  float speed;
  
  public Pulse(float x, float y, float s, color c) {
    this.x = x;
    this.y = y;
    this.s = s;
    this.c = c;
    speed = 10;
  }
  
  public void update() {
    alpha -= speed;
    if(alpha <= 0) {
      alpha = 0;
    }
  }
  
  public void lightUp() {
    alpha = 255;
  }
  
  public void draw() {
    fill(c, alpha);
    noStroke();
    ellipse(x, y, s, s);
  }
  
}
