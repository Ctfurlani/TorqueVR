#include <Servo.h>
Servo phi_servo;
Servo theta_servo;
int phi = 0;
int theta = 0;

void setup() {
  Serial.begin(9600);
  phi_servo.attach(2); // attaches servo on pin 2
  theta_servo.attach(3);
}

void loop() {
  if (Serial.available() > 0){
    phi = Serial.parseInt();
    theta = Serial.parseInt();
    if (Serial.read() == '\n'){
      // Constrain the values to 0 - 180
      phi = constrain(phi, 0, 180);
      theta = constrain(theta, 0, 180);
      phi_servo.write(phi);
      theta_servo.write(theta);  
    }
  }
  // End of LOOP
}
