----------ADD USER
CREATE OR REPLACE PROCEDURE ADD_USER (
        username VARCHAR2,
        password_hash VARCHAR2) AS
BEGIN
  INSERT INTO USERS(USERNAME, PASSWORD_HASH) VALUES (username, password_hash);
    commit;
    exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END;
--------------------

----------ADD USERINFO
CREATE OR REPLACE PROCEDURE ADD_USER_INFO (
        u_id number, u_name VARCHAR2, u_gender VARCHAR2, u_location VARCHAR2, u_birthday TIMESTAMP, 
        u_summary VARCHAR2, u_education VARCHAR2, u_work VARCHAR2) AS
BEGIN
  INSERT INTO INFO_VIEW VALUES (u_id, u_name, u_gender, u_location, u_birthday, u_summary, u_education, u_work);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_USER_INFO;
--------------------

----------ADD THEME
CREATE OR REPLACE PROCEDURE ADD_THEME (th_name VARCHAR2) 
AS
BEGIN
  INSERT INTO THEMES_VIEW(THEME_NAME) VALUES (th_name);
  commit;
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_THEME;
--------------------

----------ADD TEST
CREATE OR REPLACE PROCEDURE ADD_TEST (a_id NUMBER, t_name VARCHAR2, th_id NUMBER, time_limit NUMBER, pass_score NUMBER, is_enable NUMBER)
AS
BEGIN
  INSERT INTO TESTS_VIEW(ADMIN_ID, TEST_NAME, THEME_ID, TIME_LIMIT_IN_MINUTES, PASSING_SCORE, is_enabled) VALUES (a_id, t_name, th_id, time_limit, pass_score, is_enable);
  commit;
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_TEST;
--------------------

----------ADD QUESTION
CREATE OR REPLACE PROCEDURE ADD_QUESTION (t_id NUMBER, q_number NUMBER, qn VARCHAR2)
AS
BEGIN
  INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (t_id, q_number, qn);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_QUESTION;
--------------------

----------ADD ANSWER
CREATE OR REPLACE PROCEDURE ADD_ANSWER (answ VARCHAR2, is_r NUMBER, q_id NUMBER)
AS
BEGIN
  INSERT INTO ANSWERS_VIEW(ANSWER, IS_RIGHT, QUESTION_ID) VALUES (answ, is_r, q_id);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_ANSWER;
--------------------

----------ADD USER ANSWER
CREATE OR REPLACE PROCEDURE ADD_USER_ANSWER (res_id NUMBER, q_id NUMBER, u_answ NUMBER)
AS
BEGIN
  INSERT INTO USER_ANSWERS_VIEW VALUES (res_id, q_id, u_answ);
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ADD_USER_ANSWER;
--------------------

----------INITIALIZE USER INFO AND STATS
CREATE OR REPLACE PROCEDURE INITIALIZE_USER_INFO_AND_STATS (u_id number) 
AS
BEGIN
  INSERT INTO INFO_VIEW(USER_ID) VALUES (u_id);
  INSERT INTO STATS_VIEW VALUES (u_id, 0, 0, 0, 0, 0);
     exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END INITIALIZE_USER_INFO_AND_STATS;
--------------------

----------INITIALIZE TEST RESULT AND RETURN RESULT_ID
CREATE OR REPLACE FUNCTION FUNC_INITIALIZE_TEST_RESULT(u_id NUMBER, t_id NUMBER, try_c NUMBER, q_count NUMBER, st_date TEST_RESULT.START_DATE%TYPE) 
RETURN NUMBER
IS
BEGIN
    INSERT INTO TEST_RESULT_VIEW(USER_ID, TEST_ID, TRY_COUNT, QUESTIONS_COUNT, START_DATE, RIGHT_ANSWERS_COUNT, SCORE, IS_PASSED) VALUES (u_id, t_id, try_c,  q_count, st_date, 0, 0, 0);
  RETURN Test_Result_seq.CURRVAL;
   exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END FUNC_INITIALIZE_TEST_RESULT;
--------------------