SET NOCOUNT ON;
DECLARE @TableName    AS SYSNAME = '';
DECLARE @Sql          AS VARCHAR(MAX) = '';
DECLARE @UpdateColumn AS SYSNAME;
DECLARE @ColumnTable  AS TABLE 
(   
    Id              INT IDENTITY(1,1) PRIMARY KEY
  , OrdinalPosition INT
  , ColumnName      SYSNAME
  , IsNullable      BIT
  , DataType        NVARCHAR(128)
  , IsPrimaryKey    BIT
  , MaxLength       INT NULL 
);

INSERT INTO @ColumnTable 
(
    ColumnName
  , OrdinalPosition
  , IsNullable
  , DataType
  , IsPrimaryKey
  , MaxLength
)
SELECT c.COLUMN_NAME as 'ColumnName'
     , c.ORDINAL_POSITION AS 'Ordinal'
     , CAST(CASE WHEN c.IS_NULLABLE = 'YES' THEN 1 ELSE 0 END AS BIT)  AS 'IsNullable'
     , c.DATA_TYPE AS 'DataType'
     , CAST(CASE WHEN PK.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsPrimaryKey
     , CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS c
LEFT JOIN (SELECT ku.TABLE_CATALOG
                , ku.TABLE_SCHEMA
                , ku.TABLE_NAME
                , ku.COLUMN_NAME
            FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku
              ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
             AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME ) AS pk
  ON c.TABLE_CATALOG = pk.TABLE_CATALOG
 AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA
 AND c.TABLE_NAME = pk.TABLE_NAME
 AND c.COLUMN_NAME = pk.COLUMN_NAME
WHERE c.TABLE_NAME = @TableName;

SELECT TOP 1 @UpdateColumn = ColumnName 
FROM @ColumnTable 
WHERE ColumnName LIKE 'Update%' 
AND DataType LIKE '%varchar'
ORDER BY CASE WHEN ColumnName = 'UpdateID'     THEN 1
              WHEN ColumnName = 'UpdateBy'     THEN 2
              WHEN ColumnName = 'UpdatedBy'    THEN 3
              WHEN ColumnName = 'updated_last' THEN 4
              WHEN ColumnName = 'update_user'  THEN 9              
              WHEN ColumnName = 'UpdateDate'   THEN 2140000000
              WHEN ColumnName LIKE '%TS'       THEN 2140000000
              ELSE ColumnName
         END

IF COALESCE(@UpdateColumn,'') = ''
    RAISERROR('Unable to determine Update UserId column', 11, 1);

DECLARE @MaxLength AS INT = ( SELECT TOP 1 MIN(CHARACTER_MAXIMUM_LENGTH) 
                              FROM INFORMATION_SCHEMA.COLUMNS 
                              WHERE TABLE_NAME = 'ChangeLog' 
                                AND COLUMN_NAME IN ( 'OldValue' , 'NewValue') );

IF (SELECT COUNT(1) FROM @ColumnTable WHERE IsPrimaryKey = 1) > 1
    RAISERROR ('Unable to determine Primary Key on provided table', 11, 1);

IF (SELECT TOP 1 DataType FROM @ColumnTable WHERE IsPrimaryKey = 1) <> 'int'
    RAISERROR ('Primary key on provided table is not of `int` type.', 11, 1);

PRINT 'USE [Productivity]                                                                                     ';
PRINT 'GO                                                                                                     ';
PRINT 'SET ANSI_NULLS ON                                                                                      ';
PRINT 'GO                                                                                                     ';
PRINT 'SET QUOTED_IDENTIFIER ON                                                                               ';
PRINT 'GO                                                                                                     ';
PRINT 'CREATE TRIGGER [dbo].['+@TableName+'_ChangeLog]                                                        ';
PRINT '    ON [dbo].['+@TableName+']                                                                          ';
PRINT 'AFTER UPDATE                                                                                           ';
PRINT 'AS BEGIN                                                                                               ';
PRINT CHAR(13) + CHAR(10);

WHILE EXISTS (SELECT TOP 1 * FROM @ColumnTable WHERE IsPrimaryKey <> 1)
  BEGIN
  
    DECLARE @ColumnName  AS SYSNAME
          , @IsNullable  AS BIT
          , @DataType    AS NVARCHAR(128)
          , @Id          AS INT
          , @FieldLength AS INT;

    SELECT TOP 1
           @ColumnName  = ColumnName
         , @IsNullable  = IsNullable
         , @DataType    = DataType
         , @Id          = Id
         , @FieldLength = MaxLength
    FROM @ColumnTable
    WHERE IsPrimaryKey <> 1;

    DECLARE @Length       AS INT = 23;
    DECLARE @ExtraConvert AS VARCHAR(50) = '';
    
    IF (@FieldLength > @MaxLength)
        RAISERROR ('Field length of * is longer than the maximum length of the Old/New Value fields on the ChangeLog table.', 10, 1, @ColumnName);

    IF @DataType <> 'datetime'
        BEGIN
            SET @Length = @MaxLength;
            IF (@DataType LIKE '%varchar')
                SELECT @Length = MIN(A.A) 
                FROM ( SELECT @MaxLength AS A 
                       UNION ALL 
                       SELECT @FieldLength AS A) AS A
        END
    ELSE
        SET @ExtraConvert = ', 120';

    SET @Sql = RTRIM('     IF UPDATE (' + @ColumnName + ')                                                                  ') + CHAR(13) + CHAR(10) +
               RTRIM('     BEGIN                                                                                            ') + CHAR(13) + CHAR(10) +
               RTRIM('         INSERT INTO [dbo].[ChangeLog]                                                                ') + CHAR(13) + CHAR(10) +
               RTRIM('         (    [SourceTable]                                                                           ') + CHAR(13) + CHAR(10) +
               RTRIM('            , [TableKey]                                                                              ') + CHAR(13) + CHAR(10) +
               RTRIM('            , [TableColumn]                                                                           ') + CHAR(13) + CHAR(10) +
               RTRIM('            , [OldValue]                                                                              ') + CHAR(13) + CHAR(10) +
               RTRIM('            , [NewValue]                                                                              ') + CHAR(13) + CHAR(10) +
               RTRIM('            , [UpdateID]                                                                              ') + CHAR(13) + CHAR(10) +
               RTRIM('         )                                                                                            ') + CHAR(13) + CHAR(10) +
               RTRIM('         SELECT ''' + @TableName + '''                                                                ') + CHAR(13) + CHAR(10) +
               RTRIM('              , I.ID                                                                                  ') + CHAR(13) + CHAR(10) +
               RTRIM('              , ''' + @ColumnName + '''                                                               ') + CHAR(13) + CHAR(10) +
               RTRIM('              , CONVERT(VARCHAR('+CONVERT(VARCHAR,@Length)+'), D.' + @ColumnName + @ExtraConvert + ') ') + CHAR(13) + CHAR(10) +
               RTRIM('              , CONVERT(VARCHAR('+CONVERT(VARCHAR,@Length)+'), I.' + @ColumnName + @ExtraConvert + ') ') + CHAR(13) + CHAR(10) +
               RTRIM('              , I.UpdateBy                                                                            ') + CHAR(13) + CHAR(10) +
               RTRIM('         FROM Inserted I                                                                              ') + CHAR(13) + CHAR(10) +
               RTRIM('         LEFT OUTER JOIN Deleted D                                                                    ') + CHAR(13) + CHAR(10) +
               RTRIM('           ON I.ID = D.ID                                                                             ') + CHAR(13) + CHAR(10) +
               RTRIM('         WHERE I.' + @ColumnName + ' <> COALESCE(D.' + @ColumnName + ', '''')                         ') + CHAR(13) + CHAR(10) +
               RTRIM('     END                                                                                              ') ;
    PRINT @Sql;
    PRINT CHAR(13) + CHAR(10);

    DELETE FROM @ColumnTable WHERE Id = @Id;

  END

  PRINT 'END';
