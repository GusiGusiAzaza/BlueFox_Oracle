CREATE INDEX QUESTION_TEXT_IDX ON QUESTIONS_FOR_TESTS(QUESTION) INDEXTYPE IS CTXSYS.CTXCAT;

CREATE INDEX ANSWER_TEXT_IDX ON ANSWERS_FOR_TESTS (ANSWER) INDEXTYPE IS CTXSYS.CTXCAT;

DROP INDEX ANSWER_IDX;
DROP INDEX QUESTIONS_IDX;
COMMIT;

INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (333, 1, 'One advanceet way. Companions shy hadhich could saw guest man now heard but. Lasted my coming uneasy marked so should. Gravity letters it amongst herself dearest an windows by.');
INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (333, 2, 'Folly words widow one downs few age every seve Disscovered had get considered projection who favourable. Necessary up knowledge it tolerably. ');
INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (333, 3, 'Another journey chamber way yet females man. Too end instrument possession contrasted motionless. Calling offence six joy feeling. ');
INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (333, 4, 'Village did removed enjoyed explain nor ham saw calling talking.  Joy horrible moreover man feelings own shy. Request norland neither mistake for yet.');
INSERT INTO QUESTIONS_VIEW(TEST_ID, QUESTION_NUMBER, QUESTION) VALUES (333, 5, 'In it except to so temper mutual tastes mother Out interested acceptance our partiality affronting unpleasant why add. Esteem garden men yet shy course.');
	
INSERT INTO ANSWERS_VIEW(ANSWER, IS_RIGHT, QUESTION_ID) VALUES ('Had denoting properly jointure you occasion directly raillery. In said to of poor full be post face snug. Introduced imprudence see say unpleasing devonshire acceptance son. ', 1, 525);
INSERT INTO ANSWERS_VIEW(ANSWER, IS_RIGHT, QUESTION_ID) VALUES ('Improve ashamed married expense bed her comfort pursuit mrs. Four time took ye your as fail lady. Up greatest am exertion or marianne. Shy occasional terminated insensible', 0, 525);
INSERT INTO ANSWERS_VIEW(ANSWER, IS_RIGHT, QUESTION_ID) VALUES ('You disposal strongly quitting his endeavor two settling him. Manners ham him hearted hundred expense. Get open game him what hour more part. Adapted as smiling of females oh me journey exposed concern.', 0, 525);
INSERT INTO ANSWERS_VIEW(ANSWER, IS_RIGHT, QUESTION_ID) VALUES ('Effect if in up no depend seemed. Ecstatic elegance gay but disposed. We me rent been part what. An concluded sportsman offending so provision mr education. ', 0, 525);

COMMIT;

SELECT *  FROM QUESTIONS_VIEW WHERE QUESTION LIKE('%norland%');
SELECT *  FROM ANSWERS_VIEW WHERE ANSWER LIKE('%marianne%');
SELECT *  FROM ANSWERS_VIEW WHERE ANSWER LIKE('%Theme138%');

SELECT * FROM QUESTIONS_VIEW WHERE CATSEARCH(QUESTION, 'norland',NULL) > 0;
SELECT * FROM ANSWERS_VIEW WHERE CATSEARCH(ANSWER, 'marianne',NULL) > 0;
SELECT * FROM ANSWERS_VIEW WHERE CATSEARCH(ANSWER, 'THEME138',NULL) > 0;

