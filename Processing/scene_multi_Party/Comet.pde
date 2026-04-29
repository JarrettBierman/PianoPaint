public class Comet {
  int storage = 60;
  float mx[] = new float[storage];
  float my[] = new float[storage];
  
  float x;
  float y;
  
  float startX;
  float startY;
  
  float step = 0;
  float speed = 2;
  float amplitude;
  float vel1;
  float vel2;
  float vel3;
  
  int mode;
  color c;
  
  float lifeSpan = 500;
  
  Comet(float x, float y) {
    this.x = x;
    this.y = y;
    
    this.startX = x;
    this.startY = y;
    
    for(int i = 0; i < storage; i++) {
      mx[i] = startX;
      my[i] = startY;
    }
    
    mode = int(random(4));
    c = color(random(200), random(200), random(200));
    
    amplitude = random(-100, 100);
    vel1 = random(-8, 8);
    vel2 = random(-8, 8);
    vel3 = random(0.001, 0.1);
  }
  
  void updateAndDraw() {
    noStroke();
    
    int which = frameCount % storage;
    mx[which] = x;
    my[which] = y;
    
    if(mode == 0) {
      x = startX + vel1 * step;
      y = startY + vel2 * step;
    }
    if(mode == 1) {
      x = startX + vel1 * step;
      y = startY + vel2 * step;
    }
    if(mode == 2) {
      x = startX + amplitude * sin(step * vel3);
      y = startY + 4 * step;
    }
    if(mode == 3) {
      x = startX + 4 * step;
      y = startY + amplitude * sin(step * vel3);
    }
    
    step += speed;
    
    for (int i = 0; i < storage; i++) {
      // which+1 is the smallest (the oldest in the array)
      int index = (which+1 + i) % storage;
      fill(c, map(i, 0, storage, 0, 100));
      ellipse(mx[index], my[index], i/2, i/2);
    }
    lifeSpan--;
    
  }
  
}
