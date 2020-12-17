----------CHECK USER LOGIN
CREATE OR REPLACE PROCEDURE CHECK_USER_LOGIN(usrname VARCHAR2, p_hash VARCHAR2) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM USERS_VIEW WHERE LOWER(USERNAME) = LOWER(usrname) AND PASSWORD_HASH = p_hash;
  DBMS_SQL.RETURN_RESULT(user_cur);
    exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END CHECK_USER_LOGIN;
--------------------

----------CHECK IF USER EXISTS
CREATE OR REPLACE FUNCTION USER_EXISTS(u_id NUMBER) 
RETURN NUMBER
IS
  counter NUMBER := 0;
BEGIN
  SELECT COUNT(*) into counter 
  FROM USERS_VIEW
  WHERE USER_ID = u_id;
  RETURN counter;
    exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END USER_EXISTS;
--------------------

----------CHECK IF USER EXISTS BY USERNAME
CREATE OR REPLACE FUNCTION USER_EXISTS_USERNAME(usrname VARCHAR2) 
RETURN NUMBER
IS
  counter NUMBER := 0;
BEGIN
  SELECT COUNT(*) into counter 
  FROM USERS_VIEW
  WHERE LOWER(USERNAME) = LOWER(usrname);
  RETURN counter;
    exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END USER_EXISTS_USERNAME;
--------------------

----------CHECK IF USER IS ADMIN
CREATE OR REPLACE FUNCTION USER_IS_ADMIN(u_id NUMBER) 
RETURN NUMBER
IS
  counter NUMBER := 0;
BEGIN
  SELECT COUNT(*) into counter 
  FROM ADMINS_VIEW
  WHERE USER_ID = u_id;
  if counter = 0 then return 0;
  else SELECT ADMIN_ID into counter FROM ADMINS_VIEW WHERE USER_ID = u_id; 
  RETURN counter;
  END IF;
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END USER_IS_ADMIN;
--------------------