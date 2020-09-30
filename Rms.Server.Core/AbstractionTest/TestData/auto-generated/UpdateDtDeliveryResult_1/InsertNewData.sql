/*SID=1*/
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
           ,1
           ,'Test1'
           ,'2020/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

/*SID=2*/
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
           ,1
           ,'Test2'
           ,'2020/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

/*SID=2‚Æ‚È‚é*/
INSERT INTO [core].[DT_DEVICE]
           ([INSTALL_TYPE_SID]
           ,[CONNECT_STATUS_SID]
           ,[EDGE_ID]
           ,[EQUIPMENT_UID]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,1
           ,'12345678-1234-5678-9012-123456789012'
           ,'hogehogehoge'
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000')

INSERT INTO [core].[DT_DELIVERY_RESULT]
           ([DEVICE_SID]
           ,[GW_DEVICE_SID]
           ,[DELIVERY_GROUP_SID]
           ,[CREATE_DATETIME])
     VALUES
           (1
           ,1
           ,1
           ,'2020/4/1 0:00:00.0000000');
