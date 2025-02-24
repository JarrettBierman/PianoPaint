public class Lever {
  FLine body;
  FWorld world;
  float x1, y1, x2, y2;
  float w = 2;
  boolean visible = true;
  float dissapearTime;
  
  public Lever(float x1, float y1, float x2, float y2, FWorld world) {
    this.x1 = x1;
    this.x2 = x2;
    this.y1 = y1;
    this.y2 = y2;
    this.world = world;
    body = new FLine(x1, y1, x2, y2);
    world.add(body);
  }
  
  public void dissapear() {
    world.remove(body);
    visible = false;
    dissapearTime = millis();
  }
  
  public void appear() {
    world.add(body);
    visible = true;
  }
  
  public boolean readyToAppear(float millisToWait) {
    return !visible && millis() > (dissapearTime + millisToWait);
  }
  
}
