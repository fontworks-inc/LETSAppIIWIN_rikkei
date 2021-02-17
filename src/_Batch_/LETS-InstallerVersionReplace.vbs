Dim fso
Dim orgFile, bakFile
Dim rTs, wts

If Wscript.Arguments.Count < 2 Then
	Wscript.Echo "ファイルパス、バージョンを指定してください"
	WScript.Quit
End If

filePath = WScript.Arguments.Item(0)
newVersion = WScript.Arguments.Item(1)

' ファイルバックアップ
orgFile = filePath
bakFile = orgFile & ".bak"
Set fso = CreateObject("Scripting.FileSystemObject")
Call fso.CopyFile(orgFile, bakFile, true)

Dim objRegExp1
Set objRegExp1 = CreateObject("VBScript.RegExp")
With objRegExp1
	.Pattern = """ProductName"" = ""8:LETS-Ver[^""]*"""
	.IgnoreCase = True
End With
newVersionTag1 = """ProductName"" = ""8:LETS-Ver" & newVersion & """"

Dim objRegExp2
Set objRegExp2 = CreateObject("VBScript.RegExp")
With objRegExp2
	.Pattern = """ProductVersion"" = ""8:[^""]*"""
	.IgnoreCase = True
End With
newVersionTag2 = """ProductVersion"" = ""8:" & newVersion & """"


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
	l2 = newVersionTag1
	l = objRegExp1.Replace(l, l2)
	l2 = newVersionTag2
	l = objRegExp2.Replace(l, l2)
	oadodbobj.WriteText l, 1
Loop

oadodbobj.SaveToFile orgFile, 2

iadodbobj.Close
oadodbobj.Close
