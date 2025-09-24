class Squig {
  ArrayList<PVector> trails;
  PVector pos;
  PVector vel;
  PVector acc;
  color c;
  SquigState state;
  float lifespan;
  int reference;
  float weight;
  float maxSpeed;
  
  public Squig(float x, float y, color c, float weight, float maxSpeed, int reference) {
    trails = new ArrayList<PVector>();
    pos = new PVector(x, y);
    vel = new PVector(0, 0);
    acc = new PVector(0, 0);
    this.reference = reference;
    this.weight = weight;
    this.maxSpeed = maxSpeed;
    this.c = c;
    lifespan = 256;
    state = SquigState.GROW;
  }
  
  void update() {
    if(state != SquigState.SHRINK) {
      vel.add(acc);
      vel.limit(maxSpeed);
      pos.add(vel);
      acc.mult(0);
      
      PVector v = new PVector(pos.x, pos.y);
      trails.add(v);
    }

    
    if (state == SquigState.STAY) {
      //lifeSpan--;
      maxSpeed -= 0.5;
      if(maxSpeed <= 0) {
        state = SquigState.SHRINK;
      }
    }
    
    if(state == SquigState.SHRINK){
      lifespan -= 2;
      if(lifespan <= 0) {
        state = SquigState.DEAD;
      }
    }
    
    //if(lifeSpan <= 0 && state == SquigState.STAY) {
    //  state = SquigState.SHRINK;
    //}
    
    //if(trails.size() <= 1 && state == SquigState.SHRINK){
    //  state = SquigState.DEAD;
    //}
  }
  
  void follow(PVector[] vectors) {
    
    int x = floor(pos.x / scl);
    int y = floor(pos.y / scl);
    
    // Make sure we're within the grid
   x = constrain(x, 0, cols-1);
   y = constrain(y, 0, rows-1);
    
    int index = x + y * cols;
    PVector force = vectors[index];
    applyForce(force);
  }
  
  void applyForce(PVector force) {
    acc.add(force);
  }
  
  void show() {
    for (int i = 0; i < trails.size() - 1; i++) {
      PVector pos = trails.get(i);
      PVector nextPos = trails.get(i + 1);
      float alpha = map(i, 0, trails.size(), 0, lifespan + 1);
      stroke(c, alpha);
      noFill();
      var sw = weight * alpha * 0.01;
      if(sw < 0) sw = 0;
      strokeWeight(sw);
      line(pos.x, pos.y, nextPos.x, nextPos.y);
      //circle(pos.x, pos.y, weight);
    }
  }
  
  void edges() {
    if (pos.x > width) {
      pos.x = 0;
    }
    if (pos.x < 0) {
      pos.x = width;
    }
    if (pos.y > height) {
      pos.y = 0;
    }
    if (pos.y < 0) {
      pos.y = height;
    }
  }
}

enum SquigState { GROW, STAY, SHRINK, DEAD }
