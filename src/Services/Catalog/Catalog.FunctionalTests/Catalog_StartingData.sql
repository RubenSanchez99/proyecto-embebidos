INSERT INTO "CatalogType" ("Type") VALUES 
('Sensor')
,('Device')
,('Kit')
,('Wireless')
;

INSERT INTO "CatalogBrand" ("Brand") VALUES 
('Arduino')
,('Raspberry Pi')
,('SparkFun')
,('micro:bit')
,('Other')
;

INSERT INTO "Catalog" ("Name","Description","Price","PictureFileName","CatalogTypeId","CatalogBrandId","AvailableStock","RestockThreshold","MaxStockThreshold","OnReorder") VALUES 
('Raspberry Pi 3 B+ Starter Kit','Raspberry Pi 3 B+ Starter Kit',1800.00,'1.jpg',3,2,100,0,200,False)
,('Arduino Pro Mini 328 - 5V/16MHz','Arduino Pro Mini 328 - 5V/16MHz',200.00,'2.jpg',2,1,89,0,200,True)
,('SparkFun Simultaneous RFID Reader - M6E Nano','SparkFun Simultaneous RFID Reader - M6E Nano',224.95,'3.jpg',2,5,56,0,200,False)
,('SparkFun Inventors Kit - v4.0','SparkFun Inventors Kit - v4.0',1900.00,'4.jpg',3,3,120,0,200,False)
,('SparkFun XBee Explorer USB','SparkFun XBee Explorer USB',600.00,'5.jpg',4,3,55,0,200,False)
,('micro:bit Go Bundle','micro:bit Go Bundle',300.00,'6.jpg',3,4,17,0,200,False)
,('SparkFun Tinker Kit','SparkFun Tinker Kit',890.00,'7.jpg',3,3,8,0,200,False)
,('Load Cell - 10kg, Straight Bar (TAL220)','Load Cell - 10kg, Straight Bar (TAL220)',160.00,'8.jpg',1,5,34,0,200,False)
,('SparkFun RedBoard - Programmed with Arduino','SparkFun RedBoard - Programmed with Arduino',380.00,'9.jpg',2,3,76,0,200,False)
,('Arduino Uno - R3','Arduino Uno - R3',450.00,'10.jpg',2,1,11,0,200,False)
,('SparkFun XBee Explorer Serial','parkFun XBee Explorer Serial',370.00,'11.jpg',4,3,3,0,200,False)
,('micro:bit Educator Lab Pack','micro:bit Educator Lab Pack',300.00,'12.jpg',3,4,0,0,200,False)
;