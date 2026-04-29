public class LineEmitter {
  float x;
  float y;
  float speed;
  float lineSize;
  float maxAngle;
  
  float prevY;
  
  FWorld world;
  
  public LineEmitter(float x, float y, float speed, float lineSize, FWorld world) {
    this.x = x;
    this.y = y;
    prevY = y;
    this.speed = speed;
    this.lineSize = lineSize;
    this.world = world;
  }
  
  public void AddLine(float input) {
    float yOffset = map(input, 0, 128, -maxAngle, maxAngle);
    FLine line = new FLine(x, prevY, x - lineSize, y + yOffset);
    line.setStatic(false);
    line.setStrokeWeight(5);
    line.setStrokeColor(color(0, 10, 150));
    line.setVelocity(25, 0);
    world.add(line);
    
    prevY = y + yOffset;
  }
}
