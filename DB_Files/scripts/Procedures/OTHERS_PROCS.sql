----------UPDATE USERINFO
CREATE OR REPLACE PROCEDURE UPDATE_USER_INFO (
        u_id number, u_name VARCHAR2, u_gender VARCHAR2, u_location VARCHAR2, u_birthday VARCHAR2, 
        u_summary VARCHAR2, u_education VARCHAR2, u_work VARCHAR2) 
AS
BEGIN
  UPDATE USER_INFO
   SET NAME = u_name, GENDER = u_gender, LOCATION = u_location, BIRTHDAY = TO_TIMESTAMP(u_birthday, 'DD.MM.YYYY'), SUMMARY = u_summary, EDUCATION = u_education, WORK = u_work
   WHERE USER_ID = u_id;
     commit;
      exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END UPDATE_USER_INFO;
--------------------

----------UPDATE TEST_RESULT ON TEST END
CREATE OR REPLACE PROCEDURE UPDATE_TEST_RESULT_TEST_END (
        res_id NUMBER, scor TEST_RESULT.SCORE%TYPE, q_count NUMBER, right_ans_count NUMBER, is_pass TEST_RESULT.IS_PASSED%TYPE, 
        end_d TEST_RESULT.END_DATE%TYPE)
AS
BEGIN
  UPDATE TEST_RESULT
   SET SCORE = scor, QUESTIONS_COUNT = q_count, RIGHT_ANSWERS_COUNT = right_ans_count, IS_PASSED = is_pass, END_DATE = end_d
   WHERE RESULT_ID = res_id;
     commit;
      exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END UPDATE_TEST_RESULT_TEST_END;
--------------------

----------ENABLE/DISABLE TEST
CREATE OR REPLACE PROCEDURE ENABLE_TEST (t_id number, is_enable number) 
AS
BEGIN
  UPDATE TESTS
   SET IS_ENABLED = is_enable WHERE TEST_ID = t_id;
     commit;
      exception
    when others
    then RAISE_APPLICATION_ERROR(sqlerrm, sqlcode);
END ENABLE_TEST;
--------------------