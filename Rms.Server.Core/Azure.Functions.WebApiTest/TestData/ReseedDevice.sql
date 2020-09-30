/*端末データ削除*/
delete core.DT_DEVICE;

/*次回SID=1にする*/
DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);
