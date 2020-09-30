/*SID=2となる*/
INSERT INTO [core].[DT_DELIVERY_FILE]
           ([DELIVERY_FILE_TYPE_SID]
           ,[INSTALL_TYPE_SID]
           ,[IS_CANCELED]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,1
           ,'false'
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

INSERT INTO [core].[DT_DELIVERY_MODEL]
           ([DELIVERY_FILE_SID]
           ,[EQUIPMENT_MODEL_SID]
           ,[CREATE_DATETIME])
     VALUES
           (2
           ,1
           ,'2020/4/1 0:00:00.0000000');

/* Startedな配信グループステータスを設定する */
INSERT INTO [core].[DT_DELIVERY_GROUP]
           ([DELIVERY_FILE_SID]
           ,[DELIVERY_GROUP_STATUS_SID]
           ,[NAME]
           ,[START_DATETIME]
           ,[DOWNLOAD_DELAY_TIME]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (2
           ,(select [e].[SID] from [core].[MT_DELIVERY_GROUP_STATUS] as [e]
                where [e].[CODE] = 'started')
           ,'Test1'
           ,'2020/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');
