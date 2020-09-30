INSERT INTO [core].[DT_DEVICE]
           ([EQUIPMENT_MODEL_SID]
           ,[INSTALL_TYPE_SID]
           ,[CONNECT_STATUS_SID]
           ,[EDGE_ID]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,1
           ,1
           ,newid()
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

INSERT INTO [core].[DT_DELIVERY_FILE]
           ([DELIVERY_FILE_TYPE_SID]
           ,[INSTALL_TYPE_SID]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,1
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

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
           ,1
           ,'Test1'
           ,'2020/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

INSERT INTO [core].[DT_DELIVERY_RESULT]
           ([DEVICE_SID]
           ,[GW_DEVICE_SID]
           ,[DELIVERY_GROUP_SID]
           ,[CREATE_DATETIME])
     VALUES
           (2
           ,2
           ,1
           ,'2020/4/1 0:00:00.0000000');

INSERT INTO [core].[DT_DELIVERY_MODEL]
           ([DELIVERY_FILE_SID]
           ,[EQUIPMENT_MODEL_SID]
           ,[CREATE_DATETIME])
     VALUES
           (2
           ,1
           ,'2020/4/1 0:00:00.0000000');
