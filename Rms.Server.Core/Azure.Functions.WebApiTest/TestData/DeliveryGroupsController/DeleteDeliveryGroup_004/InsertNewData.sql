INSERT INTO [core].[DT_DELIVERY_GROUP]
           ([DELIVERY_FILE_SID]
           ,[DELIVERY_GROUP_STATUS_SID]
           ,[NAME]
           ,[START_DATETIME]
           ,[DOWNLOAD_DELAY_TIME]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,2
           ,N'WebApiAddTestあ'
           ,'2099/4/1 09:00:00.0000000'
           ,10
           ,'2020/4/1 09:00:00.0000000'
           ,'2020/4/1 09:00:00.0000000');

INSERT INTO [core].[DT_DELIVERY_RESULT]
           ([DEVICE_SID]
           ,[GW_DEVICE_SID]
           ,[DELIVERY_GROUP_SID]
           ,[CREATE_DATETIME])
     VALUES
           (1
           ,1
           ,1
           ,'2020/4/1 09:00:00.0000000'),
           (2
           ,1
           ,1
           ,'2020/4/1 09:00:00.0000000');

INSERT INTO [core].[DT_INSTALL_RESULT]
           ([DEVICE_SID]
           ,[DELIVERY_RESULT_SID]
           ,[INSTALL_RESULT_STATUS_SID]
           ,[COLLECT_DATETIME]
           ,[CREATE_DATETIME])
     VALUES
           (1
           ,1
           ,1
           ,'2020/4/1 09:00:00.0000000'
           ,'2020/4/1 09:00:00.0000000'),
           (2
           ,2
           ,1
           ,'2020/4/1 09:00:00.0000000'
           ,'2020/4/1 09:00:00.0000000');
