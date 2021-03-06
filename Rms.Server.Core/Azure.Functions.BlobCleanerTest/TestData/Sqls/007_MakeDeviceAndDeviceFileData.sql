/* データ削除 + 次回 SID = 1*/
delete core.DT_DEVICE_FILE_ATTRIBUTE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE_ATTRIBUTE', RESEED, 0);
delete core.DT_DEVICE_FILE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE', RESEED, 0);
delete core.DT_DEVICE;
DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);

/* 端末データテーブル */
INSERT [core].[DT_DEVICE] ([EQUIPMENT_MODEL_SID], [INSTALL_TYPE_SID], [CONNECT_STATUS_SID], [EDGE_ID], [EQUIPMENT_UID], [REMOTE_CONNECT_UID], [RMS_SOFT_VERSION], [CONNECT_START_DATETIME], [CONNECT_UPDATE_DATETIME], [CREATE_DATETIME], [UPDATE_DATETIME]) VALUES (1, 1, 1, N'369e2b6e-e1e1-4263-9af9-643f9c03ac5c', N'ae4540ff-f850-4f2d-a82d-9d89d', N'2cb4052d-135f-41e3-8856-e762d', NULL, NULL, NULL, CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-04-01T00:00:00.0000000' AS DateTime2))

/* 端末ファイルデータテーブル */
INSERT INTO core.DT_DEVICE_FILE (DEVICE_SID, CONTAINER, FILE_PATH, UPDATE_DATETIME, CREATE_DATETIME) VALUES (1, 'device', '369e2b6e-e1e1-4263-9af9-643f9c03ac5c/folder1/filea.json', '2020-03-01T00:00:00.000', '2021-03-01T00:00:00.000')
INSERT INTO core.DT_DEVICE_FILE (DEVICE_SID, CONTAINER, FILE_PATH, UPDATE_DATETIME, CREATE_DATETIME) VALUES (1, 'device', '369e2b6e-e1e1-4263-9af9-643f9c03ac5c/folder1/fileb.json', '2020-04-01T00:00:00.000', '2021-03-01T00:00:00.000')
INSERT INTO core.DT_DEVICE_FILE (DEVICE_SID, CONTAINER, FILE_PATH, UPDATE_DATETIME, CREATE_DATETIME) VALUES (1, 'device', '369e2b6e-e1e1-4263-9af9-643f9c03ac5c/folder2/filec.json', '2020-03-01T00:00:00.000', '2021-03-01T00:00:00.000')
INSERT INTO core.DT_DEVICE_FILE (DEVICE_SID, CONTAINER, FILE_PATH, UPDATE_DATETIME, CREATE_DATETIME) VALUES (1, 'device', '369e2b6e-e1e1-4263-9af9-643f9c03ac5c/folder2/filed.json', '2020-04-01T00:00:00.000', '2021-03-01T00:00:00.000')
