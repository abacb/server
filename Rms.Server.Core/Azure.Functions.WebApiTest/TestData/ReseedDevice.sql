/*�[���f�[�^�폜*/
delete core.DT_DEVICE;

/*����SID=1�ɂ���*/
DBCC CHECKIDENT ('core.DT_DEVICE', RESEED, 0);
