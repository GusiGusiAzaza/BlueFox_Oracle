----------USERS GENERATION
declare
    X NUMBER(10) := 0;
    u_name VARCHAR2(20);
    p_hash VARCHAR2(15);
begin
    ADD_USER('God', 'MTggICLaZPSay+9');
    INSERT INTO admins (user_id) VALUES (1);
    commit;
    while (x < 200000)
        loop
            X:= X + 1;
            u_name := CONCAT('TestUser',TO_NCHAR(X));
            p_hash := CONCAT('7H/akFCy',TO_NCHAR(X));
            INSERT INTO USERS(USERNAME, PASSWORD_HASH) VALUES (u_name, p_hash);
        end loop;
        commit;
end;
--------------------

EXEC GET_USER_BY_USERNAME('TestUser152678');
EXEC GET_USER_BY_USERNAME('User73772');
EXEC GET_USER_BY_ID(292562);
SELECT count(*) FROM USERS_VIEW;

----------TESTS GENERATION
declare
    TH NUMBER(10) := 102;
    T NUMBER(10) := 0;
    Q NUMBER(10) := 0;
    A NUMBER(10) := 0;
    th_name VARCHAR2(70);
    t_name VARCHAR2(50);
    q_name VARCHAR2(200);
    a_name VARCHAR2(200);
begin
    while (TH < 200) ------THEMES
        loop
            TH:= TH + 1;
            th_name := CONCAT('Theme',TO_NCHAR(TH));
            ADD_THEME(th_name);
                while (T < 200) ------TESTS
                    loop
                        T:= T + 1;
                        t_name := CONCAT(CONCAT(th_name, ' Test '), TO_NCHAR(T));
                        if (T<100) then ADD_TEST (1, t_name, Themes_For_Tests_seq.CURRVAL, T, T, 1);
                        else ADD_TEST (1, t_name, Themes_For_Tests_seq.CURRVAL, T, 77, 1);
                        end if;
                        while (Q < 15) ------QUESTIONS
                            loop
                            Q:= Q + 1;
                            q_name := CONCAT(CONCAT(t_name, ' Question '), TO_NCHAR(Q));
                            ADD_QUESTION (Tests_seq.CURRVAL, Q, q_name);
                            while (A < 4) ------ANSWERS
                                loop
                                    A:= A + 1;
                                    a_name := CONCAT(CONCAT(q_name, ' Answer '), TO_NCHAR(A));
                                    if (A = 2) then ADD_ANSWER (a_name, 1, Questions_For_Tests_seq.CURRVAL);
                                    else ADD_ANSWER (a_name, 0, Questions_For_Tests_seq.CURRVAL);
                                    end if;
                            end loop;
                            A:=0;
                        end loop;
                        Q:=0;
                end loop;
          T:= 0;
        end loop;
end;
--------------------

select * from users;
select * from user_info;
select * from user_stats;
select * from admins;
select * from tests;
select * from questions_for_tests;
select * from answers_for_tests;
select * from user_answers;
select * from test_result;
select * from themes_for_tests;





