^!p::Pause

^!n::

Loop
{
	IfWinExist Vestas Remote Panel
	{
		WinActivate
	}
	else
	{
		Run "C:\Program Files\Vestas\Vestas Remote Panel\VrpWin.exe"
		WinWait Vestas Remote Panel
		Sleep 1000
		WinActivate
		Sleep 1000
		Send ^d
		Sleep 6000
	}


	Send {F10}
	Send ea
	Sleep 100
	Send {F10}
	Send ec

	IfWinExist �ޱ��� - ���±�
	{
		WinActivate
	}
	else
	{
		Run Notepad
		WinWait �ޱ��� - ���±�
		WinActivate
		Send ^v
		Send {F10}
		Send fs
		WinWait ���Ϊ
		FormatTime, TimeString, T12, yyyyMMddhhmmss
		Send E:\VMP\LogData\
		Send %TimeString%{Enter}
		Sleep 500
		WinClose %TimeString%.txt - ���±�
		MouseClick, left, 780, 150
	}
	Sleep 60000
}

return