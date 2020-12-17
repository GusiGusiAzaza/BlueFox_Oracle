----------GET ALL USERS FROM USERS
CREATE OR REPLACE PROCEDURE GET_USERS IS
  user_cur SYS_REFCURSOR;  
BEGIN
  OPEN user_cur FOR SELECT * FROM USERS_VIEW u LEFT JOIN ADMINS_VIEW a ON a.User_Id = u.User_Id;
  DBMS_SQL.RETURN_RESULT(user_cur);
  exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USERS;
--------------------

----------GET USER BY USERNAME
CREATE OR REPLACE PROCEDURE GET_USER_BY_USERNAME(usrname USERS.USERNAME%TYPE) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM USERS_VIEW WHERE LOWER(USERNAME) = LOWER(usrname);
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_BY_USERNAME;
--------------------

----------GET USER BY ID
CREATE OR REPLACE PROCEDURE GET_USER_BY_ID(u_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM USERS_VIEW WHERE USER_ID = u_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
  exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_BY_ID;
--------------------

----------GET USER WITH INFO
CREATE OR REPLACE PROCEDURE GET_USER_WITH_INFO(u_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM USERS_VIEW u LEFT JOIN USER_INFO info ON u.user_id = info.user_id  WHERE u.user_id = u_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_WITH_INFO;
--------------------

----------GET USER INFO BY ID
CREATE OR REPLACE PROCEDURE GET_USER_INFO(u_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM INFO_VIEW  WHERE user_id = u_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_INFO;
--------------------

----------GET THEMES
CREATE OR REPLACE PROCEDURE GET_THEMES IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM THEMES_VIEW;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_THEMES;
--------------------

----------GET THEME BY NAME
CREATE OR REPLACE PROCEDURE GET_THEME_BY_NAME(th_name VARCHAR2) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM THEMES_VIEW WHERE LOWER(THEME_NAME) = LOWER(th_name);
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_THEME_BY_NAME;
--------------------

----------GET TESTS BY THEME ID
CREATE OR REPLACE PROCEDURE GET_TESTS_BY_THEME_ID(th_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM TESTS_VIEW  WHERE THEME_ID = th_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_TESTS_BY_THEME_ID;
--------------------

----------GET TEST BY NAME
CREATE OR REPLACE PROCEDURE GET_TEST_BY_NAME(t_name VARCHAR2) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM TESTS_VIEW WHERE LOWER(TEST_NAME) = LOWER(t_name);
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_TEST_BY_NAME;
--------------------

----------GET QUESTIONS BY TEST ID
CREATE OR REPLACE PROCEDURE GET_QUESTIONS_BY_TEST_ID(t_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM QUESTIONS_VIEW  WHERE TEST_ID = t_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_QUESTIONS_BY_TEST_ID;
--------------------

----------GET ANSWERS BY QUESTION ID
CREATE OR REPLACE PROCEDURE GET_ANSWERS_BY_QUESTION_ID(q_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM ANSWERS_VIEW  WHERE QUESTION_ID = q_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_ANSWERS_BY_QUESTION_ID;
--------------------

----------GET USER ANSWERS BY RESULT ID
CREATE OR REPLACE PROCEDURE GET_USER_ANSWERS_BY_RESULT_ID(res_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM USER_ANSWERS_VIEW  WHERE RESULT_ID = res_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_ANSWERS_BY_RESULT_ID;
--------------------

----------FIND TEST RESULT
CREATE OR REPLACE PROCEDURE FIND_TEST_RESULT(u_id NUMBER, t_id NUMBER, try_c NUMBER)  IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM TEST_RESULT_VIEW  WHERE TEST_ID = t_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END FIND_TEST_RESULT;
--------------------

----------GET TEST RESULT BY RESULT ID
CREATE OR REPLACE PROCEDURE GET_TEST_RESULT_BY_RESULT_ID(res_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM TEST_RESULT_VIEW WHERE RESULT_ID = res_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
  exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_TEST_RESULT_BY_RESULT_ID;
--------------------

----------GET TEST RESULTS BY USER ID
CREATE OR REPLACE PROCEDURE GET_TEST_RESULTS_BY_USER_ID(u_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM TEST_RESULT_VIEW WHERE USER_ID = u_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
  exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_TEST_RESULTS_BY_USER_ID;
--------------------

----------GET USER STATS BY USER ID
CREATE OR REPLACE PROCEDURE GET_USER_STATS(u_id NUMBER) IS
  user_cur SYS_REFCURSOR;
BEGIN
  OPEN user_cur FOR SELECT * FROM STATS_VIEW WHERE USER_ID = u_id;
  DBMS_SQL.RETURN_RESULT(user_cur);
  exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END GET_USER_STATS;
--------------------