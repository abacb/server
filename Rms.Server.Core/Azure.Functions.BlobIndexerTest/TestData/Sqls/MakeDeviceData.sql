/* �f�[�^�폜 + ���� SID = 1*/
delete core.DT_DEVICE_FILE_ATTRIBUTE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE_ATTRIBUTE', RESEED, 0);
/* �f�[�^�폜 + ���� SID = 1*/
delete core.DT_DEVICE_FILE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE', RESEED, 0);
/* �f�[�^�폜 + ���� SID = 1*/
delete core.DT_DEVICE;
DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);

/* �[���f�[�^�e�[�u�� */
INSERT [core].[DT_DEVICE] ([EQUIPMENT_MODEL_SID], [INSTALL_TYPE_SID], [CONNECT_STATUS_SID], [EDGE_ID], [EQUIPMENT_UID], [REMOTE_CONNECT_UID], [RMS_SOFT_VERSION], [CONNECT_START_DATETIME], [CONNECT_UPDATE_DATETIME], [CREATE_DATETIME], [UPDATE_DATETIME]) VALUES (1, 1, 1, N'0385828a-9a58-4cd6-a705-3a0d95334af9', N'120', N'2cb4052d-135f-41e3-8856-e762d', NULL, NULL, NULL, CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2))
INSERT [core].[DT_DEVICE] ([EQUIPMENT_MODEL_SID], [INSTALL_TYPE_SID], [CONNECT_STATUS_SID], [EDGE_ID], [EQUIPMENT_UID], [REMOTE_CONNECT_UID], [RMS_SOFT_VERSION], [CONNECT_START_DATETIME], [CONNECT_UPDATE_DATETIME], [CREATE_DATETIME], [UPDATE_DATETIME]) VALUES (1, 1, 1, N'7a1f44d1-9a70-4d40-b515-045fcec9e42e', N'abcd1234', N'2cb4052d-135f-41e3-8856-e762e', NULL, NULL, NULL, CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2))