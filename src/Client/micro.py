import RPi.GPIO as GPIO
import time
import requests

s2 = 23
s3 = 24
signal = 25
NUM_CYCLES = 10

def setup():
  GPIO.setmode(GPIO.BCM)
  GPIO.setup(signal,GPIO.IN, pull_up_down=GPIO.PUD_UP)
  GPIO.setup(s2,GPIO.OUT)
  GPIO.setup(s3,GPIO.OUT)
  print("\n")
  

def loop():
  temp = 1
  while(1):  

    GPIO.output(s2,GPIO.LOW)
    GPIO.output(s3,GPIO.LOW)
    time.sleep(0.1)
    start = time.time()
    for impulse_count in range(NUM_CYCLES):
      GPIO.wait_for_edge(signal, GPIO.FALLING)
    duration = time.time() - start 
    red  = NUM_CYCLES / duration   
   
    GPIO.output(s2,GPIO.LOW)
    GPIO.output(s3,GPIO.HIGH)
    time.sleep(0.1)
    start = time.time()
    for impulse_count in range(NUM_CYCLES):
      GPIO.wait_for_edge(signal, GPIO.FALLING)
    duration = time.time() - start
    blue = NUM_CYCLES / duration
    

    GPIO.output(s2,GPIO.HIGH)
    GPIO.output(s3,GPIO.HIGH)
    time.sleep(0.1)
    start = time.time()
    for impulse_count in range(NUM_CYCLES):
      GPIO.wait_for_edge(signal, GPIO.FALLING)
    duration = time.time() - start
    green = NUM_CYCLES / duration

    print("Read: (", red, ", ", green, ", ", blue, ")")
    
    if red > 31000 and red < 34500 and green > 21000 and green < 25000 and blue > 29000 and blue < 31000:
      sendData(100)
      
    #if green<7000 and blue<7000 and red>12000:
    #  sendData(20)
    #  temp=1
    #elif red<12000 and  blue<12000 and green>12000:
    #  sendData(50)
    #  temp=1
    #elif green<7000 and red<7000 and blue>12000:
    #  sendData(100)
    #  temp=1
    #elif green<7000 and red<7000 and blue>12000:
    #  sendData(200)
    #  temp=1
    #elif green<7000 and red<7000 and blue>12000:
    #  sendData(500)
    #  temp=1
    #elif green<7000 and red<7000 and blue>12000:
    #  sendData(1000)
    #  temp=1
    #elif red>10000 and green>10000 and blue>10000 and temp==1:
    #  print("place the object.....")
    #  temp=0

def sendData(amount):
    print("Sending $" + amount + "...")
    r = requests.post("http://localhost:5108/api/v1/payment/a275fcb7-3873-47ae-bb70-fe3aa46172a7", data=amount)
    print(r.status_code, r.reason)
    print(r.text[:300] + '...')
    time.sleep(5)

def endprogram():
    GPIO.cleanup()

if __name__=='__main__':
    
    setup()

    try:
        loop()

    except KeyboardInterrupt:
        endprogram()