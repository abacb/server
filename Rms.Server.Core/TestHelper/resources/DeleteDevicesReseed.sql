delete core.DT_DEVICE_FILE_ATTRIBUTE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE_ATTRIBUTE', RESEED, 0);
delete core.DT_DEVICE_FILE;
DBCC CHECKIDENT ('core.DT_DEVICE_FILE', RESEED, 0);
delete core.DT_DIRECTORY_USAGE;
DBCC CHECKIDENT ('core.DT_DIRECTORY_USAGE', RESEED, 0);
delete core.DT_PARENT_CHILD_CONNECT;
DBCC CHECKIDENT ('core.DT_PARENT_CHILD_CONNECT', RESEED, 0);
delete core.DT_DEVICE;
DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);
