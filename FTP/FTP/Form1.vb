
Imports System.Net
Imports System.IO

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        MoverABackup()
    End Sub

    Private Sub MoverABackup()
        LeerArcFTP("201.212.4.134", "documentos_acofar", "c4RpetaD0cs2015", "/ecommerce/Precios/")
        For Each Arc In FTP_Archivos.Items
            RenameFileName("201.212.4.134", "documentos_acofar", "c4RpetaD0cs2015", "/ecommerce/Precios/", "/ecommerce/Precios/Back/", Arc, Arc)
        Next
    End Sub

    Private Sub RenameFileName(Host As String, Usr As String, Clv As String, DirOri As String, DirDes As String, ByVal currentFilename As String, ByVal newFilename As String)

        Dim reqFTP As FtpWebRequest = Nothing
        Dim ftpStream As Stream = Nothing
        currentFilename = "ftp://" + Host + DirOri + currentFilename
        newFilename = DirDes + newFilename
        Try
            reqFTP = DirectCast(FtpWebRequest.Create(New Uri(currentFilename)), FtpWebRequest)
            reqFTP.Method = WebRequestMethods.Ftp.Rename
            reqFTP.RenameTo = newFilename
            reqFTP.UseBinary = True
            reqFTP.KeepAlive = False
            reqFTP.Credentials = New NetworkCredential(Usr, Clv)

            Dim response As FtpWebResponse = DirectCast(reqFTP.GetResponse(), FtpWebResponse)
            ftpStream = response.GetResponseStream()
            ftpStream.Close()
            response.Close()
        Catch ex As Exception
            If ftpStream IsNot Nothing Then
                ftpStream.Close()
                ftpStream.Dispose()
            End If
            Throw New Exception(ex.Message.ToString())
        End Try
        ' End If
    End Sub


    Private Sub LeerArcFTP(Host As String, Usr As String, Clv As String, Dir As String)
        Dir = "FTP://" + Host + Dir
        FTP_Archivos.Items.Clear()
        Dim dirftp As FtpWebRequest = CType(FtpWebRequest.Create(Dir), FtpWebRequest)
        Dim cr As New NetworkCredential(Usr, Clv)

        ' Para saber si el objeto existe, solicitamos la fecha de creación del mismo
        dirftp.Method = WebRequestMethods.Ftp.GetDateTimestamp
        dirftp.UsePassive = False

        dirftp.Credentials = cr
        dirftp.Method = "LIST"
        ' También usando la enumeración de WebRequestMethods.Ftp
        dirftp.Method = WebRequestMethods.Ftp.ListDirectoryDetails
        ' Obtener el resultado del comando
        Dim reader As New StreamReader(dirftp.GetResponse().GetResponseStream())

        Do While reader.Peek <> -1
            Dim res As String = reader.ReadLine()
            If res.Substring(14, 1) = "1" Then
                FTP_Archivos.Items.Add(res.Substring(InStrRev(res, " ")))

            End If
        Loop
        ' Cerrar el stream abierto.
        reader.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        MoverABackup()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        MoverABackup()
        Close()
    End Sub
End Class
