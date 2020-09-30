/*中止フラグnullの配信ファイルを作成する(SID=2となる)*/
INSERT INTO [core].[DT_DELIVERY_FILE]
           ([DELIVERY_FILE_TYPE_SID]
           ,[INSTALL_TYPE_SID]
           ,[IS_CANCELED]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (1
           ,1
           ,null
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

/*通常データ*/
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
/*開始日が2099年*/
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
           ,'Test2'
           ,'2099/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');
/*配信グループステータスがStarted*/
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
           ,'Test3'
           ,'2020/4/1 0:00:00.0000000'
           ,30
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

/*子エンティティ取得確認用*/
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
