CREATE TABLE Users (
  User_Id number(10) NOT NULL ,
  Username nvarchar2(20) NOT NULL,
  Password_Hash nvarchar2(15) NOT NULL, 
 PRIMARY KEY (User_Id)
);

CREATE TABLE Admins (
  Admin_Id number(10) NOT NULL ,
  User_Id number(10) NOT NULL, 
 PRIMARY KEY (Admin_Id)
);

CREATE TABLE User_Stats (
  User_Id number(10) NOT NULL,
  Right_Answered number(10),
  Total_Answered number(10),
  Avg_Score number(4,2),
  Finished_Tests_Count number(10),
  Passed_Tests_Count number(10), 
 PRIMARY KEY (User_Id)
);

CREATE TABLE User_Info (
  User_Id number(10) NOT NULL,
  Name nvarchar2(30),
  Gender nvarchar2(15),
  Location nvarchar2(50),
  Birthday timestamp(3),
  Summary nvarchar2(100),
  Education nvarchar2(60),
  Work nvarchar2(60), 
 PRIMARY KEY (User_Id)
);

CREATE TABLE Test_Result (
  Result_Id number(10) ,
  User_Id number(10),
  Test_Id number(10),
  Try_Count number(10),
  Is_Passed number(1),
  Score number(4,2),
  Right_Answers_Count number(10),
  Questions_Count number(10),
  Start_Date date,
  End_Date date,
 PRIMARY KEY (Result_Id)
);

CREATE TABLE Tests (
  Test_Id number(10) NOT NULL ,
  Admin_Id number(10),
  Test_Name nvarchar2(50),
  Theme_Id number(10), 
  Time_Limit_In_Minutes number(10),
  Passing_Score number(4,2),
  Is_Enabled number(1),
 PRIMARY KEY (Test_Id)
);

CREATE TABLE Questions_For_Tests (
  Question_Id number(10) NOT NULL ,
  Test_Id number(10),
  Question_Number number(10),
  Question nvarchar2(200), 
 PRIMARY KEY (Question_Id)
);

CREATE TABLE Answers_For_Tests (
  Answer_Id number(10) NOT NULL ,
  Answer nvarchar2(200),
  Is_Right number(1) NOT NULL,
  Question_Id number(10), 
 PRIMARY KEY (Answer_Id)
);

CREATE TABLE Themes_For_Tests (
  Theme_Id number(10) NOT NULL ,
  Theme_Name nvarchar2(70), 
 PRIMARY KEY (Theme_Id)
);

CREATE TABLE User_Answers (
  Result_Id number(10) NOT NULL,
  Question_Id number(10),
  User_Answer number(10)
);


-- Generate ID using sequence and trigger
CREATE SEQUENCE Users_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Users_seq_tr
 BEFORE INSERT ON Users FOR EACH ROW
 WHEN (NEW.User_Id IS NULL)
BEGIN
 SELECT Users_seq.NEXTVAL INTO :NEW.User_Id FROM DUAL;
END;
/

CREATE SEQUENCE Admins_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Admins_seq_tr
 BEFORE INSERT ON Admins FOR EACH ROW
 WHEN (NEW.Admin_Id IS NULL)
BEGIN
 SELECT Admins_seq.NEXTVAL INTO :NEW.Admin_Id FROM DUAL;
END;
/

CREATE SEQUENCE Test_Result_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Test_Result_seq_tr
 BEFORE INSERT ON Test_Result FOR EACH ROW
 WHEN (NEW.Result_Id IS NULL)
BEGIN
 SELECT Test_Result_seq.NEXTVAL INTO :NEW.Result_Id FROM DUAL;
END;
/

CREATE SEQUENCE Tests_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Tests_seq_tr
 BEFORE INSERT ON Tests FOR EACH ROW
 WHEN (NEW.Test_Id IS NULL)
BEGIN
 SELECT Tests_seq.NEXTVAL INTO :NEW.Test_Id FROM DUAL;
END;
/

CREATE SEQUENCE Questions_For_Tests_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Questions_For_Tests_seq_tr
 BEFORE INSERT ON Questions_For_Tests FOR EACH ROW
 WHEN (NEW.Question_Id IS NULL)
BEGIN
 SELECT Questions_For_Tests_seq.NEXTVAL INTO :NEW.Question_Id FROM DUAL;
END;
/

CREATE SEQUENCE Answers_For_Tests_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Answers_For_Tests_seq_tr
 BEFORE INSERT ON Answers_For_Tests FOR EACH ROW
 WHEN (NEW.Answer_Id IS NULL)
BEGIN
 SELECT Answers_For_Tests_seq.NEXTVAL INTO :NEW.Answer_Id FROM DUAL;
END;
/

CREATE SEQUENCE Themes_For_Tests_seq START WITH 1 INCREMENT BY 1;

CREATE OR REPLACE TRIGGER Themes_For_Tests_seq_tr
 BEFORE INSERT ON Themes_For_Tests FOR EACH ROW
 WHEN (NEW.Theme_Id IS NULL)
BEGIN
 SELECT Themes_For_Tests_seq.NEXTVAL INTO :NEW.Theme_Id FROM DUAL;
END;
/

ALTER TABLE Users ADD CONSTRAINT Username_Unique UNIQUE (username);

ALTER TABLE Themes_For_Tests ADD CONSTRAINT Themes_Unique UNIQUE (Theme_Name);

ALTER TABLE Admins ADD FOREIGN KEY (User_Id) REFERENCES Users (User_Id);
        
ALTER TABLE User_Stats ADD FOREIGN KEY (User_Id) REFERENCES Users (User_Id);
        
ALTER TABLE User_Info ADD FOREIGN KEY (User_Id) REFERENCES Users (User_Id);
        
ALTER TABLE Test_Result ADD FOREIGN KEY (User_Id) REFERENCES Users (User_Id);
        
ALTER TABLE Test_Result ADD FOREIGN KEY (Test_Id) REFERENCES Tests (Test_Id);
        
ALTER TABLE Tests ADD FOREIGN KEY (Admin_Id) REFERENCES Admins (Admin_Id);
        
ALTER TABLE Tests ADD FOREIGN KEY (Theme_Id) REFERENCES Themes_For_Tests (Theme_Id);
        
ALTER TABLE Questions_For_Tests ADD FOREIGN KEY (Test_Id) REFERENCES Tests (Test_Id);
        
ALTER TABLE Answers_For_Tests ADD FOREIGN KEY (Question_Id) REFERENCES Questions_For_Tests (Question_Id) ON DELETE CASCADE;

ALTER TABLE User_Answers ADD FOREIGN KEY (Result_Id) REFERENCES Test_Result (Result_Id);

ALTER TABLE User_Answers ADD FOREIGN KEY (Question_Id) REFERENCES Questions_For_Tests (Question_Id);

ALTER TABLE User_Answers ADD FOREIGN KEY (User_Answer) REFERENCES Answers_For_Tests (Answer_Id);