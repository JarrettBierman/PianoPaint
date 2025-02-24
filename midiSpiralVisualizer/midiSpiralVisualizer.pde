int maxPoints = 500; // Total number of points in the spiral
int subsetPoints = 100; // Number of points to display in the subset
float angleStep = 0.1; // Angle increment per point
float radiusStep = 0.1; // Radius increment per point
float offsetX = 0; // Offset to move the spiral horizontally
float offsetY = 0; // Offset to move the spiral vertically

void setup() {
  size(800, 800);
}

void draw() {
  background(30);
  translate(width / 2 + offsetX, height / 2 + offsetY);
  noFill();
  stroke(255, 100, 200);
  strokeWeight(2);

  beginShape();
  for (int i = 0; i < subsetPoints; i++) {
    int pointIndex = (frameCount + i) % maxPoints; // Loop through the spiral points
    float angle = pointIndex * angleStep;
    float radius = pointIndex * radiusStep;
    float x = cos(angle) * radius;
    float y = sin(angle) * radius;
    vertex(x, y);
  }
  endShape();
  
  subsetPoints++;

  // Update the offsets to move the spiral
  //offsetX += speedX;
  //offsetY += speedY;

  // Wrap the offsets to keep the motion within bounds
  //if (offsetX > width / 2 || offsetX < -width / 2) speedX *= -1;
  //if (offsetY > height / 2 || offsetY < -height / 2) speedY *= -1;
}

void keyPressed() {
  if (key == 'w') subsetPoints += 10; // Increase the subset size
  if (key == 's') subsetPoints -= 10; // Decrease the subset size
  subsetPoints = constrain(subsetPoints, 10, maxPoints); // Constrain the subset size
}
