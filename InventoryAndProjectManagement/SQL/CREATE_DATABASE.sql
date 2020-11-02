USE GROVERTEST;

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'UNITS')
	CREATE TABLE UNITS
	(
		unit_id INTEGER IDENTITY(1,1),
		unit_name VARCHAR(20) NOT NULL,
		unit_abbreviaton VARCHAR(20),
		allow_partial CHAR(1) NOT NULL,
		PRIMARY KEY(unit_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'IMAGES')
	CREATE TABLE IMAGES
	(
		image_id INTEGER IDENTITY(1,1),
		image_path VARCHAR(MAX) NOT NULL, 
		PRIMARY KEY(image_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'PARTS')
	CREATE TABLE PARTS
	(
		part_id INTEGER IDENTITY(1,1),
		part_name VARCHAR(40) NOT NULL,
		part_description VARCHAR(MAX) NOT NULL, 
		part_unit INTEGER FOREIGN KEY REFERENCES UNITS(unit_id),
		PRIMARY KEY(part_id)
	);


IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'PART_IMAGES')
	CREATE TABLE PART_IMAGES
	(
		part_id INTEGER FOREIGN KEY REFERENCES PARTS(part_id),
		image_id INTEGER FOREIGN KEY REFERENCES IMAGES(image_id),
		PRIMARY KEY(part_id, image_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'PART_INVENTORY')
	CREATE TABLE PART_INVENTORY
	(
		part_id INTEGER FOREIGN KEY REFERENCES PARTS(part_id),
		part_amount FLOAT NOT NULL, 
		PRIMARY KEY(part_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'MACHINES')
	CREATE TABLE MACHINES
	(
		machine_id INTEGER IDENTITY(1,1),
		machine_name VARCHAR(40) NOT NULL,
		machine_description VARCHAR(MAX) NOT NULL, 
		PRIMARY KEY(machine_id)
	);


IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'MACHINE_IMAGES')
	CREATE TABLE MACHINE_IMAGES
	(
		machine_id INTEGER FOREIGN KEY REFERENCES MACHINES(machine_id),
		image_id INTEGER FOREIGN KEY REFERENCES IMAGES(image_id),
		PRIMARY KEY(machine_id, image_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'STEPS')
	CREATE TABLE STEPS
	(
		step_id INTEGER IDENTITY(1,1),
		step_number INTEGER NOT NULL,
		step_description VARCHAR(max),
		PRIMARY KEY(step_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'STEP_IMAGES')
	CREATE TABLE STEP_IMAGES
	(
		step_id INTEGER FOREIGN KEY REFERENCES STEPS(step_id),
		image_id INTEGER FOREIGN KEY REFERENCES IMAGES(image_id),
		PRIMARY KEY(step_id, image_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'MACHINE_STEPS')
	CREATE TABLE MACHINE_STEPS
	(
		step_id INTEGER FOREIGN KEY REFERENCES STEPS(step_id),
		machine_id INTEGER FOREIGN KEY REFERENCES MACHINES(machine_id),
		PRIMARY KEY(step_id, machine_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'STEP_PARTS')
	CREATE TABLE STEP_PARTS
	(
		step_id INTEGER FOREIGN KEY REFERENCES STEPS(step_id),
		part_id INTEGER FOREIGN KEY REFERENCES PARTS(part_id),
		part_amount FLOAT NOT NULL, 
		PRIMARY KEY(step_id,part_id)
	);

IF NOT EXISTS (SELECT [name] FROM sys.tables WHERE [name] = 'PROJECTS')
	CREATE TABLE PROJECTS
	(
		project_id INTEGER IDENTITY(1,1),
		machine_id INTEGER FOREIGN KEY REFERENCES MACHINES(machine_id),
		project_name VARCHAR(50) NOT NULL,
		project_description VARCHAR(MAX),
		current_step Integer NOT NULL DEFAULT(0), 
		complete CHAR(1) NOT NULL,
		PRIMARY KEY(project_id)
	);