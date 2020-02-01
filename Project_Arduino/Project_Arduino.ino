#include<Wire.h>
#include <SoftwareSerial.h>
#include <math.h>

const int MPU_addr=0x68;  // I2C address of the MPU-6050
int16_t AcX,AcY,AcZ,Tmp,GyX,GyY,GyZ;
int SensorPin = A0;
int SensorPin1 = A1;
int SensorPin2 = A2;
int raserPin = 13;
boolean flag=0;
double angle = 0, deg;
double dgy_y ;
double angle2 = 0, deg2;
double dgy_x ;
int gyroX, gyroY;


SoftwareSerial BTSerial(8, 9);

void setup(){
  Wire.begin();
  Wire.beginTransmission(MPU_addr);
  Wire.write(0x6B);  // PWR_MGMT_1 register
  Wire.write(0);     // set to zero (wakes up the MPU-6050)
  Wire.endTransmission(true);
  pinMode(raserPin, OUTPUT);
  BTSerial.begin(9600);
  Serial.begin(9600);
}

void loop(){

  int SensorReading = analogRead(SensorPin);
  int SensorReading1 = analogRead(SensorPin1);
  int SensorReading2 = analogRead(SensorPin2);
 
  int button1 = map(SensorReading, 0, 1024, 0, 255);
  int button2 = map(SensorReading1, 0, 1024, 0, 255);
  int button3 = map(SensorReading2, 0, 1024, 0, 255);
  
  Wire.beginTransmission(MPU_addr);
  Wire.write(0x3B);  // starting with register 0x3B (ACCEL_XOUT_H)
  Wire.endTransmission(false);
  Wire.requestFrom(MPU_addr,14,true);  // request a total of 14 registers
  
  AcX = Wire.read() << 8 | Wire.read();  // 0x3B (ACCEL_XOUT_H) & 0x3C (ACCEL_XOUT_L)    
  AcY = Wire.read() << 8 | Wire.read();  // 0x3D (ACCEL_YOUT_H) & 0x3E (ACCEL_YOUT_L)
  AcZ = Wire.read() << 8 | Wire.read();  // 0x3F (ACCEL_ZOUT_H) & 0x40 (ACCEL_ZOUT_L)
  Tmp = Wire.read() << 8 | Wire.read();  // 0x41 (TEMP_OUT_H) & 0x42 (TEMP_OUT_L)
  GyX=Wire.read()<<8|Wire.read();  // 0x43 (GYRO_XOUT_H) & 0x44 (GYRO_XOUT_L)
  GyY=Wire.read()<<8|Wire.read();  // 0x45 (GYRO_YOUT_H) & 0x46 (GYRO_YOUT_L)
  GyZ=Wire.read()<<8|Wire.read();  // 0x47 (GYRO_ZOUT_H) & 0x48 (GYRO_ZOUT_L)

  deg = atan2(AcX, AcZ) * 180 / PI ;  //rad to deg
  dgy_y = GyY / 131. ;  //16-bit data to 250 deg/sec
  angle = (0.98 * (angle + (dgy_y * 0.001))) + (0.02 * deg) ; //complementary filter

  deg2 = atan2(AcY, AcZ) * 180 / PI ;  //rad to deg
  dgy_x = GyX / 131. ;  //16-bit data to 250 deg/sec
  angle2 = (0.98 * (angle2 + (dgy_x * 0.001))) + (0.02 * deg2) ; //complementary filter

  gyroX = round(angle*39);
  gyroY = round(angle2*29);

  if (gyroX <0 )
  {
    gyroX = 0;
  }

  if (gyroY <0)
  {
    gyroY = 0;
  }

  Serial.print(gyroX);Serial.print("///");
  Serial.print(gyroY);Serial.println("///");
  

  if(button2>=15)
  {
    flag = !flag;
  }

  if(flag == 1)
  {
    digitalWrite(raserPin, HIGH);
  }
    
  else if(flag == 0)
  {
    digitalWrite(raserPin, LOW);
  }

   if(((button1>=0)&&(button1<5))&&((button2>=0)&&(button2<5))&&((button3>=0)&&(button3<5)))
    {
      BTSerial.print("0/0/0"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
    }

    else if(((button1>=0)&&(button1<5))&&((button2>=0)&&(button2<5))&&(button3>=10))
    {
      BTSerial.print("0/0/1"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
      delay(200);
    }

    else if(((button1>=0)&&(button1<5))&&(button2>=10)&&((button3>=0)&&(button3<5)))
    {
      BTSerial.print("0/1/0"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
      delay(200);
    }

    else if(((button1>=0)&&(button1<5))&&(button2>=10)&&(button3>=10))
    {
      BTSerial.print("0/1/1"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
    }
    
    else if(((button1>=10)&&(button1<15))&&((button2>=0)&&(button2<5))&&((button3>=0)&&(button3<5)))
    {
      BTSerial.print("1/0/0"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
      delay(200);
    }

    else if((button1>=15)&&((button2>=0)&&(button2<5))&&((button3>=0)&&(button3<5)))
    {
      BTSerial.print("2/0/0"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
      delay(200);
    }

    else if((button1>=10)&&((button2>=0)&&(button2<5))&&(button3>=10))
    {
      BTSerial.print("1/0/1"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
    }
    
    else if((button1>=10)&&(button2>=10)&&((button3>=0)&&(button3<5)))
    {
      BTSerial.print("1/1/0"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
    }

    else if((button1>=10)&&(button2>=10)&&(button3>=10))
    {
      BTSerial.print("1/1/1"); BTSerial.print('/');
      BTSerial.print(gyroX); BTSerial.print('/');
      BTSerial.println(gyroY);
    }
 
  delay(10);
}
