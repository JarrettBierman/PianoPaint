class GradientManager {

  color[][] combos = {
    {#FF0000, #00FFFF},
    {#0000FF, #FFFF00},
    {#FF00FF, #00FF00},
    {#00FF00, #FF00FF},
    {#FF5733, #33A1FF},
    {#6B238E, #8EBD23},
    {#E91E63, #4CAF50},
    {#9C27B0, #CDDC39},
    {#3F51B5, #FFEB3B},
    {#F44336, #2196F3},
    {#FF9800, #03A9F4},
    {#8BC34A, #9C27B0},
    {#E040FB, #76FF03},
    {#D50000, #00E5FF},
    {#AA00FF, #FFFF00}
  };
  
  color c1;
  color c2;
  
  color c3;
  color c4;
  
  int counter = 0;
  
  float delay;
  
  public GradientManager(float delay) {
    this.delay = delay;
    selectRandomGradient();
  }
  
  void selectRandomGradient() {
    int r = int(random(combos.length));
    int i = int(random(2));
    c1 = combos[r][i];
    c2 = combos[r][1-i];
    
    r = int(random(combos.length));
    i = int(random(2));
    c3 = combos[r][i];
    c4 = combos[r][1-i];
  }
  
  color gradient(float lerpVal) {
    if(lerpVal < 0.5) {
      return lerpColor(c1, c2, map(lerpVal, 0, 0.5, 0, 1));
    }
    else {
      return lerpColor(c3, c4, map(lerpVal, 0.5, 1, 0, 1));
    }
    
  }
  
  void update() {
    println(counter);
    counter++;
    if(counter >= delay) {
      selectRandomGradient();
      counter = 0;
    }
  }

}
