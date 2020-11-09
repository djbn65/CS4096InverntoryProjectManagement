USE GROVERTEST;

IF NOT EXISTS (SELECT [unit_name] FROM UNITS WHERE [unit_name] = 'Each')
	INSERT INTO UNITS (unit_name,unit_abbreviaton,allow_partial)
	VALUES('Each','ea.','N');

SELECT * FROM UNITS;