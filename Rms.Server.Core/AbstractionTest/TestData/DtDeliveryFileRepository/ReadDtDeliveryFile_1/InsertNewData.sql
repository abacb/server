/*SID=2�ƂȂ�*/
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

/*�q�G���e�B�e�B�擾�m�F�p*/
INSERT INTO [core].[DT_DELIVERY_MODEL]
           ([DELIVERY_FILE_SID]
           ,[EQUIPMENT_MODEL_SID]
           ,[CREATE_DATETIME])
     VALUES
           (2
           ,1
           ,'2020/4/1 0:00:00.0000000');

