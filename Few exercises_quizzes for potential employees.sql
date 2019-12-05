
-- Find number of sundays from beginning of the year till today.

declare @startdate datetime;
declare @enddate datetime;
 
set @startdate = '2018-01-01';
set @enddate = CAST(getdate() as date);
 
with datecte as
(
select  @startdate AS DateValue
union all
select DateValue + 1 from datecte
where DateValue + 1 <= @enddate
)
 
SELECT COUNT(1) as NumOfSunday FROM datecte
WHERE DATENAME(weekday,dateValue)='Sunday'
OPTION (maxrecursion 366)
 
--Or pretty one-liner:
 
SELECT DATEDIFF(wk, '2018-01-01', CAST(getdate() AS DATE));
/**************************************************************/
-- Block SELECT * FROM syntax during querying database.

CREATE TABLE T_TEST(ID INT,SelectStarIsBad AS (1/0));
GO
INSERT INTO T_TEST (ID) VALUES(1);
GO
SELECT ID FROM T_TEST;
GO
SELECT * FROM T_TEST;
GO

/**************************************************************/

--Create table with two columns (TimeStamp (type DATE) and Amount (type INT)) and insert 100 records. TimeStamp column should contain dates from beginning of the year (100 days) and column Amount should be filled with random values from range 10 and 1000. After that compute running total for column Amount for ten first days of February.

CREATE TABLE T_DATA ([TimeStamp] DATE, Amount INT);
GO
 
DECLARE @numOfRecords INT = 100;
DECLARE @index INT = 0;
DECLARE @randomAmount INT;
DECLARE @lowerBound INT = 10;
DECLARE @upperBound INT = 1000;
DECLARE @startDate DATE = CAST('2018-01-01' AS DATE);
DECLARE @date DATE = @startDate;
BEGIN
    SET NOCOUNT ON;
    WHILE @index < @numOfRecords
    BEGIN
          SET @randomAmount = ROUND(((@upperBound - @lowerBound -1) * RAND() + @lowerBound), 0)
          INSERT INTO T_DATA([TimeStamp],Amount) VALUES (@date,@randomAmount);
          SET @date = DATEADD(DAY,1,@date);
          SET @index = @index + 1;
    END;
END;
GO
 
--Solution 1 (CROSS APPLY)
WITH data AS
(SELECT [TimeStamp],Amount FROM T_DATA WHERE [TimeStamp] BETWEEN CAST('2018-02-01' AS DATE) AND CAST('2018-02-10' AS DATE))
SELECT A.[TimeStamp],SUM(B.Amount) FROM data A
CROSS APPLY (SELECT Amount FROM data WHERE [TimeStamp]<=A.[TimeStamp]) B
GROUP BY A.[TimeStamp];
 
--Solution 2 (SUB SELECT)
SELECT A.[TimeStamp], RunningTotal = (
    SELECT SUM(Amount) FROM T_DATA
    WHERE [TimeStamp] <= A.[TimeStamp]
    AND [TimeStamp] >= CAST('2018-02-01' AS DATE)
  )
FROM T_DATA A
WHERE A.[TimeStamp] BETWEEN CAST('2018-02-01' AS DATE) AND CAST('2018-02-10' AS DATE)
ORDER BY A.[TimeStamp];
 
--Solution 3 (WINDOW FUNCTIONS)
SELECT [TimeStamp],SUM(Amount) OVER (ORDER BY [TimeStamp] RANGE UNBOUNDED PRECEDING) AS RUNNING_TOTAL FROM T_DATA
WHERE [TimeStamp] BETWEEN CAST('2018-02-01' AS DATE) AND CAST('2018-02-10' AS DATE);

/**************************************************************/

--Draw triangle (christmas tree (smile)) using asterisk sign (*). The number of tree levels should be defined by variable. Tree should looks like this (this tree is level 5):

DECLARE @n INT = 10;
WITH treeStar AS (
    SELECT 1 AS [Level],REPLICATE(' ',@n)+'*' AS Star
    UNION ALL
    SELECT [Level] + 1,REPLICATE(' ', @n-[Level]) + REPLICATE('*',([Level]*2)+1) AS Star FROM treeStar WHERE [Level] < @n
)
SELECT Star from treeStar;































































