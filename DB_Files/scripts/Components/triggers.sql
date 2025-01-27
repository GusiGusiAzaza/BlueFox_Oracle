create or replace trigger USER_INFO_INIT
after insert on USERS for each row
begin
   INITIALIZE_USER_INFO_AND_STATS(Users_seq.CURRVAL);
end;

create or replace trigger UPDATE_USER_STATS_ON_TEST_END
after update on TEST_RESULT for each row
begin
    UPDATE USER_STATS SET 
      AVG_SCORE = (AVG_SCORE * FINISHED_TESTS_COUNT + :NEW.SCORE) / (FINISHED_TESTS_COUNT + 1),
      TOTAL_ANSWERED = TOTAL_ANSWERED + :NEW.QUESTIONS_COUNT,
      RIGHT_ANSWERED = RIGHT_ANSWERED + :NEW.RIGHT_ANSWERS_COUNT,
      FINISHED_TESTS_COUNT = FINISHED_TESTS_COUNT + 1, 
      PASSED_TESTS_COUNT = PASSED_TESTS_COUNT + :NEW.IS_PASSED
    WHERE  USER_ID = :OLD.USER_ID;
end;


