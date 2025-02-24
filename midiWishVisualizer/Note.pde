class Note {
  MidiBus bus;
  int pitch;
  int vel;
  int channel;  
  float lifeSpan = 30;
  boolean finished;
  
  public Note(MidiBus bus, int pitch, int vel) {
    this.bus = bus;
    this.pitch = pitch;
    this.vel = vel;
    this.channel = 1;
    finished = false;
    
    start();
  }
  
  void start() {
    bus.sendNoteOn(channel, pitch, vel);
  }
  
  void stop() {
    bus.sendNoteOff(channel, pitch, vel);
  }
  
  void update() {
    lifeSpan--;
    if(lifeSpan <= 0 && finished == false) {
      stop();
      finished = true;
    }
  }
}
