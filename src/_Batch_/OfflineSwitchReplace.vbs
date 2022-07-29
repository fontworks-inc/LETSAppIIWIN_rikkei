Dim fso
Dim orgFile, bakFile
Dim rTs, wts

If Wscript.Arguments.Count < 1 Then
	Wscript.Echo "ファイルパスを指定してください"
	WScript.Quit
End If

filePath = WScript.Arguments.Item(0)

' ファイルバックアップ
orgFile = filePath
bakFile = orgFile & ".bak"
Set fso = CreateObject("Scripting.FileSystemObject")
Call fso.CopyFile(orgFile, bakFile, true)

Dim objRegExp
Set objRegExp = CreateObject("VBScript.RegExp")
With objRegExp
	.Pattern = "__COMPLETELY_OFFLINE__"
	.IgnoreCase = True
End With
compileSwitch = "COMPLETELY_OFFLINE"


Set iadodbobj = CreateObject("ADODB.Stream")
With iadodbobj
	.Open
	.Charset = "UTF-8"
	.LoadFromFile bakFile
End With

Set oadodbobj = CreateObject("ADODB.Stream")
With oadodbobj
	.Open
	.Charset = "UTF-8"
End With

Do Until iadodbobj.EOS
	l = iadodbobj.ReadText(-2)
	l2 = compileSwitch
	l = objRegExp.Replace(l, l2)
	oadodbobj.WriteText l, 1
Loop

oadodbobj.SaveToFile orgFile, 2

iadodbobj.Close
oadodbobj.Close
