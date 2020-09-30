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
           ,'ParentDeviceUid'
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

/*SID=3‚Æ‚È‚é*/
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
           ,'12345678-1234-5678-9012-123456789013'
           ,'ChildDeviceUid'
           ,'2020/4/1 0:00:00.0000000'
           ,'2020/4/1 0:00:00.0000000');

INSERT INTO [core].[DT_PARENT_CHILD_CONNECT]
           ([PARENT_DEVICE_SID]
           ,[CHILD_DEVICE_SID]
           ,[PARENT_RESULT]
           ,[PARENT_CONFIRM_DATETIME]
           ,[CREATE_DATETIME]
           ,[UPDATE_DATETIME])
     VALUES
           (2
           ,3
           ,'false'
           ,'2099/12/31 0:00:00'
           ,'2020/4/1 0:00:00'
           ,'2020/4/1 0:00:00');
