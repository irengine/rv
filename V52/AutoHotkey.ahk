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

	IfWinExist 无标题 - 记事本
	{
		WinActivate
	}
	else
	{
		Run Notepad
		WinWait 无标题 - 记事本
		WinActivate
		Send ^v
		Send {F10}
		Send fs
		WinWait 另存为
		FormatTime, TimeString, T12, yyyyMMddhhmmss
		Send E:\VMP\LogData\
		Send %TimeString%{Enter}
		Sleep 500
		WinClose %TimeString%.txt - 记事本
		MouseClick, left, 780, 150
	}
	Sleep 60000
}

return