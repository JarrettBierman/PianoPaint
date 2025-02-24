class BigLetter {
  char letter;
  float x;
  float y;
  float size;
  float lifespan;
  float deathRate;
  float growRate;
  
  color c;
  
  public BigLetter(char letter, float x, float y, float size, color c) {
    this.letter = letter;
    this.x = x;
    this.y = y;
    this.size = size;
    this.c = c;
    
    lifespan = 255;
    deathRate = 2;   
    growRate = random(2, 10);
  }
  
  void update() {
    if(lifespan > 0)
      lifespan -= deathRate;
    if(lifespan < 0)
      lifespan = 0;
    size += growRate;
  }
  
  void show() {
    pushMatrix();
    textSize(size);
    fill(c, lifespan);
    text(letter, x, y);
    popMatrix();
  }
}
