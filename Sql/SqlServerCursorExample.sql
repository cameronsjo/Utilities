  DECLARE @StartDate AS DATE = '2017-04-01';
  DECLARE @EndDate AS DATE = '2017-05-01';
  
  DECLARE _c CURSOR
  FOR
    WITH    cte
              AS ( SELECT   CASE WHEN DATEPART(DAY, @StartDate) = 1 THEN @StartDate
                                 ELSE DATEADD(MONTH, DATEDIFF(MONTH, 0, @StartDate) + 1, 0)
                            END AS myDate
                   UNION ALL
                   SELECT   DATEADD(MONTH, 1, cte.myDate)
                   FROM     cte
                   WHERE    DATEADD(MONTH, 1, cte.myDate) <= @EndDate )
    SELECT  cte.myDate
    FROM    cte
  OPTION    ( MAXRECURSION 0 );

  OPEN _c;
  DECLARE @Date AS DATETIME;
  FETCH NEXT FROM _c INTO @Date;
  WHILE @@FETCH_STATUS = 0
    BEGIN 
        PRINT CONVERT(NVARCHAR(22), @Date, 121);
		FETCH NEXT FROM _c INTO @Date;
    END;
  CLOSE _c;
  DEALLOCATE _c;
