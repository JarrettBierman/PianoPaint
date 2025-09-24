class NoisyLerpColor {
  float rval;
  float gval;
  float bval;
  
  float inc;
  
  public NoisyLerpColor() {
    rval = random(999);
    bval = random(999);
    bval = random(999);
    inc = 0.01;
  }
  void update() {
    rval += inc;
    gval += inc;
    bval += inc;
  }
  public color GetColor() {
    float r = noise(rval) * 256;
    float g = noise(gval) * 256;
    float b = noise(bval) * 256;
    return color(r, g, b);
  }
  public color GetLerpedColor(float lerpVal) {
    float r = noise(rval) * 256;
    float g = noise(gval) * 256;
    float b = noise(bval) * 256;
    color c1 = color(r,g,b);
    color c2 = color(256-r, 256-g, 256-b);
    return lerpColor(c1, c2, lerpVal);
  }
  public color GetMoreRandomColor(float offset) {
    float r = constrain(noise(rval) * 256 + random(-offset, offset), 0, 256);
    float g = constrain(noise(gval) * 256 + random(-offset, offset), 0, 256);
    float b = constrain(noise(bval) * 256 + random(-offset, offset), 0, 256);
    return color(r, g, b);
  }
}
